(function () {
    if (!window.takeExamInit) {
        console.error("takeExamInit not found.");
        return;
    }

    let remaining = Number.isFinite(+window.takeExamInit.timeRemaining) ? parseInt(window.takeExamInit.timeRemaining, 10) : 0;
    const timeDisplay = document.getElementById('timeDisplay');

    function formatTime(s) {
        const m = Math.floor(s / 60);
        const sec = s % 60;
        return `${m.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
    }

    function tick() {
        if (remaining <= 0) {
            timeDisplay.textContent = "00:00";
            clearInterval(timerInterval);
            alert("Hết giờ! Bài thi sẽ được tự động nộp. Vui lòng bấm Nộp bài ngay.");
            return;
        }
        timeDisplay.textContent = formatTime(remaining);
        remaining--;
    }

    const timerInterval = setInterval(tick, 1000);
    tick();

    // Cập nhật trạng thái câu hỏi bên sidebar
    function updateStatusBadges() {
        document.querySelectorAll('#questionStatus li').forEach(li => {
            const qid = li.getAttribute('data-qid');
            const checked = document.querySelector(`input[name="q_${qid}"]:checked`);
            const badge = li.querySelector('.status-badge');
            if (badge) {
                badge.textContent = checked ? 'Đã làm' : 'Chưa làm';
                badge.className = `badge ${checked ? 'bg-success' : 'bg-secondary'} status-badge`;
            }
        });
    }

    document.getElementById('questionsList')?.addEventListener('change', function (ev) {
        const target = ev.target;
        if (!target) return;
        if (target.matches('input[type=radio]')) {
            updateStatusBadges();
        }
    });

    // Chuyển câu hỏi khi click sidebar
    document.querySelectorAll('.question-link').forEach(a => {
        a.addEventListener('click', function (ev) {
            ev.preventDefault();
            const href = this.getAttribute('href');
            const target = document.querySelector(href);
            if (!target) return;
            target.scrollIntoView({ behavior: 'smooth', block: 'center' });
            target.classList.add('border-primary');
            setTimeout(() => target.classList.remove('border-primary'), 1200);
        });
    });

    updateStatusBadges();

    // Cảnh báo nếu người dùng rời trang
    window.addEventListener('beforeunload', function (e) {
        const msg = 'Bạn chưa nộp bài. Rời trang sẽ làm mất tiến độ.';
        e.returnValue = msg;
        return msg;
    });
})();
