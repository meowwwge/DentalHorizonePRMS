// Global variables
let currentPatientId = null;

// Toggle patient form modal
function togglePatientForm() {
    const modal = document.getElementById('patientFormModal');
    modal.classList.toggle('hidden');

    // Reset form when opening
    if (!modal.classList.contains('hidden')) {
        document.querySelector("#patientFormModal form").reset();
    }
}

// Success / Error Modals
function showSuccessModal(message) {
    const modal = document.getElementById("successModal");
    const msg = modal.querySelector(".modal-message");
    if (msg) msg.textContent = message;
    modal.classList.remove("hidden");
}

function closeSuccessModal() {
    const modal = document.getElementById("successModal");
    modal.classList.add("hidden");
}

function showErrorModal(message) {
    const modal = document.getElementById("errorModal");
    const msg = modal.querySelector(".modal-message");
    if (msg) msg.textContent = message;
    modal.classList.remove("hidden");
}

function closeErrorModal() {
    const modal = document.getElementById("errorModal");
    modal.classList.add("hidden");
}
