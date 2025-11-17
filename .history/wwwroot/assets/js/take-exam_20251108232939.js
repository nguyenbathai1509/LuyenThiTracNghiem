// wwwroot/js/takeexam.js
(function () {
    if (!window.takeExamInit) {
        console.error("takeExamInit not found.");
        return;
    }

    const attemptId = parseInt(window.takeExamInit.attemptId, 10);
    let remaining = parseInt(window.takeExamInit.timeRemaining, 10) || 0;
    const saveAnswerUrl = window.takeExamInit.saveAnswerUrl;
    const submitUrl = window.takeExamInit.submitUrl;

    const timeDisplay = document.getElementById('timeDisplay');

    function formatTime(s) {
        const m = Math.floor(s / 60);
        const sec = s % 60;
        return `${m.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
    }

    function tick() {
        if (remaining <= 0) {
            timeDisplay.textContent = "00:00";
            autoSubmit();
            return;
        }
        timeDisplay.textContent = formatTime(remaining);
        remaining--;
    }

    // start timer
    tick();
    const timerInterval = setInterval(tick, 1000);

    // helpers
    function getRequestVerificationToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    async function postFormData(url, formData) {
        try {
            const res = await fetch(url, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin'
            });
            return res;
        } catch (err) {
            console.error('postFormData error', err);
            return null;
        }
    }

    // save one answer
    async function saveAnswer(questionId, answerId) {
        const token = getRequestVerificationToken();
        const fd = new FormData();
        if (token) fd.append('__RequestVerificationToken', token);
        fd.append('attemptId', attemptId);
        fd.append('questionId', questionId);
        if (answerId !== null && answerId !== undefined) fd.append('answerId', answerId);

        const res = await postFormData(saveAnswerUrl, fd);
        if (res && !res.ok) console.warn('SaveAnswer failed', res.status);
    }

    // attach listeners to radios
    document.querySelectorAll('.answer-radio').forEach(radio => {
        radio.addEventListener('change', function () {
            const qid = this.getAttribute('data-qid');
            const aid = this.getAttribute('data-aid');
            updateStatusBadges();
            saveAnswer(qid, aid);
        });
    });

    // update sidebar badges
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

    // manual save all selected answers
    const saveAllBtn = document.getElementById('saveAllBtn');
    if (saveAllBtn) {
        saveAllBtn.addEventListener('click', function () {
            document.querySelectorAll('.question').forEach(qDiv => {
                const qid = qDiv.getAttribute('data-qid');
                const sel = qDiv.querySelector('input[type=radio]:checked');
                const aid = sel ? sel.value : null;
                saveAnswer(qid, aid);
            });
            saveAllBtn.classList.add('btn-success');
            setTimeout(() => saveAllBtn.classList.remove('btn-success'), 800);
            alert('Đã lưu tạm các câu đã chọn.');
        });
    }

    // submit
    const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
        submitBtn.addEventListener('click', function () {
            if (!confirm('Bạn có chắc chắn muốn nộp bài? Sau khi nộp không thể sửa.')) return;
            clearInterval(timerInterval);
            submitExam();
        });
    }

    // finish later
    const finishBtn = document.getElementById('finishLaterBtn');
    if (finishBtn) {
        finishBtn.addEventListener('click', function () {
            if (!confirm('Bạn sẽ rời trang làm bài. Lượt thi vẫn đang chạy. Bạn có chắc?')) return;
            window.location.href = '/';
        });
    }

    async function submitExam() {
        const token = getRequestVerificationToken();
        const fd = new FormData();
        if (token) fd.append('__RequestVerificationToken', token);
        fd.append('attemptId', attemptId);

        const res = await postFormData(submitUrl, fd);
        if (!res) {
            alert('Không thể kết nối server, vui lòng thử lại.');
            return;
        }

        // nếu server redirect (code 302) fetch won't follow as browser redirect,
        // nhưng server can return JSON { redirectUrl: "..." } or return 200 with meta
        if (res.redirected) {
            window.location.href = res.url;
            return;
        }

        if (res.ok) {
            // try parse json for redirect
            const ct = res.headers.get('content-type') || '';
            if (ct.indexOf('application/json') !== -1) {
                const j = await res.json();
                if (j && j.redirectUrl) {
                    window.location.href = j.redirectUrl;
                    return;
                }
            }
            // fallback: reload page (server may have set location header)
            location.reload();
        } else {
            alert('Nộp bài thất bại. Vui lòng thử lại.');
        }
    }

    // auto submit when time runs out
    function autoSubmit() {
        alert('Hết thời gian, hệ thống sẽ tự nộp bài.');
        submitExam();
    }

    // jump from sidebar
    document.querySelectorAll('.question-link').forEach(a => {
        a.addEventListener('click', function (ev) {
            ev.preventDefault();
            const href = this.getAttribute('href');
            const target = document.querySelector(href);
            if (target) {
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                target.classList.add('border-primary');
                setTimeout(() => target.classList.remove('border-primary'), 1200);
            }
        });
    });

    // initial status
    updateStatusBadges();

    // Optional: autosave heartbeat, session keepalive, localStorage backup...
})();
