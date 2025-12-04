document.addEventListener("DOMContentLoaded", function () {
    const deleteBtn = document.getElementById("deleteBtn");
    if (deleteBtn) {
        const subjectId = deleteBtn.dataset.subjectid;

        deleteBtn.addEventListener("click", function () {
            Swal.fire({
                title: "Xóa môn học?",
                text: "Bạn có chắc chắn muốn xóa môn học này? Hành động này không thể hoàn tác.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#3085d6",
                confirmButtonText: "Xóa môn học",
                cancelButtonText: "Hủy"
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch("/Admin/Subject/Delete", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({ id: subjectId })
                    })
                    .then(response => {
                        if (response.ok) {
                            Swal.fire("Đã xóa!", "môn học của bạn đã được xóa.", "success")
                                .then(() => window.location.href = "/Admin/Subject");
                        } else {
                            Swal.fire("Lỗi!", "Không thể xóa môn học. Vui lòng thử lại.", "error");
                        }
                    })
                    .catch(() => {
                        Swal.fire("Lỗi!", "Đã xảy ra lỗi khi xóa môn học.", "error");
                    });
                }
            });
        });
    }
});
