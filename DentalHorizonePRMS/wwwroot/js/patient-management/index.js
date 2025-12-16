let patients = [];
window.currentPatientId = null;


async function loadPatients() {
    try {
        const response = await fetch("/api/Patient/active-patients");
        if (!response.ok) throw new Error("Failed to load patients.");
        const data = await response.json();
        if (!Array.isArray(data)) throw new Error("Invalid response format");

        patients = data;
        generateYearOptions();
        renderPatients(patients);
    } catch (err) {
        console.error(err);
        alert("Failed to load patients. Check console.");
    }
}

function generateYearOptions() {
    const yearSelect = document.getElementById("yearSelect");
    if (!yearSelect) return;

    // Clear existing options
    yearSelect.innerHTML = "";

    // Add "All"
    yearSelect.innerHTML += `<option value="0">All</option>`;

    if (patients.length === 0) return;

    // Extract all years from patient records
    const years = patients.map(p => new Date(p.dateOfVisit).getFullYear());

    // Get unique years, sort descending
    const uniqueYears = [...new Set(years)].sort((a, b) => b - a);

    // Add each year
    uniqueYears.forEach(y => {
        yearSelect.innerHTML += `<option value="${y}">${y}</option>`;
    });
}

// Disable month when year = All
document.getElementById("yearSelect").addEventListener("change", function () {
    const monthSelect = document.getElementById("monthSelect");

    if (this.value === "0") {
        monthSelect.value = "0";
        monthSelect.disabled = true;
    } else {
        monthSelect.disabled = false;
    }

    filterCustom();
});

function resetFilters() {
    document.getElementById("searchInput").value = "";
    document.getElementById("monthSelect").value = "0"; // All
    document.getElementById("yearSelect").value = "0";  // All

    // Re-enable month dropdown
    document.getElementById("monthSelect").disabled = false;

    // Show all patients
    renderPatients(patients);
}

