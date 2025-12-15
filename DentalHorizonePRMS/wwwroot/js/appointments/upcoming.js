// =====================================
// UPCOMING APPOINTMENTS PAGE JS
// =====================================

let upcomingAppointments = [];
let selectedAppointmentId = null;

// -------------------------------------
// INIT
// -------------------------------------
document.addEventListener("DOMContentLoaded", () => {
    loadUpcomingAppointments();

    const searchEl = document.getElementById("searchUpcoming");
    if (searchEl) searchEl.addEventListener("input", filterUpcoming);

    const cancelRescheduleBtn = document.getElementById("cancelReschedule");
    if (cancelRescheduleBtn) cancelRescheduleBtn.onclick = closeRescheduleModal;

    const confirmRescheduleBtn = document.getElementById("confirmReschedule");
    if (confirmRescheduleBtn) confirmRescheduleBtn.onclick = confirmReschedule;

    const closeCancelBtn = document.getElementById("closeCancel");
    if (closeCancelBtn) closeCancelBtn.onclick = closeCancelModal;

    const confirmCancelBtn = document.getElementById("confirmCancel");
    if (confirmCancelBtn) confirmCancelBtn.onclick = confirmCancel;
});

// -------------------------------------
// LOAD UPCOMING APPOINTMENTS
// -------------------------------------
async function loadUpcomingAppointments() {
    try {
        const res = await fetch("/api/Patient/upcoming-appointments");
        upcomingAppointments = await res.json();
        renderUpcoming(upcomingAppointments);
    } catch {
        showErrorModal("Failed to load upcoming appointments.");
    }
}

// -------------------------------------
// RENDER TABLE
// -------------------------------------
function renderUpcoming(list) {
    const tbody = document.getElementById("upcomingTable");
    tbody.innerHTML = "";

    list.forEach(a => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td class="px-4 py-2">${a.patientName}</td>
            <td class="px-4 py-2">${a.age}</td>
            <td class="px-4 py-2">${a.telephone}</td>
            <td class="px-4 py-2">${formatDate(a.nextAppointment)}</td>
            <td class="px-4 py-2">${a.service}</td>
            <td class="px-4 py-2">${a.complaint}</td>
            <td class="px-4 py-2 text-green-600 font-semibold">Upcoming</td>
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
function filterUpcoming() {
    const term = document.getElementById("searchUpcoming").value.toLowerCase();
    const filtered = upcomingAppointments.filter(a =>
        a.patientName.toLowerCase().includes(term)
    );
    renderUpcoming(filtered);
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

async function confirmReschedule() {
    const newDate = document.getElementById("newDate").value;
    if (!newDate || !selectedAppointmentId) return;

    const iso = `${newDate}T00:00:00`; // ISO format

    console.log("[Reschedule] Sending payload:", iso, "for patientId:", selectedAppointmentId);

    try {
        const res = await fetch(`/api/Patient/${selectedAppointmentId}/reschedule`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(iso) // ✅ send as JSON string
        });

        console.log("[Reschedule] Response status:", res.status);
        const text = await res.text();
        console.log("[Reschedule] Response body:", text);

        if (!res.ok) throw new Error(text);

        closeRescheduleModal();
        showSuccessModal("Appointment rescheduled.");
        loadUpcomingAppointments();
    } catch (e) {
        console.error("[Reschedule] error:", e);
        showErrorModal("Failed to reschedule appointment.");
    }
}



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

async function confirmCancel() {
    if (!selectedAppointmentId) return;

    console.log("[Cancel] Sending cancel for patientId:", selectedAppointmentId);

    try {
        const res = await fetch(`/api/Patient/${selectedAppointmentId}/cancel-appointment`, {
            method: "PUT"
        });

        console.log("[Cancel] Response status:", res.status);

        const text = await res.text();
        console.log("[Cancel] Response body:", text);

        if (!res.ok) throw new Error(text);

        closeCancelModal();
        showSuccessModal("Appointment cancelled.");
        loadUpcomingAppointments();
    } catch (e) {
        console.error("[Cancel] error:", e);
        showErrorModal("Failed to cancel appointment.");
    }
}

// -------------------------------------
// HELPERS
// -------------------------------------
function formatDate(date) {
    if (!date) return "";
    return new Date(date).toLocaleDateString();
}
