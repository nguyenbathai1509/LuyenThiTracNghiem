document.addEventListener("DOMContentLoaded", function () {
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
                    fetch("/Admin/User/Delete", {
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
                                .then(() => window.location.href = "/Admin/User");
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
