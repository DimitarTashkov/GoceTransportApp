document.getElementById('showPasswordToggle').addEventListener('change', function () {
    let passwordFields = document.querySelectorAll('.password-field');
    passwordFields.forEach(field => {
        field.type = this.checked ? 'text' : 'password';
    });
});