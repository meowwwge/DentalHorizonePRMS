let isSubmitting = false;

// ✅ Helpers moved to top-level so they’re usable everywhere
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

// ✅ Toggle modal for Add Patient button
function togglePatientForm() {
    const modal = document.getElementById("patientFormModal");
    if (!modal) return;
    modal.classList.toggle("hidden");
}

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

    const checkbox = document.getElementById("noNextAppointmentCheckbox");
    const nextAppointmentInput = document.getElementById("nextAppointment");

    if (checkbox && nextAppointmentInput) {
        // Apply initial state
        if (checkbox.checked) {
            nextAppointmentInput.disabled = true;
            nextAppointmentInput.classList.add("bg-gray-100", "cursor-not-allowed");
        }

        // Toggle on change
        checkbox.addEventListener("change", () => {
            if (checkbox.checked) {
                nextAppointmentInput.value = "";
                nextAppointmentInput.disabled = true;
                nextAppointmentInput.classList.add("bg-gray-100", "cursor-not-allowed");
            } else {
                nextAppointmentInput.disabled = false;
                nextAppointmentInput.classList.remove("bg-gray-100", "cursor-not-allowed");
            }
        });
    }

    // Capitalize every word for certain inputs
    const multiWordInputs = document.querySelectorAll("input.capitalize-first");
    multiWordInputs.forEach(input => {
        input.addEventListener("blur", () => {
            input.value = capitalizeWords(input.value);
        });
    });

    // Capitalize only the first letter for complaint
    const complaintInput = document.querySelector("input[name='complaint']");
    if (complaintInput) {
        complaintInput.addEventListener("blur", () => {
            complaintInput.value = capitalizeFirstLetter(complaintInput.value);
        });
    }

});

async function submitPatientForm(e) {
    e.preventDefault();
    if (isSubmitting) return;
    isSubmitting = true;

    const submitButton = e.target.querySelector('button[type="submit"]');
    if (submitButton) submitButton.disabled = true;

    const form = e.target;
    let hasError = false;

    const ageInput = form.querySelector("input[name='age']");
    const age = parseInt(ageInput.value);
    if (isNaN(age) || age <= 0) {
        showFieldError(ageInput, "Age must not be 0.");
        hasError = true;
    } else {
        clearFieldError(ageInput);
    }

    const telephoneInput = form.querySelector("input[name='telephone']");
    const telephone = telephoneInput.value.trim();
    const telephonePattern = /^\d{4}-\d{3}-\d{4}$/;
    if (!telephonePattern.test(telephone)) {
        showFieldError(telephoneInput, "The format should be NNNN-NNN-NNNN.");
        hasError = true;
    } else {
        clearFieldError(telephoneInput);
    }

    const requiredFields = ["patientName", "occupation", "address", "complaint"];
    requiredFields.forEach(name => {
        const input = form.querySelector(`input[name='${name}']`);
        if (!input.value.trim()) {
            showFieldError(input, "This field is required.");
            hasError = true;
        } else {
            clearFieldError(input);
        }
    });

    const checkbox = document.getElementById("noNextAppointmentCheckbox");
    const nextAppointmentInput = document.getElementById("nextAppointment");
    let nextAppointment = null;

    // ✅ Keep your nextAppointment logic exactly as-is
    if (!checkbox.checked) {
        const nextAppointmentRaw = nextAppointmentInput.value;
        try {
            if (nextAppointmentRaw) {
                nextAppointment = new Date(nextAppointmentRaw).toISOString();
            }
        } catch (error) {
            console.error("Error parsing nextAppointment:", error);
            showFieldError(nextAppointmentInput, "Invalid appointment date.");
            hasError = true;
        }
    }

    if (hasError) {
        isSubmitting = false;
        if (submitButton) submitButton.disabled = false;
        return;
    }

    // ✅ Recalculate balance before submit
    const debitField = document.getElementById("debit");
    const creditField = document.getElementById("credit");
    const balanceField = document.getElementById("balance");
    updateBalance(debitField, creditField, balanceField);

    const nameInput = form.querySelector("input[name='patientName']");
    const occupationInput = form.querySelector("input[name='occupation']");
    const addressInput = form.querySelector("input[name='address']");
    const complaintInput = form.querySelector("input[name='complaint']");

    nameInput.value = capitalizeWords(nameInput.value);
    occupationInput.value = capitalizeWords(occupationInput.value);
    addressInput.value = capitalizeWords(addressInput.value);
    complaintInput.value = capitalizeFirstLetter(complaintInput.value);

    const payload = {
        patientName: nameInput.value,
        age: age,
        occupation: occupationInput.value,
        telephone: telephone,
        address: addressInput.value,
        dateOfVisit: new Date(form.querySelector("input[name='dateOfVisit']").value).toISOString(),
        complaint: complaintInput.value,
        nextAppointment: nextAppointment
            ? new Date(nextAppointment).toISOString()
            : null,
        service: form.querySelector("#service").value,
        patientStatus: form.querySelector("select[name='patientStatus']").value,
        visitStatus: form.querySelector("select[name='visitStatus']").value,
        debit: parseFloat(debitField.value) || 0,
        credit: parseFloat(creditField.value) || 0,
        balance: parseFloat(balanceField.value) || 0
    };


    try {
        const response = await fetch('/api/Patient/create-patient', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            const { id } = await response.json();
            const patient = await fetch(`/api/Patient/${id}`).then(r => r.json());
            addPatientToTable(patient);
            togglePatientForm();
            showSuccessModal();
            console.log("Patient added successfully:", patient);
        } else {
            const errorText = await response.text();
            console.error("API Error:", response.status, errorText);
            showErrorModal(`Failed to add patient: ${errorText}`);
        }
    } catch (error) {
        console.error("Network or other error:", error);
        showErrorModal("Network error. Please try again.");
    } finally {
        isSubmitting = false;
        if (submitButton) submitButton.disabled = false;
    }
}

function showFieldError(input, message) {
    input.classList.add("input-error");
    let existingError = input.parentNode.querySelector(".error-message");
    if (existingError) existingError.remove();

    const error = document.createElement("div");
    error.className = "error-message";
    error.innerText = message;
    input.parentNode.appendChild(error);
}

function clearFieldError(input) {
    input.classList.remove("input-error");
    let existingError = input.parentNode.querySelector(".error-message");
    if (existingError) existingError.remove();
}

document.addEventListener("input", (e) => {
    const target = e.target;
    if (target.matches("input, select, textarea")) {
        clearFieldError(target);
    }
});
