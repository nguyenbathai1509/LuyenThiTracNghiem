(function () {
    if (!window.takeExamInit) {
        console.error("takeExamInit not found.");
        return;
    }

    const attemptId = Number.isFinite(+window.takeExamInit.attemptId) ? parseInt(window.takeExamInit.attemptId, 10) : null;
    let remaining = Number.isFinite(+window.takeExamInit.timeRemaining) ? parseInt(window.takeExamInit.timeRemaining, 10) : 0;
    const saveAnswerUrl = window.takeExamInit.saveAnswerUrl;
    const submitUrl = window.takeExamInit.submitUrl;

    const timeDisplay = document.getElementById('timeDisplay');
    const questionsList = document.getElementById('questionsList'); // scroll container
    const saveAllBtn = document.getElementById('saveAllBtn');
    const submitBtn = document.getElementById('submitBtn');
    const finishBtn = document.getElementById('finishLaterBtn');

    let isSubmitting = false;
    const perQuestionTimers = new Map();

    function formatTime(s) {
        const m = Math.floor(s / 60);
        const sec = s % 60;
        return `${m.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
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

    const timerInterval = setInterval(tick, 1000);
    tick();

    function getRequestVerificationToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    async function postFormData(url, formData, timeout = 8000) {
        try {
            const controller = new AbortController();
            const id = setTimeout(() => controller.abort(), timeout);
            const res = await fetch(url, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin',
                signal: controller.signal
            });
            clearTimeout(id);
            return res;
        } catch (err) {
            console.error('postFormData error', err);
            return null;
        }
    }

    function scheduleSaveAnswer(questionId, answerId) {
        if (perQuestionTimers.has(questionId)) {
            clearTimeout(perQuestionTimers.get(questionId));
        }
        const t = setTimeout(() => {
            perQuestionTimers.delete(questionId);
            saveAnswer(questionId, answerId);
        }, 300);
        perQuestionTimers.set(questionId, t);
    }

    async function saveAnswer(questionId, answerId) {
        const token = getRequestVerificationToken();
        const fd = new FormData();
        if (token) fd.append('__RequestVerificationToken', token);
        if (attemptId !== null) fd.append('attemptId', attemptId);
        fd.append('questionId', questionId);
        if (answerId !== null && answerId !== undefined) fd.append('answerId', answerId);

        const res = await postFormData(saveAnswerUrl, fd, 8000);
        if (!res || !res.ok) console.warn('SaveAnswer failed', res ? res.status : 'no response');
    }

    if (questionsList) {
        questionsList.addEventListener('change', function (ev) {
            const target = ev.target;
            if (!target) return;
            if (target.matches('.answer-radio')) {
                const qid = target.getAttribute('data-qid');
                const aid = target.getAttribute('data-aid');
                updateStatusBadgesForQuestion(qid, true);
                scheduleSaveAnswer(qid, aid);
            }
        });
    }

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

    function updateStatusBadgesForQuestion(qid, done) {
        const li = document.querySelector(`#questionStatus li[data-qid="${qid}"]`);
        if (!li) return;
        const badge = li.querySelector('.status-badge');
        if (!badge) return;
        badge.textContent = done ? 'Đã làm' : 'Chưa làm';
        badge.className = `badge ${done ? 'bg-success' : 'bg-secondary'} status-badge`;
    }

    if (saveAllBtn) {
        saveAllBtn.addEventListener('click', async function () {
            saveAllBtn.disabled = true;
            const questions = Array.from(document.querySelectorAll('.question'));
            for (const qDiv of questions) {
                const qid = qDiv.getAttribute('data-qid');
                const sel = qDiv.querySelector('input[type=radio]:checked');
                const aid = sel ? sel.value : null;
                await saveAnswer(qid, aid);
            }
            saveAllBtn.classList.add('btn-success');
            setTimeout(() => saveAllBtn.classList.remove('btn-success'), 800);
            alert('Đã lưu tạm các câu đã chọn.');
            saveAllBtn.disabled = false;
        });
    }

    if (submitBtn) {
        submitBtn.addEventListener('click', function () {
            if (!confirm('Bạn có chắc chắn muốn nộp bài? Sau khi nộp không thể sửa.')) return;
            if (isSubmitting) return;
            isSubmitting = true;
            clearInterval(timerInterval);
            submitExam();
        });
    }

    if (finishBtn) {
        finishBtn.addEventListener('click', function () {
            if (!confirm('Bạn sẽ rời trang làm bài. Lượt thi vẫn đang chạy. Bạn có chắc?')) return;
            window.location.href = '/';
        });
    }

    async function submitExam() {
        if (isSubmitting) return;
        isSubmitting = true;
        submitBtn.disabled = true;

        const answers = Array.from(document.querySelectorAll('.question')).map(qDiv => {
            const qid = parseInt(qDiv.getAttribute('data-qid'));
            const sel = qDiv.querySelector('input[type=radio]:checked');
            const aid = sel ? parseInt(sel.value) : null;
            return { QuestionId: qid, SelectedAnswerId: aid };
        });

        const payload = { AttemptId: attemptId, Answers: answers };
        const token = getRequestVerificationToken();

        try {
            const res = await fetch(submitUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) throw new Error('Nộp bài thất bại, vui lòng thử lại.');

            const json = await res.json();
            if (json.redirectUrl) {
                window.location.href = json.redirectUrl;
            } else {
                location.reload();
            }
        } catch (e) {
            alert(e.message);
            isSubmitting = false;
            submitBtn.disabled = false;
        }
    }

    function autoSubmit() {
        if (isSubmitting) return;
        alert('Hết thời gian, hệ thống sẽ tự nộp bài.');
        submitExam();
    }

    document.querySelectorAll('.question-link').forEach(a => {
        a.addEventListener('click', function (ev) {
            ev.preventDefault();
            const href = this.getAttribute('href');
            const target = document.querySelector(href);
            if (!target) return;

            if (questionsList && getComputedStyle(questionsList).overflowY !== 'visible') {
                const relativeTop = target.offsetTop;
                const centerOffset = Math.round(relativeTop - (questionsList.clientHeight / 2) + (target.clientHeight / 2));
                questionsList.scrollTo({ top: Math.max(0, centerOffset), behavior: 'smooth' });
            } else {
                target.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }

            target.classList.add('border-primary');
            setTimeout(() => target.classList.remove('border-primary'), 1200);
        });
    });

    updateStatusBadges();

    window.addEventListener('beforeunload', function (e) {
        if (!isSubmitting) {
            const msg = 'Bạn chưa nộp bài. Rời trang sẽ làm mất tiến độ.';
            e.returnValue = msg;
            return msg;
        }
    });
})();
