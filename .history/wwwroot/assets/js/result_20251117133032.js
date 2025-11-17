// exam-result.js
document.addEventListener("DOMContentLoaded", function () {
    // Thêm hiệu ứng khi hover các câu hỏi
    document.querySelectorAll(".list-group-item").forEach(item => {
        item.addEventListener("mouseenter", function () {
            this.classList.add("shadow-sm");
        });
        item.addEventListener("mouseleave", function () {
            this.classList.remove("shadow-sm");
        });
    });

    // Tự động cuộn tới câu trả lời sai hoặc chưa làm đầu tiên
    const firstWrong = document.querySelector(".list-group-item.bg-danger, .list-group-item.bg-secondary");
    if (firstWrong) {
        firstWrong.scrollIntoView({ behavior: "smooth", block: "start" });
    }
});
