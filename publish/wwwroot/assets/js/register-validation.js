document.addEventListener("DOMContentLoaded", function() {
    const form = document.getElementById("registerForm");

    form.addEventListener("submit", function(e) {
        e.preventDefault(); // Ngăn form submit tạm thời

        const fields = [
            { id: "FullName" },
            { id: "Username" },
            { id: "PasswordHash" },
            { id: "PhoneNumber" },
            { id: "Email" },
            { id: "BirthDate" },
            { id: "Gender" }
        ];


        let isValid = true;

        fields.forEach(field => {
            const input = document.getElementById(field.id);
            const errorDiv = document.getElementById(field.id + "Error");
            const value = input.value.trim();
            errorDiv.textContent = "";
            input.classList.remove("is-invalid", "is-valid");

            switch(field.id) {
                case "FullName":
                    if (!value) {
                        setError(input, errorDiv, "Họ và tên không được để trống");
                        isValid = false;
                    } else if (value.length < 3) {
                        setError(input, errorDiv, "Họ và tên phải có ít nhất 3 ký tự");
                        isValid = false;
                    }
                    break;

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

                case "PhoneNumber":
                    if (!value) {
                        setError(input, errorDiv, "Số điện thoại không được để trống");
                        isValid = false;
                    } else if (!/^0\d{9}$/.test(value)) {
                        setError(input, errorDiv, "Số điện thoại phải gồm 10 số và bắt đầu bằng 0");
                        isValid = false;
                    }
                    break;

                case "Email":
                    if (!value) {
                        setError(input, errorDiv, "Email không được để trống");
                        isValid = false;
                    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                        setError(input, errorDiv, "Email không hợp lệ");
                        isValid = false;
                    }
                    break;

                case "BirthDate":
                    if (!value) {
                        setError(input, errorDiv, "Ngày sinh không được để trống");
                        isValid = false;
                    } else {
                        const birth = new Date(value);
                        const today = new Date();
                        if (birth > today) {
                            setError(input, errorDiv, "Ngày sinh không thể ở tương lai");
                            isValid = false;
                        }
                    }
                    break;

                case "Gender":
                    if (!value){
                        setError(input, errorDiv, "Giới tính không được để trống");
                        isValid = false;
                    }else {
                        input.classList.add("is-valid");
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

