document.addEventListener("DOMContentLoaded", () => {
    setDefaultMonthYear();
    document.getElementById("generateReportBtn").addEventListener("click", loadReport);
});

// Set current month/year as default
function setDefaultMonthYear() {
    const now = new Date();
    document.getElementById("reportMonth").value = now.getMonth() + 1;

    const yearSelect = document.getElementById("reportYear");
    if (![...yearSelect.options].some(opt => opt.value == now.getFullYear())) {
        const opt = document.createElement("option");
        opt.value = now.getFullYear();
        opt.textContent = now.getFullYear();
        yearSelect.prepend(opt);
    }
    yearSelect.value = now.getFullYear();
}

// Load monthly report
async function loadReport() {
    const month = document.getElementById("reportMonth").value;
    const year = document.getElementById("reportYear").value;

    try {
        const res = await fetch(`/api/Reports/monthly-patients?month=${month}&year=${year}`);
        const data = await res.json();
        renderReport(data, month, year);

        window.print();
    } catch (err) {
        console.error("Failed to load report:", err);
    }
}

function renderReport(patients, month, year) {
    const title = document.getElementById("reportTitle");
    const tbody = document.getElementById("reportTableBody");

    title.textContent = `Dental Horizone – Patient Report (${month}/${year})`;
    tbody.innerHTML = "";

    if (!patients || patients.length === 0) {
        tbody.innerHTML = `<tr><td colspan="11" class="px-4 py-2 text-center text-gray-500">No records found</td></tr>`;
        return;
    }

    patients.forEach(p => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td class="px-4 py-2">${p.patientName}</td>
            <td class="px-4 py-2">${p.telephone}</td>
            <td class="px-4 py-2">${p.address}</td>
            <td class="px-4 py-2">${formatDate(p.dateOfVisit)}</td>
            <td class="px-4 py-2">${p.service}</td>
            <td class="px-4 py-2 font-semibold ${statusColor(p.visitStatus)}">${p.visitStatus}</td>
            <td class="px-4 py-2">₱${p.debit}</td>
            <td class="px-4 py-2">₱${p.credit}</td>
            <td class="px-4 py-2">₱${p.debit - p.credit}</td>
        `;
        tbody.appendChild(tr);
    });
}

function formatDate(dateStr) {
    const date = new Date(dateStr);
    return date.toLocaleDateString("en-PH", { year: "numeric", month: "short", day: "numeric" });
}

function statusColor(status) {
    switch (status) {
        case "Completed": return "text-green-600";
        case "Pending": return "text-red-600";
        default: return "";
    }
}

