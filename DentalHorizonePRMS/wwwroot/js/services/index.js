let isSubmitting = false;

// Modal toggle
function togglePatientForm() {
    const modal = document.getElementById("patientFormModal");
    if (!modal) return;
    modal.classList.toggle("hidden");
}

// Capitalization helpers
function capitalizeWords(text) {
    return text
        .toLowerCase()
        .split(" ")
        .filter(word => word.trim() !== "")
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(" ");
}

function capitalizeFirstLetter(text) {
    if (!text) return "";
    return text.charAt(0).toUpperCase() + text.slice(1);
}

// Finance helpers
function updateDebit(serviceField, debitField, creditField, balanceField, servicePrices) {
    const selectedService = serviceField.value;
    const price = servicePrices[selectedService] || 0;
    debitField.value = price;
    updateBalance(debitField, creditField, balanceField);
}

function updateBalance(debitField, creditField, balanceField) {
    const debit = parseFloat(debitField.value) || 0;
    const credit = parseFloat(creditField.value) || 0;
    balanceField.value = debit - credit;
}


document.addEventListener("DOMContentLoaded", () => {
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

    const serviceField = document.getElementById("service");
    const debitField = document.getElementById("debit");
    const creditField = document.getElementById("credit");
    const balanceField = document.getElementById("balance");

    if (serviceField && debitField && creditField && balanceField) {
        serviceField.addEventListener("change", () =>
            updateDebit(serviceField, debitField, creditField, balanceField, servicePrices)
        );

        creditField.addEventListener("input", () =>
            updateBalance(debitField, creditField, balanceField)
        );
    }

    const noNextAppointmentCheckbox = document.getElementById("noNextAppointmentCheckbox");
    const nextAppointmentInput = document.getElementById("nextAppointment");
    if (noNextAppointmentCheckbox && nextAppointmentInput) {
        noNextAppointmentCheckbox.addEventListener("change", () => {
            nextAppointmentInput.disabled = noNextAppointmentCheckbox.checked;
            if (noNextAppointmentCheckbox.checked) nextAppointmentInput.value = "";
        });
    }
});

// Submit patient form
async function submitPatientForm(e) {
    e.preventDefault();
    if (isSubmitting) return;
    isSubmitting = true;

    const form = e.target;
    let hasError = false;

    // Validations
    const ageInput = form.querySelector("input[name='age']");
    const age = parseInt(ageInput.value);
    if (!age || age <= 0) {
        showFieldError(ageInput, "Age must be greater than 0.");
        hasError = true;
    } else {
        clearFieldError(ageInput);
    }

    const telephoneInput = form.querySelector("input[name='telephone']");
    const telephonePattern = /^\d{4}-\d{3}-\d{4}$/;
    if (!telephonePattern.test(telephoneInput.value)) {
        showFieldError(telephoneInput, "Format must be NNNN-NNN-NNNN.");
        hasError = true;
    } else {
        clearFieldError(telephoneInput);
    }

    if (hasError) {
        isSubmitting = false;
        return;
    }

    const payload = {
        patientName: capitalizeWords(form.patientName.value),
        age,
        occupation: capitalizeWords(form.occupation.value),
        telephone: telephoneInput.value,
        address: capitalizeWords(form.address.value),
        dateOfVisit: new Date(form.dateOfVisit.value).toISOString(),
        complaint: capitalizeFirstLetter(form.complaint.value),
        nextAppointment: form.nextAppointment.value
            ? new Date(form.nextAppointment.value).toISOString()
            : null,
        service: document.getElementById("service").value,
        patientStatus: form.patientStatus.value,
        visitStatus: form.visitStatus.value,
        debit: parseFloat(document.getElementById("debit").value) || 0,
        credit: parseFloat(document.getElementById("credit").value) || 0,
        balance: parseFloat(document.getElementById("balance").value) || 0
    };

    try {
        const response = await fetch("/api/Patient/create-patient", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error("Failed to add patient.");

        form.reset();
        togglePatientForm();
        showSuccessModal("Patient added successfully!");

        setTimeout(() => {
            window.location.href = "/PatientManagement"; // Redirect to PM page
        }, 1000); // small delay to show success modal

    } catch (error) {
        console.error(error);
        showErrorModal("Failed to add patient.");
    } finally {
        isSubmitting = false;
    }
}

// Field errors
function showFieldError(input, message) {
    input.classList.add("input-error");
    const error = document.createElement("div");
    error.className = "error-message";
    error.innerText = message;
    input.parentNode.appendChild(error);
}

function clearFieldError(input) {
    input.classList.remove("input-error");
    const error = input.parentNode.querySelector(".error-message");
    if (error) error.remove();
}