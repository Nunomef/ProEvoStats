window.onload = function () {
    var checkboxes = document.querySelectorAll('input[type=checkbox]');
    checkboxes.forEach(checkbox => {
        var row = checkbox.parentElement.parentElement;
        var inputs = row.querySelectorAll('input[type=number], select');
        var allInputsHaveValues = Array.from(inputs).every(input => input.value.trim() !== '');
        checkbox.checked = allInputsHaveValues;
        toggleInput(checkbox);
    });
}

function toggleInput(checkbox) {
    var row = checkbox.parentElement.parentElement;
    var inputs = row.querySelectorAll('input[type=number], select');
    inputs.forEach(input => input.disabled = checkbox.checked);
}

$('.bootstrap-toggle').bootstrapToggle();