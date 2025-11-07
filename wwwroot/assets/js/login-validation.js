document.addEventListener("DOMContentLoaded", function() {
    const form = document.getElementById("loginForm");

    form.addEventListener("submit", function(e) {
        e.preventDefault();

        const fields = [
            { id: "Username" },
            { id: "PasswordHash" }
        ];

        let isValid = true;

        fields.forEach(field => {
            const input = document.getElementById(field.id);
            const errorDiv = document.getElementById(field.id + "Error");
            const value = input.value.trim();
            errorDiv.textContent = "";
            input.classList.remove("is-invalid", "is-valid");

            switch(field.id) {
                case "Username":
                    if (!value) {
                        setError(input, errorDiv, "Tên đăng nhập không được để trống");
                        isValid = false;
                    } else if (value.length < 6) {
                        setError(input, errorDiv, "Tên đăng nhập phải có ít nhất 6 ký tự");
                        isValid = false;
                    } else if (!/^[a-zA-Z0-9_]+$/.test(value)) {
                        setError(input, errorDiv, "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới");
                        isValid = false;
                    }
                    break;

                case "PasswordHash":
                    if (!value) {
                        setError(input, errorDiv, "Mật khẩu không được để trống");
                        isValid = false;
                    } else if (value.length < 8) {
                        setError(input, errorDiv, "Mật khẩu phải có ít nhất 8 ký tự");
                        isValid = false;
                    } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(value)) {
                        setError(input, errorDiv, "Mật khẩu phải chứa chữ hoa, chữ thường và số");
                        isValid = false;
                    }
                    break;
            }

            if (!errorDiv.textContent) {
                input.classList.add("is-valid");
            }
        });

        if (isValid) {
            form.submit();
        }
    });

    function setError(input, errorDiv, message) {
        errorDiv.textContent = message;
        input.classList.add("is-invalid");
    }
});