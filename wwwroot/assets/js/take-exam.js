(function () {
    // Kiểm tra biến khởi tạo
    if (!window.takeExamInit) {
        console.error("takeExamInit not found.");
        return;
    }

    // Đồng hồ đếm ngược
    let remaining = Number(window.takeExamInit.timeRemaining) || 0;
    let submitted = false;
    const timeDisplay = document.getElementById('timeDisplay');
    const form = document.getElementById('takeExamForm');

    function formatTime(seconds) {
        const m = Math.floor(seconds / 60);
        const s = seconds % 60;
        return `${m.toString().padStart(2,'0')}:${s.toString().padStart(2,'0')}`;
    }

    function tick() {
        if (remaining <= 0) {
            timeDisplay.textContent = "00:00";
            clearInterval(timerInterval);
            autoSubmit();
            return;
        }
        timeDisplay.textContent = formatTime(remaining);
        remaining--;
    }

    function autoSubmit() {
        if (submitted || !form) return;
        submitted = true;
        window.removeEventListener('beforeunload', beforeUnloadHandler);
        const alertBox = document.createElement('div');
        alertBox.className = 'alert alert-warning mb-2';
        alertBox.textContent = 'Hết giờ! Bài thi đang được nộp tự động...';
        form.prepend(alertBox);
        form.submit();
    }

    const timerInterval = setInterval(tick, 1000);
    tick();

    // Cập nhật trạng thái sidebar
    function updateStatusBadges() {
        document.querySelectorAll('#questionsList .question').forEach(q => {
            const qid = q.dataset.qid;
            const checked = q.querySelector('input[type=radio]:checked');
            const badge = document.querySelector(`#questionStatus li a[href="#q-${qid}"] .status-badge`);
            if (badge) {
                badge.textContent = checked ? 'Đã làm' : 'Chưa làm';
                badge.className = `badge ${checked ? 'bg-success' : 'bg-secondary'} status-badge`;
            }
        });
    }

    // Lắng nghe radio để cập nhật trạng thái
    const questionsList = document.getElementById('questionsList');
    if (questionsList) {
        questionsList.addEventListener('change', e => {
            if (e.target.matches('input[type=radio]')) {
                updateStatusBadges();
            }
        });
    }

    // Scroll đến câu hỏi khi click sidebar
    document.querySelectorAll('.question-link').forEach(a => {
        a.addEventListener('click', e => {
            e.preventDefault();
            const target = document.querySelector(a.getAttribute('href'));
            if (!target) return;
            target.scrollIntoView({ behavior: 'smooth', block: 'center' });
            target.classList.add('border-primary');
            setTimeout(() => target.classList.remove('border-primary'), 1200);
        });
    });

    // Cảnh báo khi rời trang
    const beforeUnloadHandler = e => {
        e.returnValue = 'Bạn chưa nộp bài. Rời trang sẽ làm mất tiến độ.';
        return e.returnValue;
    };
    window.addEventListener('beforeunload', beforeUnloadHandler);

    if (form) {
        form.addEventListener('submit', () => {
            submitted = true;
            window.removeEventListener('beforeunload', beforeUnloadHandler);
        });
    }

    // Cập nhật ngay khi load
    updateStatusBadges();
})();
