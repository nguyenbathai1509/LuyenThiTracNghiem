document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("changePasswordForm");

    form.addEventListener("submit", function (e) {
        let isValid = true;

        const oldPass = document.getElementById("OldPassword");
        const newPass = document.getElementById("NewPassword");
        const confirmPass = document.getElementById("ConfirmPassword");

        // reset errors
        document.getElementById("OldPasswordError").textContent = "";
        document.getElementById("NewPasswordError").textContent = "";
        document.getElementById("ConfirmPasswordError").textContent = "";

        // Mật khẩu mới
        if (!newPass.value || newPass.value.length < 8 ||
            !/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(newPass.value)) {
            document.getElementById("NewPasswordError").textContent = "Mật khẩu mới phải >=8 ký tự, gồm chữ hoa, chữ thường và số.";
            isValid = false;
        }

        // Xác nhận
        if (newPass.value !== confirmPass.value) {
            document.getElementById("ConfirmPasswordError").textContent = "Xác nhận mật khẩu không khớp!";
            isValid = false;
        }

        if (!isValid) e.preventDefault();
    });
});
