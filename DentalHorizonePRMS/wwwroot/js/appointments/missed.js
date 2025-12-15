// =====================================
// MISSED APPOINTMENTS PAGE JS
// =====================================

let missedAppointments = [];
let selectedAppointmentId = null;

// -------------------------------------
// LOAD MISSED APPOINTMENTS
// -------------------------------------
document.addEventListener("DOMContentLoaded", () => {
    loadMissedAppointments();

    document
        .getElementById("searchMissed")
        .addEventListener("input", filterMissed);
});

async function loadMissedAppointments() {
    try {
        const res = await fetch("/api/patient/missed-appointments");
        missedAppointments = await res.json();
        renderMissed(missedAppointments);
    } catch {
        showErrorModal("Failed to load missed appointments.");
    }
}

// -------------------------------------
// RENDER TABLE
// -------------------------------------
function renderMissed(list) {
    const tbody = document.getElementById("missedTable");
    tbody.innerHTML = "";

    list.forEach(a => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td class="px-4 py-2">${a.patientName}</td>
            <td class="px-4 py-2">${a.age}</td>
            <td class="px-4 py-2">${a.telephone}</td>
            <td class="px-4 py-2">${formatDate(a.originalAppointmentDate)}</td>
            <td class="px-4 py-2">${a.service}</td>
            <td class="px-4 py-2">${a.complaint}</td>
            <td class="px-4 py-2 text-red-600 font-semibold">Missed</td>
            <td class="px-4 py-2 space-x-2">
                <button onclick="openRescheduleModal(${a.id})"
                        class="text-blue-600 hover:underline">
                    Reschedule
                </button>
                <button onclick="openCancelModal(${a.id})"
                        class="text-red-600 hover:underline">
                    Cancel
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// -------------------------------------
// SEARCH FILTER
// -------------------------------------
function filterMissed() {
    const term = document
        .getElementById("searchMissed")
        .value
        .toLowerCase();

    const filtered = missedAppointments.filter(a =>
        a.patientName.toLowerCase().includes(term)
    );

    renderMissed(filtered);
}

// -------------------------------------
// RESCHEDULE MODAL
// -------------------------------------
function openRescheduleModal(id) {
    selectedAppointmentId = id;
    document.getElementById("newDate").value = "";
    document.getElementById("rescheduleModal").classList.remove("hidden");
}

function closeRescheduleModal() {
    document.getElementById("rescheduleModal").classList.add("hidden");
}

document.getElementById("cancelReschedule").onclick = closeRescheduleModal;

document.getElementById("confirmReschedule").onclick = async () => {
    const newDate = document.getElementById("newDate").value;
    if (!newDate) return;

    try {
        const res = await fetch(`/api/patient/reschedule/${selectedAppointmentId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ newDate })
        });

        if (!res.ok) throw new Error();

        closeRescheduleModal();
        showSuccessModal("Appointment rescheduled.");
        loadMissedAppointments();
    } catch {
        showErrorModal("Failed to reschedule appointment.");
    }
};

// -------------------------------------
// CANCEL MODAL
// -------------------------------------
function openCancelModal(id) {
    selectedAppointmentId = id;
    document.getElementById("cancelModal").classList.remove("hidden");
}

function closeCancelModal() {
    document.getElementById("cancelModal").classList.add("hidden");
}

document.getElementById("closeCancel").onclick = closeCancelModal;

document.getElementById("confirmCancel").onclick = async () => {
    try {
        const res = await fetch(`/api/patient/cancel-appointment/${selectedAppointmentId}`, {
            method: "DELETE"
        });

        if (!res.ok) throw new Error();

        closeCancelModal();
        showSuccessModal("Appointment cancelled.");
        loadMissedAppointments();
    } catch {
        showErrorModal("Failed to cancel appointment.");
    }
};

// -------------------------------------
// HELPERS
// -------------------------------------
function formatDate(date) {
    return new Date(date).toLocaleDateString();
}
