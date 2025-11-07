document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("profileForm");
    const editBtn = document.getElementById("editBtn");
    const saveBtn = document.getElementById("saveBtn");
    const cancelBtn = document.getElementById("cancelBtn");
    const inputs = form.querySelectorAll("input");

    const originalValues = {};
    inputs.forEach(input => {
        originalValues[input.id] = input.value;
    });

    editBtn.addEventListener("click", function () {
        inputs.forEach(input => {
            if (input.id !== "Username") {
                input.removeAttribute("disabled");
            }
        });
        editBtn.classList.add("d-none");
        saveBtn.classList.remove("d-none");
        cancelBtn.classList.remove("d-none");
    });

    cancelBtn.addEventListener("click", function () {
        inputs.forEach(input => {
            input.value = originalValues[input.id];
            input.setAttribute("disabled", true);
            input.classList.remove("is-valid", "is-invalid");
        });
        saveBtn.classList.add("d-none");
        cancelBtn.classList.add("d-none");
        editBtn.classList.remove("d-none");
    });

    form.addEventListener("submit", function (e) {
        let isValid = true;

        const validators = {
            FullName: value => {
                if (!value) return "Họ và tên không được để trống";
                if (value.length < 3) return "Họ và tên phải có ít nhất 3 ký tự";
                return "";
            },
            BirthDate: value => {
                if (!value) return "Ngày sinh không được để trống";
                const birth = new Date(value);
                const today = new Date();
                if (birth > today) return "Ngày sinh không thể ở tương lai";
                return "";
            },
            PhoneNumber: value => {
                if (!value) return "Số điện thoại không được để trống";
                if (!/^0\d{9}$/.test(value)) return "Số điện thoại phải gồm 10 số và bắt đầu bằng 0";
                return "";
            },
            Email: value => {
                if (!value) return "Email không được để trống";
                if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) return "Email không hợp lệ";
                return "";
            }
        };

        inputs.forEach(input => {
            const errorDiv = document.getElementById(input.id + "Error");
            if (!errorDiv) return;

            const message = validators[input.id]?.(input.value.trim()) || "";
            errorDiv.textContent = message;
            input.classList.remove("is-invalid", "is-valid");

            if (message) {
                input.classList.add("is-invalid");
                isValid = false;
            } else {
                input.classList.add("is-valid");
            }
        });

        if (!isValid) {
            e.preventDefault();
        }
    });

    // --- Xóa tài khoản ---
    const deleteBtn = document.getElementById("deleteBtn");
    if (deleteBtn) {
        const userId = deleteBtn.dataset.userid; // lấy từ data-userid

        deleteBtn.addEventListener("click", function () {
            Swal.fire({
                title: "Xóa tài khoản?",
                text: "Bạn có chắc chắn muốn xóa tài khoản này? Hành động này không thể hoàn tác.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#3085d6",
                confirmButtonText: "Xóa tài khoản",
                cancelButtonText: "Hủy"
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch("/Profile/DeleteAccount", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({ id: userId })
                    })
                    .then(response => {
                        if (response.ok) {
                            Swal.fire("Đã xóa!", "Tài khoản của bạn đã được xóa.", "success")
                                .then(() => window.location.href = "/Home/Index");
                        } else {
                            Swal.fire("Lỗi!", "Không thể xóa tài khoản. Vui lòng thử lại.", "error");
                        }
                    })
                    .catch(() => {
                        Swal.fire("Lỗi!", "Đã xảy ra lỗi khi xóa tài khoản.", "error");
                    });
                }
            });
        });
    }
});
