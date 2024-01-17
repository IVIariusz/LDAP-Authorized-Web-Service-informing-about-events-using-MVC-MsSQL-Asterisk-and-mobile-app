
document.addEventListener('DOMContentLoaded', function () {
    var emailInput = document.getElementById('emailInput');
    var passwordInput = document.getElementById('passwordInput');

    emailInput.addEventListener('focus', function() {
        var label = this.previousElementSibling;
        label.textContent = this.placeholder;
    });

    passwordInput.addEventListener('focus', function() {
        var label = this.previousElementSibling;
        label.textContent = this.placeholder;
    });

    emailInput.addEventListener('blur', function() {
        this.previousElementSibling.textContent = '';
    });

    passwordInput.addEventListener('blur', function() {
        this.previousElementSibling.textContent = '';
    });
});