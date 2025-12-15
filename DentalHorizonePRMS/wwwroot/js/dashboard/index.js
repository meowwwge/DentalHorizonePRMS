// =====================================
// DASHBOARD PAGE JS
// =====================================

document.addEventListener("DOMContentLoaded", loadDashboardTotals);

// -------------------------------------
// LOAD DASHBOARD TOTALS
// -------------------------------------
async function loadDashboardTotals() {
    try {
        const res = await fetch("/api/patient/dashboard-totals");
        const data = await res.json();

        document.getElementById("totalPatients").textContent =
            data.totalPatients ?? 0;

        document.getElementById("upcomingAppointments").textContent =
            data.upcomingAppointments ?? 0;

        document.getElementById("missedAppointments").textContent =
            data.missedAppointments ?? 0;

    } catch (err) {
        console.error("Failed to load dashboard totals:", err);
    }
}
