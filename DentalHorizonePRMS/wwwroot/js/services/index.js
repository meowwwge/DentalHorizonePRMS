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

    //Trigger real-time validation manually
    debitField.dispatchEvent(new Event("input"));
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

    const telephoneInput = document.querySelector("input[name='telephone']");
    if (telephoneInput) {
        telephoneInput.addEventListener("input", () => {
            let digits = telephoneInput.value.replace(/\D/g, "");

            if (digits.length > 4 && digits.length <= 7)
                telephoneInput.value = digits.slice(0,4) + "-" + digits.slice(4);
            else if (digits.length > 7)
                telephoneInput.value = digits.slice(0,4) + "-" + digits.slice(4,7) + "-" + digits.slice(7,11);
            else
                telephoneInput.value = digits;
        });
    }

    attachRealtimeValidation();
    attachNextAppointmentRealtimeValidation();
});

//Real-time validation + green borders + shake animation
function attachRealtimeValidation() {
    const form = document.querySelector("#patientFormModal form");

    const validators = {
        patientName: value => value.trim() !== "",
        age: value => parseInt(value) > 0,
        occupation: value => value.trim() !== "",
        telephone: value => /^\d{4}-\d{3}-\d{4}$/.test(value),
        address: value => value.trim() !== "",
        dateOfVisit: value => value !== "",
        complaint: value => value.trim() !== "",
        service: value => value !== "Choose a Service",
        debit: value => parseFloat(value) >= 0,
        credit: value => parseFloat(value) >= 0
    };

    Object.keys(validators).forEach(field => {
        const input =
            form.querySelector(`[name='${field}']`) ||
            document.getElementById(field);

        if (!input) return;

        input.addEventListener("input", () => {
            const isValid = validators[field](input.value);

            if (isValid) {
                clearFieldError(input);
                input.classList.add("input-valid");
            } else {
                input.classList.remove("input-valid");
            }
        });
    });
}

function attachNextAppointmentRealtimeValidation() {
    const form = document.querySelector("#patientFormModal form");
    if (!form) return;

    const nextAppointmentInput = form.querySelector("input[name='nextAppointment']");
    const noNextAppointmentCheckbox = document.getElementById("noNextAppointmentCheckbox");

    if (!nextAppointmentInput || !noNextAppointmentCheckbox) return;

    // When user types/changes the date
    nextAppointmentInput.addEventListener("input", () => {
        if (nextAppointmentInput.value) {
            clearFieldError(nextAppointmentInput);
            nextAppointmentInput.classList.add("input-valid");
        } else {
            nextAppointmentInput.classList.remove("input-valid");
        }
    });

    // When checkbox is toggled
    noNextAppointmentCheckbox.addEventListener("change", () => {
        if (noNextAppointmentCheckbox.checked) {
            // Checkbox checked → field is effectively "not required"
            clearFieldError(nextAppointmentInput);
            nextAppointmentInput.classList.remove("input-error");
            nextAppointmentInput.classList.remove("input-valid");
        } else {
            // Checkbox unchecked → if empty, show error when submitting, not instantly
            if (!nextAppointmentInput.value) {
                // Optionally, you can show or not show error here.
                // For softer UX, we only validate on submit.
            }
        }
    });
}

