document.addEventListener("DOMContentLoaded", function () {
    const deleteBtn = document.getElementById("deleteBtn");
    if (deleteBtn) {
        const examId = deleteBtn.dataset.examid;

        deleteBtn.addEventListener("click", function () {
            Swal.fire({
                title: "Xóa đề thi?",
                text: "Bạn có chắc chắn muốn xóa đề thi này? Hành động này không thể hoàn tác.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#3085d6",
                confirmButtonText: "Xóa đề thi",
                cancelButtonText: "Hủy"
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch("/Admin/Exam/Delete", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({ id: examId })
                    })
                    .then(response => {
                        if (response.ok) {
                            Swal.fire("Đã xóa!", "đề thi của bạn đã được xóa.", "success")
                                .then(() => window.location.href = "/Admin/Exam");
                        } else {
                            Swal.fire("Lỗi!", "Không thể xóa đề thi. Vui lòng thử lại.", "error");
                        }
                    })
                    .catch(() => {
                        Swal.fire("Lỗi!", "Đã xảy ra lỗi khi xóa đề thi.", "error");
                    });
                }
            });
        });
    }
});
