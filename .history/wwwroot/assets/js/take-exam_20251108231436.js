// --- init data from server ---
    const attemptId = parseInt(document.getElementById('attemptId').value, 10);
    let remaining = @Model.TimeRemainingSeconds; // seconds from server (integer)
    const timeDisplay = document.getElementById('timeDisplay');

    function formatTime(s) {
        const m = Math.floor(s / 60);
        const sec = s % 60;
        return `${m.toString().padStart(2,'0')}:${sec.toString().padStart(2,'0')}`;
    }

    // Timer tick
    function tick() {
        if (remaining <= 0) {
            timeDisplay.textContent = "00:00";
            autoSubmit();
            return;
        }
        timeDisplay.textContent = formatTime(remaining);
        remaining--;
    }

    tick();
    const timerInterval = setInterval(tick, 1000);

    // Update sidebar statuses
    function updateStatusBadges() {
        document.querySelectorAll('#questionStatus li').forEach(li => {
            const qid = li.getAttribute('data-qid');
            const checked = document.querySelector(`input[name="q_${qid}"]:checked`);
            const badge = li.querySelector('.status-badge');
            if (checked) {
                badge.textContent = 'Đã làm';
                badge.className = 'badge bg-success status-badge';
            } else {
                badge.textContent = 'Chưa làm';
                badge.className = 'badge bg-secondary status-badge';
            }
        });
    }

    // Get antiforgery token
    function getRequestVerificationToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    // Save one answer (autosave)
    async function saveAnswer(questionId, answerId) {
        const token = getRequestVerificationToken();
        const fd = new FormData();
        fd.append('__RequestVerificationToken', token);
        fd.append('attemptId', attemptId);
        fd.append('questionId', questionId);
        if (answerId !== null && answerId !== undefined) fd.append('answerId', answerId);

        try {
            const res = await fetch('@Url.Action("SaveAnswer","TakeExam")', {
                method: 'POST',
                body: fd,
                credentials: 'same-origin'
            });
            // optional: handle json
            if (!res.ok) console.warn('SaveAnswer failed', res.status);
        } catch (err) {
            console.error('SaveAnswer error', err);
        }
    }

    // Attach change listeners to radio inputs for autosave
    document.querySelectorAll('.answer-radio').forEach(radio => {
        radio.addEventListener('change', function() {
            const qid = this.getAttribute('data-qid');
            const aid = this.getAttribute('data-aid');
            updateStatusBadges();
            saveAnswer(qid, aid);
        });
    });

    // Manual save all selected answers
    document.getElementById('saveAllBtn').addEventListener('click', function() {
        document.querySelectorAll('.question').forEach(qDiv => {
            const qid = qDiv.getAttribute('data-qid');
            const sel = qDiv.querySelector('input[type=radio]:checked');
            const aid = sel ? sel.value : null;
            saveAnswer(qid, aid);
        });
        // small UI feedback
        this.classList.add('btn-success');
        setTimeout(() => this.classList.remove('btn-success'), 1000);
        alert('Đã lưu tạm các câu đã chọn.');
    });

    // Submit exam
    document.getElementById('submitBtn').addEventListener('click', function() {
        if (!confirm('Bạn có chắc chắn muốn nộp bài? Sau khi nộp không thể sửa.')) return;
        // stop timer
        clearInterval(timerInterval);
        submitExam();
    });

    // Finish later (just navigate away but keep attempt running)
    document.getElementById('finishLaterBtn').addEventListener('click', function() {
        if (!confirm('Bạn sẽ rời trang làm bài. Lượt thi vẫn đang chạy. Bạn có chắc?')) return;
        window.location.href = '/'; // or any page you want
    });

    // Submit by POST to Submit action; handle redirect
    async function submitExam() {
        const token = getRequestVerificationToken();
        const fd = new FormData();
        fd.append('__RequestVerificationToken', token);
        fd.append('attemptId', attemptId);

        try {
            const res = await fetch('@Url.Action("Submit","TakeExam")', {
                method: 'POST',
                body: fd,
                credentials: 'same-origin'
            });

            // if server redirects, follow it
            if (res.redirected) {
                window.location.href = res.url;
                return;
            }

            if (res.ok) {
                // try to parse json or reload
                const txt = await res.text();
                // fallback: reload (or redirect to result page if server returns URL)
                location.reload();
            } else {
                alert('Nộp bài thất bại. Vui lòng thử lại.');
            }
        } catch (err) {
            console.error('submitExam error', err);
            alert('Có lỗi khi nộp bài.');
        }
    }

    // Auto submit when time runs out
    function autoSubmit() {
        alert('Hết thời gian, hệ thống sẽ tự nộp bài.');
        submitExam();
    }

    // Jump to question when clicking sidebar link
    document.querySelectorAll('.question-link').forEach(a => {
        a.addEventListener('click', function(ev) {
            ev.preventDefault();
            const href = this.getAttribute('href');
            const target = document.querySelector(href);
            if (target) {
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                // optionally highlight
                target.classList.add('border-primary');
                setTimeout(()=> target.classList.remove('border-primary'), 1200);
            }
        });
    });

    // initial badges
    updateStatusBadges();

    // Optional: periodically refresh remaining time from server every X minutes
    // Optional: store answers in localStorage as backup