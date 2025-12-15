async function loadArchivedPatients() {
    try {
        const response = await fetch("/api/ArchivedPatients/archived-patients");
        if (!response.ok) throw new Error("Failed to load archived patients.");
        const data = await response.json();
        renderArchivedPatients(data);
    } catch (err) {
        console.error(err);
        alert("Failed to load archived patients.");
    }
}

function renderArchivedPatients(list) {
    let tbody = document.getElementById("archivedPatientsTable").querySelector("tbody");
    tbody.innerHTML = "";

    list.forEach(p => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${p.patientName}</td>
            <td>${p.age}</td>
            <td>${p.occupation}</td>
            <td>${p.telephone}</td>
            <td>${p.address}</td>
            <td>${p.status}</td>
            <td>${p.patientStatus}</td>
            <td>
                <button onclick="restorePatient(${p.id})" class="text-green-600">Restore</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

async function restorePatient(id) {
    try {
        const response = await fetch(`/api/ArchivedPatients/${id}/restore`, {
            method: "PUT"
        });

        if (response.ok) {
            showSuccessModal("Patient restored successfully!");
          

            setTimeout(() => {
                window.location.href = "/PatientManagement"; // adjust route if needed
            }, 2000);
        } else {
            const errorText = await response.text();
            showErrorModal("Failed to restore patient: " + errorText);
        }
    } catch (err) {
        console.error("Restore error:", err);
        showErrorModal("Network error while restoring patient.");
    }
}


document.addEventListener("DOMContentLoaded", loadArchivedPatients);
