// result.js - JavaScript cho trang kết quả thi
document.addEventListener('DOMContentLoaded', function() {
    // Thêm hiệu ứng xuất hiện tuần tự cho các câu hỏi
    const questions = document.querySelectorAll('.list-group-item');
    
    questions.forEach((question, index) => {
        question.style.opacity = '0';
        question.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            question.style.transition = 'all 0.6s ease-out';
            question.style.opacity = '1';
            question.style.transform = 'translateY(0)';
        }, index * 100);
    });
    
    // Hiệu ứng hover cho đáp án
    const answerItems = document.querySelectorAll('.list-group-item .list-group-item');
    
    answerItems.forEach(item => {
        item.addEventListener('mouseenter', function() {
            if (!this.classList.contains('list-group-item-success') && 
                !this.classList.contains('list-group-item-danger')) {
                this.style.backgroundColor = '#e8f4fc';
                this.style.borderLeftColor = '#3498db';
            }
        });
        
        item.addEventListener('mouseleave', function() {
            if (!this.classList.contains('list-group-item-success') && 
                !this.classList.contains('list-group-item-danger')) {
                this.style.backgroundColor = '';
                this.style.borderLeftColor = '';
            }
        });
    });
    
    // Hiệu ứng cho nút quay lại
    const backButton = document.querySelector('.btn-primary');
    
    backButton.addEventListener('click', function(e) {
        e.preventDefault();
        
        // Thêm hiệu ứng loading
        this.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang chuyển hướng...';
        this.disabled = true;
        
        // Giả lập thời gian chuyển hướng
        setTimeout(() => {
            window.location.href = this.href;
        }, 1000);
    });
    
    // Thêm chức năng tìm kiếm câu hỏi (có thể mở rộng)
    console.log('Trang kết quả thi đã được tải hoàn tất!');
});