// Submit patient form
async function submitPatientForm(e) {
    e.preventDefault();
    if (isSubmitting) return;
    isSubmitting = true;

    const form = e.target;
    let hasError = false;

    //Clear previous errors
    clearAllErrors(form);

    //Name
    const nameInput = form.querySelector("input[name='patientName']");
    if (!nameInput.value.trim()) {
        showFieldError(nameInput, "Name is required.");
        hasError = true;
    }

    //Age
    const ageInput = form.querySelector("input[name='age']");
    const age = parseInt(ageInput.value);
    if (!age || age <= 0) {
        showFieldError(ageInput, "Age must be greater than 0.");
        hasError = true;
    }

    //Occupation
    const occupationInput = form.querySelector("input[name='occupation']");
    if (!occupationInput.value.trim()) {
        showFieldError(occupationInput, "Occupation is required.");
        hasError = true;
    }

    //Telephone
    const telephoneInput = form.querySelector("input[name='telephone']");
    const telephonePattern = /^\d{4}-\d{3}-\d{4}$/;
    if (!telephonePattern.test(telephoneInput.value)) {
        showFieldError(telephoneInput, "Format must be NNNN-NNN-NNNN.");
        hasError = true;
    }

    //Address
    const addressInput = form.querySelector("input[name='address']");
    if (!addressInput.value.trim()) {
        showFieldError(addressInput, "Address is required.");
        hasError = true;
    }

    //Date of Visit
    const dateOfVisitInput = form.querySelector("input[name='dateOfVisit']");
    if (!dateOfVisitInput.value) {
        showFieldError(dateOfVisitInput, "Date of visit is required.");
        hasError = true;
    }

    //Complaint
    const complaintInput = form.querySelector("input[name='complaint']");
    if (!complaintInput.value.trim()) {
        showFieldError(complaintInput, "Complaint is required.");
        hasError = true;
    }

    //Service
    const serviceField = document.getElementById("service");
    if (serviceField.value === "Choose a Service") {
        showFieldError(serviceField, "Please select a service.");
        hasError = true;
    }

    //Debit
    const debitField = document.getElementById("debit");
    if (debitField.value === "" || parseFloat(debitField.value) < 0) {
        showFieldError(debitField, "Debit must be a positive number.");
        hasError = true;
    }

    //Credit
    const creditField = document.getElementById("credit");
    if (creditField.value === "" || parseFloat(creditField.value) < 0) {
        showFieldError(creditField, "Credit must be a positive number.");
        hasError = true;
    }

    //Next Appointment (only if checkbox is unchecked)
    const noNextAppointmentCheckbox = document.getElementById("noNextAppointmentCheckbox");
    const nextAppointmentInput = form.querySelector("input[name='nextAppointment']");
    if (!noNextAppointmentCheckbox.checked && !nextAppointmentInput.value) {
        showFieldError(nextAppointmentInput, "Next appointment is required unless checkbox is checked.");
        hasError = true;
    }

    //Stop submission if errors exist
    if (hasError) {
        isSubmitting = false;
        return;
    }

    //Build payload (your original logic)
    const payload = {
        patientName: capitalizeWords(form.patientName.value),
        age,
        occupation: capitalizeWords(form.occupation.value),
        telephone: telephoneInput.value,
        address: capitalizeWords(form.address.value),
        dateOfVisit: new Date(form.dateOfVisit.value).toISOString(),
        complaint: capitalizeFirstLetter(form.complaint.value),
        nextAppointment: noNextAppointmentCheckbox.checked
            ? null
            : new Date(form.nextAppointment.value).toISOString(),
        service: serviceField.value,
        patientStatus: form.patientStatus.value,
        visitStatus: form.visitStatus.value,
        debit: parseFloat(debitField.value) || 0,
        credit: parseFloat(creditField.value) || 0,
        balance: parseFloat(document.getElementById("balance").value) || 0
    };

    try {
        //Send to backend
        const response = await fetch("/api/Patient/create-patient", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error("Failed to add patient.");

        //Success
        form.reset();
        togglePatientForm();
        showSuccessModal("Patient added successfully!");

        //Redirect after short delay
        setTimeout(() => {
            window.location.href = "/PatientManagement";
        }, 1000);

    } catch (error) {
        console.error(error);
        showErrorModal("Failed to add patient.");
    } finally {
        isSubmitting = false;
    }
}

// Field errors
function showFieldError(input, message) {
    clearFieldError(input);

    input.classList.add("input-error");
    input.classList.remove("input-valid");

    //Shake animation
    input.classList.add("shake");
    setTimeout(() => input.classList.remove("shake"), 300);

    const error = document.createElement("div");
    error.className = "error-message text-red-600 text-xs mt-1";
    error.innerText = message;

    input.parentNode.appendChild(error);
}

function clearFieldError(input) {
    input.classList.remove("input-error");
    const error = input.parentNode.querySelector(".error-message");
    if (error) error.remove();
}

function clearAllErrors(form) {
    form.querySelectorAll(".error-message").forEach(e => e.remove());
    form.querySelectorAll(".input-error").forEach(i => i.classList.remove("input-error"));
}