function renderPatients(list) {
    let tbody = document.getElementById("patientsTable");
    if (!tbody) return;
    if (tbody.tagName !== "TBODY") tbody = tbody.querySelector("tbody");
    if (!tbody) return;

    tbody.innerHTML = "";

    list.forEach((p) => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td class="px-4 py-2">${p.patientName}</td>
            <td class="px-4 py-2">${p.age}</td>
            <td class="px-4 py-2">${p.occupation}</td>
            <td class="px-4 py-2">${p.telephone}</td>
            <td class="px-4 py-2">${p.address}</td>
            <td class="px-4 py-2">${formatDateForTable(p.dateOfVisit)}</td>
            <td class="px-4 py-2">${p.nextAppointment ? formatDateForTable(p.nextAppointment) : "-"}</td>
            <td class="px-4 py-2">${p.complaint}</td>
            <td class="px-4 py-2">${p.service}</td>
            <td class="px-4 py-2">${p.debit}</td>
            <td class="px-4 py-2">${p.credit}</td>
            <td class="px-4 py-2">${p.balance}</td>
            <td class="px-4 py-2">${p.status}</td>
            <td class="px-4 py-2">${p.visitStatus}</td>
            <td class="px-4 py-2">${p.patientStatus}</td>
            <td class="px-4 py-2 space-x-2">
                <button onclick="openEditPatientForm(${p.id})" class="text-blue-600">Edit</button>
                <button onclick="openPmDeleteModal(${p.id})" class="text-red-600">Archive</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function filterCustom() {
    const search = document.getElementById("searchInput").value.toLowerCase();

    const month = document.getElementById("monthSelect").value; // "0" or "1-12"
    const year = document.getElementById("yearSelect").value;   // "0" or "2024/2025"

    let filtered = patients.filter(p => {
        const nameMatch = p.patientName.toLowerCase().includes(search);
        if (!nameMatch) return false;

        const date = new Date(p.dateOfVisit);
        const visitMonth = date.getMonth() + 1;
        const visitYear = date.getFullYear();

        // ✅ Case 1: Year = All AND Month = All → show everything
        if (year === "0" && month === "0") {
            return true;
        }

        // ✅ Case 2: Year = specific AND Month = All → filter by year only
        if (year !== "0" && month === "0") {
            return visitYear == year;
        }

        // ✅ Case 3: Year = All AND Month = specific → ignore month filter
        if (year === "0" && month !== "0") {
            return true;
        }

        // ✅ Case 4: Year = specific AND Month = specific → filter both
        return visitYear == year && visitMonth == month;
    });

    renderPatients(filtered);
}

document.addEventListener("DOMContentLoaded", loadPatients);

const servicePrices = {
    "Consultation": 500,
    "Cleaning / Prophylaxis": 1000,
    "Tooth Extraction": 1000,
    "Tooth Filling": 1000,
    "Veneers": 5000,
    "Jacket Crown": 5000,
    "Retainer": 3000,
    "Plastic Denture": 1000,
    "Full Denture": 1500
};

// Edit form
async function openEditPatientForm(id) {
    const patient = await fetch(`/api/Patient/${id}`).then(r => r.json());
    currentPatientId = id;

    const modal = document.getElementById("pmEditModal");
    modal.classList.remove("hidden");

    const form = document.getElementById("pmEditForm");

    form.elements["patientName"].value = patient.patientName ?? "";
    form.elements["age"].value = patient.age ?? "";
    form.elements["occupation"].value = patient.occupation ?? "";
    form.elements["telephone"].value = patient.telephone ?? "";
    form.elements["address"].value = patient.address ?? "";
    form.elements["dateOfVisit"].value = patient.dateOfVisit ? formatDateForInput(patient.dateOfVisit) : "";
    form.elements["nextAppointment"].value = patient.nextAppointment ? formatDateForInput(patient.nextAppointment) : "";
    form.elements["complaint"].value = patient.complaint ?? "";
    form.elements["service"].value = patient.service ?? "";
    form.elements["patientStatus"].value = patient.patientStatus ?? "Active";
    form.elements["visitStatus"].value = patient.visitStatus ?? "Pending";
    form.elements["debit"].value = patient.debit ?? 0;
    form.elements["credit"].value = patient.credit ?? 0;
    form.elements["balance"].value = patient.balance ?? 0;

    // Balance auto-calc
    const debitField = form.elements["debit"];
    const creditField = form.elements["credit"];
    const balanceField = form.elements["balance"];
    updateBalance(debitField, creditField, balanceField);
    debitField.oninput = () => updateBalance(debitField, creditField, balanceField);
    creditField.oninput = () => updateBalance(debitField, creditField, balanceField);

    // Checkbox logic
    const checkbox = document.getElementById("editNoNextAppointmentCheckbox");
    const nextAppointmentInput = form.elements["nextAppointment"];
    checkbox.checked = !patient.nextAppointment;
    nextAppointmentInput.disabled = checkbox.checked;
    checkbox.onchange = () => {
        nextAppointmentInput.disabled = checkbox.checked;
        if (checkbox.checked) nextAppointmentInput.value = "";
    };

    // Debit auto-update when selecting a service
    const serviceField = form.elements["service"];
    serviceField.onchange = () => {
        const selectedService = serviceField.value;
        if (servicePrices[selectedService] !== undefined) {
            debitField.value = servicePrices[selectedService];
            updateBalance(debitField, creditField, balanceField);
        }
    };

    // Run once when opening (so debit matches service immediately)
    if (servicePrices[serviceField.value] !== undefined) {
        debitField.value = servicePrices[serviceField.value];
        updateBalance(debitField, creditField, balanceField);
    }
}

// Close edit modal
function closePmEditModal() {
    document.getElementById("pmEditModal").classList.add("hidden");
}

// Submit edit form
async function submitPmEditForm(event) {
    event.preventDefault();
    if (!currentPatientId) return;

    const form = event.target;
    const checkbox = document.getElementById("editNoNextAppointmentCheckbox");

    const payload = {
        patientName: form.elements["patientName"].value,
        address: form.elements["address"].value,
        telephone: form.elements["telephone"].value,
        age: parseInt(form.elements["age"].value),
        occupation: form.elements["occupation"].value,
        complaint: form.elements["complaint"].value,
        // send raw yyyy-mm-dd string (no timezone conversion)
        dateOfVisit: form.elements["dateOfVisit"].value || null,
        nextAppointment: checkbox.checked ? null : form.elements["nextAppointment"].value || null,
        service: form.elements["service"].value,
        visitStatus: form.elements["visitStatus"].value,
        status: form.elements["visitStatus"].value,
        patientStatus: form.elements["patientStatus"].value,
        debit: parseFloat(form.elements["debit"].value),
        credit: parseFloat(form.elements["credit"].value),
        balance: parseFloat(form.elements["balance"].value)
    };

    try {
        const response = await fetch(`/api/Patient/${currentPatientId}/update-patient`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            closePmEditModal();
            await loadPatients();
            showSuccessModal("Patient updated successfully!");
        } else {
            const errorText = await response.text();
            showErrorModal("Failed to update patient: " + errorText);
        }
    } catch (err) {
        console.error("Update error:", err);
        showErrorModal("Network error while updating patient.");
    }
}

// Helpers
function formatDateForInput(date) {
    if (!date) return "";
    // If backend sends ISO string, slice only the date part
    return date.toString().substring(0, 10); // yyyy-MM-dd
}

function formatDateForTable(date) {
    if (!date) return "";
    return new Date(date).toLocaleDateString(); // show nicely in table
}

function updateBalance(debitInput, creditInput, balanceInput) {
    const debit = parseFloat(debitInput.value) || 0;
    const credit = parseFloat(creditInput.value) || 0;
    balanceInput.value = debit - credit;
}

// Delete modal
let patientIdToDelete = null;

function openPmDeleteModal(id) {
    patientIdToDelete = id;
    document.getElementById("pmDeleteModal").classList.remove("hidden");
}

function closePmDeleteModal() {
    patientIdToDelete = null;
    document.getElementById("pmDeleteModal").classList.add("hidden");
}

async function confirmDeletePatient() {
    if (!patientIdToDelete) return;

    try {
        const response = await fetch(`/api/ArchivedPatients/${patientIdToDelete}/soft-delete`, {
            method: "PUT"
        });

        if (response.ok) {
            closePmDeleteModal();
            await loadPatients(); // reload active patients
            showSuccessModal("Patient archived successfully!");
        } else {
            const errorText = await response.text();
            showErrorModal("Failed to archive patient: " + errorText);
        }
    } catch (err) {
        console.error("Delete error:", err);
        showErrorModal("Network error while archiving patient.");
    }
}

