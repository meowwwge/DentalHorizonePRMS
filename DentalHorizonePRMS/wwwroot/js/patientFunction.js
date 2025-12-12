// -------------------- SAFE DATE HELPER --------------------
function safeDate(date) {
    try {
        const parsed = new Date(date);
        if (!date || isNaN(parsed.getTime()) || parsed.getFullYear() < 2000) return "-";
        return parsed.toLocaleDateString();
    } catch {
        return "-";
    }
}

// -------------------- LOAD PATIENTS --------------------
async function loadPatients() {
    try {
        const response = await fetch('/api/Patient/all-patients');
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Failed to fetch patients: ${response.status} ${errorText}`);
        }

        const patients = await response.json();
        const tableBody = document.getElementById("patientsTable");
        tableBody.innerHTML = "";

        patients.forEach(patient => {
            try {
                addPatientToTable(patient);
            } catch (rowErr) {
                console.error("Error rendering patient row:", rowErr, patient);
            }
        });
    } catch (error) {
        console.error("Error loading patients:", error);
        if (error.message.includes("Network") || error.message.includes("Failed to fetch")) {
            showErrorModal("Unable to load patient list. Please check your connection.");
        }
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const patientTable = document.getElementById("patientsTable");
    const yearSelect = document.getElementById("yearSelect");

    if (patientTable && yearSelect) {
        loadYearOptions();
        filterCustom();
        refreshTables();
    }
});

setInterval(refreshTables, 60000); // Auto-refresh every 60s

// -------------------- REFRESH TABLES --------------------
async function refreshTables() {
    try {
        const patients = await fetch('/api/Patient/all-patients').then(r => r.json());
        const tableBody = document.getElementById("patientsTable");
        tableBody.innerHTML = "";
        patients.forEach(p => {
            try {
                addPatientToTable(p);
            } catch (rowErr) {
                console.error("Error rendering patient row:", rowErr, p);
            }
        });

        const upcoming = await fetch('/api/Patient/upcoming-appointments').then(r => r.json());
        renderUpcoming(upcoming);

        const missed = await fetch('/api/Patient/missed-appointments').then(r => r.json());
        renderMissed(missed);

        const totals = await fetch('/api/Patient/dashboard-totals').then(r => r.json());
        renderDashboardTotals(totals);
    } catch (err) {
        console.error("Error refreshing tables:", err);
    }
}

// -------------------- UPCOMING --------------------
function renderUpcoming(upcoming) {
    const tbody = document.getElementById("upcomingTable");
    tbody.innerHTML = "";

    if (!upcoming || upcoming.length === 0) {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td colspan="8" class="px-4 py-6 text-center text-gray-500 italic">
                No upcoming appointments.
            </td>
        `;
        tbody.appendChild(row);
        return;
    }

    upcoming.forEach(p => {
        const row = document.createElement("tr");
        row.className = "hover:bg-blue-50 transition duration-150 ease-in-out";

        row.innerHTML = `
            <td class="px-4 py-3">${p.patientName || "-"}</td>
            <td class="px-4 py-3">${p.age ?? "-"}</td>
            <td class="px-4 py-3">${p.telephone || "-"}</td>
            <td class="px-4 py-3">${safeDate(p.nextAppointment)}</td>
            <td class="px-4 py-3">${p.service || "-"}</td>
            <td class="px-4 py-3">${p.complaint || "-"}</td>
            <td class="px-4 py-3">
                <span class="px-2 py-1 rounded-full text-xs font-semibold bg-yellow-100 text-yellow-700">
                    ${p.status || "Upcoming"}
                </span>
            </td>
            <td class="px-4 py-3 space-x-2">
                <button class="px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700 reschedule-btn" 
                        data-id="${p.id}" 
                        data-date="${p.nextAppointment}">
                    Reschedule
                </button>
                <button class="cancel-btn px-3 py-1 text-xs bg-red-600 text-white rounded hover:bg-red-700" 
                        data-id="${p.id}">
                    Cancel
                </button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

// -------------------- MISSED --------------------
function renderMissed(missed) {
    const tbody = document.getElementById("missedTable");
    tbody.innerHTML = "";

    if (!missed || missed.length === 0) {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td colspan="8" class="px-4 py-6 text-center text-gray-500 italic">
                No patients with missed appointments.
            </td>
        `;
        tbody.appendChild(row);
        return;
    }

    missed.forEach(p => {
        const row = document.createElement("tr");
        row.className = "hover:bg-red-50 transition duration-150 ease-in-out";

        row.innerHTML = `
            <td class="px-4 py-3">${p.patientName || "-"}</td>
            <td class="px-4 py-3">${p.age ?? "-"}</td>
            <td class="px-4 py-3">${p.telephone || "-"}</td>
            <td class="px-4 py-3">${safeDate(p.originalAppointmentDate ?? p.nextAppointment)}</td>
            <td class="px-4 py-3">${p.service || "-"}</td>
            <td class="px-4 py-3">${p.complaint || "-"}</td>
            <td class="px-4 py-3">
                <span class="px-2 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-700">
                    ${p.status || "Missed"}
                </span>
            </td>
            <td class="px-4 py-3 space-x-2">
                <button class="px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700 reschedule-btn" 
                        data-id="${p.id}" 
                        data-date="${p.originalAppointmentDate ?? p.nextAppointment}">
                    Reschedule
                </button>
                <button class="cancel-btn px-3 py-1 text-xs bg-red-600 text-white rounded hover:bg-red-700" 
                        data-id="${p.id}">
                    Cancel
                </button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

// -------------------- RESCHEDULE --------------------
document.addEventListener("click", (e) => {
    const btn = e.target.closest(".reschedule-btn");
    if (!btn) return;

    currentPatientId = btn.getAttribute("data-id");
    const modal = document.getElementById("rescheduleModal");
    modal.classList.remove("hidden");

    const dateInput = document.getElementById("newDate");
    const currentDate = btn.getAttribute("data-date");
    if (dateInput && currentDate) {
        const parsed = new Date(currentDate);
        if (!isNaN(parsed.getTime())) {
            dateInput.value = parsed.toISOString().split("T")[0];
        }
    }
});

document.getElementById("cancelReschedule").addEventListener("click", () => {
    document.getElementById("rescheduleModal").classList.add("hidden");
    currentPatientId = null;
});

document.getElementById("confirmReschedule").addEventListener("click", async () => {
    const newDate = document.getElementById("newDate").value;
    if (!newDate || !currentPatientId) return;

    try {
        const response = await fetch(`/api/Patient/${currentPatientId}/reschedule`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(new Date(newDate).toISOString())
        });

        if (response.ok) {
            showSuccessModal("Appointment rescheduled successfully!");
            await refreshTables();
        } else {
            const errorText = await response.text();
            showErrorModal("Failed to reschedule: " + errorText);
        }
    } catch (err) {
        console.error("Error rescheduling:", err);
        showErrorModal("Network error while rescheduling.");
    } finally {
        document.getElementById("rescheduleModal").classList.add("hidden");
        currentPatientId = null;
    }
});

// -------------------- CANCEL --------------------
document.addEventListener("click", async (e) => {
    const btn = e.target.closest(".cancel-btn");
    if (!btn) return;

    const patientId = btn.getAttribute("data-id");
    if (!patientId) return;

    const confirmed = confirm("Are you sure you want to cancel this appointment?");
    if (!confirmed) return;

    try {
        const response = await fetch(`/api/Patient/${patientId}/cancel`, {
            method: "PUT"
        });

        if (response.ok) {
            const row = btn.closest("tr");
            if (row) row.remove();

            showSuccessModal("Appointment cancelled successfully!");
            await refreshTables(); // ✅ keeps dashboard and other tables in sync
        } else {
            const errorText = await response.text();
            showErrorModal("Failed to cancel appointment: " + errorText);
        }
    } catch (err) {
        console.error("Error cancelling:", err);
        showErrorModal("Network error while cancelling.");
    }
});



// SEARCH
function handleSearch(inputId, endpoint, renderFn, fallbackEndpoint) {
    const input = document.getElementById(inputId);
    if (!input) return;

    input.addEventListener("input", async (e) => {
        const keyword = e.target.value.trim();

        if (!keyword) {
            try {
                const data = await fetch(fallbackEndpoint).then(r => r.json());
                renderFn(data);
            } catch (err) {
                console.error("Error loading fallback data:", err);
            }
            return;
        }

        try {
            const separator = endpoint.includes("?") ? "&" : "?";
            const response = await fetch(`${endpoint}${separator}keyword=${encodeURIComponent(keyword)}`);

            if (response.ok) {
                const results = await response.json();
                renderFn(results);
            } else {
                const errorText = await response.text();
                console.error("Search failed:", errorText);
            }
        } catch (err) {
            console.error("Search error:", err);
        }
    });
}

// -------------------- FILTER + SEARCH MAIN PATIENTS --------------------
async function filterCustom() {
    try {
        const selectedMonth = document.getElementById("monthSelect").value;
        const selectedYear = document.getElementById("yearSelect").value;
        const searchTerm = document.getElementById("searchInput")?.value || "";

        // ✅ Build base URL
        let url = "/api/Patient/all-patients";

        // ✅ Build query params based on your new rules
        const params = [];

        if (selectedMonth !== "0") {
            params.push(`month=${selectedMonth}`);
        }

        if (selectedYear !== "All") {
            params.push(`year=${selectedYear}`);
        }

        if (params.length > 0) {
            url += "?" + params.join("&");
        }

        console.log("Fetching:", url);

        const response = await fetch(url);
        if (!response.ok) {
            const errorText = await response.text();
            console.error("Failed to fetch patients:", response.status, errorText);
            throw new Error(`Failed to fetch patients: ${response.status} ${errorText}`);
        }

        const patients = await response.json();
        const tableBody = document.getElementById("patientsTable");
        tableBody.innerHTML = "";

        // ✅ Safe search: handle null/undefined names
        const filtered = patients.filter(p => {
            const name = (p.patientName || p.name || "").toLowerCase();
            return name.includes(searchTerm.toLowerCase());
        });

        if (filtered.length === 0) {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td colspan="16" class="px-4 py-6 text-center text-gray-500 italic">
                    No patients found for the selected filters.
                </td>
            `;
            tableBody.appendChild(row);
            return;
        }

        filtered.forEach(patient => {
            try {
                addPatientToTable(patient);
            } catch (rowErr) {
                console.error("Error rendering patient row:", rowErr, patient);
            }
        });
    } catch (error) {
        console.error("Error filtering patients:", error);
        showErrorModal("Unable to filter patient list. Please check your connection.");
    }

    console.log("Selected month:", selectedMonth);
    console.log("Selected year:", selectedYear);
    console.log("Fetch URL:", url);
}

// -------------------- LOAD YEAR OPTIONS --------------------
async function loadYearOptions() {
    try {
        const response = await fetch("/api/Patient/available-years");
        if (!response.ok) throw new Error("Failed to load years");

        const years = await response.json();
        const yearSelect = document.getElementById("yearSelect");

        // Clear existing options
        yearSelect.innerHTML = "";

        // Add default "All" option
        const defaultOption = document.createElement("option");
        defaultOption.value = "All";
        defaultOption.textContent = "All";
        yearSelect.appendChild(defaultOption);

        // Add dynamic year options
        years.forEach(year => {
            const option = document.createElement("option");
            option.value = year;
            option.textContent = year;
            yearSelect.appendChild(option);
        });
    } catch (err) {
        console.error("Error loading year options:", err);
    }
}

function addPatientToTable(patient) {
    const tableBody = document.getElementById("patientsTable");
    if (!tableBody || !patient || !patient.id) return;

    const existingRow = tableBody.querySelector(`tr[data-patient-id="${patient.id}"]`);
    if (existingRow) existingRow.remove();

    const safeDate = (date) => {
        try {
            return date ? new Date(date).toLocaleDateString() : "N/A";
        } catch {
            return "N/A";
        }
    };

    const row = document.createElement("tr");
    row.setAttribute("data-patient-id", patient.id);

    row.innerHTML = `
        <td>${patient.patientName || "N/A"}</td>
        <td>${patient.age ?? "N/A"}</td>
        <td>${patient.occupation || "N/A"}</td>
        <td>${patient.telephone || "N/A"}</td>
        <td>${patient.address || "N/A"}</td>
        <td>${safeDate(patient.dateOfVisit)}</td>
        <td>${patient.nextAppointment ? safeDate(patient.nextAppointment) : "-"}</td>
        <td>${patient.complaint || "N/A"}</td>
        <td>${patient.service || "N/A"}</td>
        <td>₱${(patient.debit ?? 0).toFixed(2)}</td>
        <td>₱${(patient.credit ?? 0).toFixed(2)}</td>
        <td>₱${(patient.balance ?? 0).toFixed(2)}</td>
        <td>${patient.computedStatus || patient.status || "No appointments"}</td>
        <td>${patient.visitStatus || "N/A"}</td>
        <td>${patient.patientStatus || "N/A"}</td>
        <td>
            <button class="edit-btn px-3 py-1 text-xs bg-green-600 text-white rounded hover:bg-green-700"
            onclick='openEditPatientForm(${JSON.stringify(patient)})'>
                Edit
            </button>

            <button 
            class="delete-btn px-3 py-1 text-xs bg-red-600 text-white rounded hover:bg-red-700"
            onclick="PmDeleteModal(${patient.id})">
                Delete
            </button>

        </td>
    `;

    tableBody.appendChild(row);
}

// -------------------- OPEN EDIT PATIENT FORM --------------------
function openEditPatientForm(patient) {
    currentPatientId = patient.id;

    const modal = document.getElementById("pmEditModal");
    modal.classList.remove("hidden");

    const form = document.getElementById("pmEditForm");

    // Fill fields
    form.patientName.value = patient.patientName || "";
    form.age.value = patient.age || "";
    form.occupation.value = patient.occupation || "";
    form.telephone.value = patient.telephone || "";
    form.address.value = patient.address || "";
    form.dateOfVisit.value = formatDate(patient.dateOfVisit);
    form.nextAppointment.value = formatDate(patient.nextAppointment);
    form.complaint.value = patient.complaint || "";
    form.service.value = patient.service || "";
    form.patientStatus.value = patient.patientStatus || "Active";
    form.visitStatus.value = patient.visitStatus || "Pending";
    form.debit.value = patient.debit || 0;
    form.credit.value = patient.credit || 0;
    form.balance.value = patient.balance || 0;

    const debitField = form.debit;
    const creditField = form.credit;
    const balanceField = form.balance;

    //Recalculate immediately when modal opens
    updateBalance(debitField, creditField, balanceField);

    //Auto-update when typing
    debitField.oninput = () => updateBalance(debitField, creditField, balanceField);
    creditField.oninput = () => updateBalance(debitField, creditField, balanceField);
}

// -------------------- CLOSE EDIT PATIENT MODAL --------------------
function closePmEditModal() {
    document.getElementById("pmEditModal").classList.add("hidden");
}

// -------------------- FORMAT DATE HELPER --------------------
function formatDate(date) {
    if (!date) return "";
    const d = new Date(date);
    return d.toISOString().split("T")[0];
}

// -------------------- SUBMIT EDIT PATIENT FORM --------------------
async function submitPmEditForm(event) {
    event.preventDefault();

    if (!currentPatientId) {
        console.error("No patient selected for editing.");
        return;
    }

    const form = event.target;

    const payload = {
        patientName: form.patientName.value,
        address: form.address.value,
        telephone: form.telephone.value,
        age: parseInt(form.age.value),
        occupation: form.occupation.value,
        complaint: form.complaint.value,
        dateOfVisit: form.dateOfVisit.value
            ? new Date(form.dateOfVisit.value).toISOString()
            : null,
        nextAppointment: form.nextAppointment.value
            ? new Date(form.nextAppointment.value).toISOString()
            : null,
        service: form.service.value,
        visitStatus: form.visitStatus.value,
        status: form.visitStatus.value,
        patientStatus: form.patientStatus.value,
        debit: parseFloat(form.debit.value),
        credit: parseFloat(form.credit.value),
        balance: parseFloat(form.balance.value)
    };


    try {
        const response = await fetch(`/api/Patient/${currentPatientId}/update-patient`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            closePmEditModal();
            filterCustom();
            showSuccessModal("Patient updated successfully!");
        } else {
            const errorText = await response.text();
            console.error("Failed to update:", response.status, errorText);
            showErrorModal("Failed to update patient: " + errorText);
        }

    } catch (err) {
        console.error("Error updating patient:", err);
        showErrorModal("Network error while updating patient.");
    }
}

handleSearch(
    "searchUpcoming",
    "/api/Patient/search?status=Upcoming",
    renderUpcoming,
    "/api/Patient/upcoming-appointments"
);

handleSearch(
    "searchMissed",
    "/api/Patient/search?status=Missed",
    renderMissed,
    "/api/Patient/missed-appointments"
);
