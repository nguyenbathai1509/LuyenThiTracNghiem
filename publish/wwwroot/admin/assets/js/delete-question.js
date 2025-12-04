document.addEventListener("DOMContentLoaded", function () {
    const deleteBtn = document.getElementById("deleteBtn");
    if (!deleteBtn) return;

    const questionId = deleteBtn.dataset.questionid;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    deleteBtn.addEventListener("click", function () {
        Swal.fire({
            title: "Xác nhận xóa câu hỏi?",
            text: "Thao tác này sẽ xóa cả các đáp án liên quan và không thể khôi phục!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch("/Admin/QuestionAndAnswer/DeleteConfirmed", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": token
                    },
                    body: JSON.stringify({ id: questionId })
                })
                .then(response => {
                    if (response.ok) {
                        Swal.fire("Đã xóa!", "Câu hỏi đã được xóa thành công.", "success")
                            .then(() => window.location.href = "/Admin/QuestionAndAnswer");
                    } else {
                        Swal.fire("Lỗi!", "Không thể xóa câu hỏi. Vui lòng thử lại.", "error");
                    }
                })
                .catch(() => {
                    Swal.fire("Lỗi!", "Đã xảy ra lỗi khi xóa câu hỏi.", "error");
                });
            }
        });
    });
});
