// wwwroot/js/take-exam.js (cải tiến)
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
    const questionsList = document.getElementById('questionsList'); // container scroll
    const saveAllBtn = document.getElementById('saveAllBtn');
    const submitBtn = document.getElementById('submitBtn');
    const finishBtn = document.getElementById('finishLaterBtn');

    // safety flags
    let isSubmitting = false;
    const perQuestionTimers = new Map(); // for debounce per question

    function formatTime(s) {
        const m = Math.floor(s / 60);
        const sec = s % 60;
        return `${m.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
    }

    function tick() {
        if (remaining <= 0) {
            timeDisplay.textContent = "00:00";
            // clear timer before auto-submit to avoid double calls
            clearInterval(timerInterval);
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

    // fetch with timeout
    async function fetchWithTimeout(resource, options = {}, timeout = 8000) {
        const controller = new AbortController();
        const id = setTimeout(() => controller.abort(), timeout);
        const finalOptions = Object.assign({}, options, { signal: controller.signal });
        try {
            const res = await fetch(resource, finalOptions);
            clearTimeout(id);
            return res;
        } catch (err) {
            clearTimeout(id);
            throw err;
        }
    }

    async function postFormData(url, formData, timeout = 8000) {
        try {
            const res = await fetchWithTimeout(url, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin'
            }, timeout);
            return res;
        } catch (err) {
            console.error('postFormData error', err);
            return null;
        }
    }

    // save one answer (debounced per question)
    function scheduleSaveAnswer(questionId, answerId) {
        // clear previous timer for this question
        if (perQuestionTimers.has(questionId)) {
            clearTimeout(perQuestionTimers.get(questionId));
        }
        // schedule a save after 300ms (so quick toggles don't create many requests)
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
        if (!res) {
            // optional: show small toast instead of alert
            console.warn('SaveAnswer failed: no response');
            return;
        }
        if (!res.ok) {
            console.warn('SaveAnswer failed', res.status);
        }
        // optionally read json response for per-question status
    }

    // event delegation for radio changes (more efficient)
    if (questionsList) {
        questionsList.addEventListener('change', function (ev) {
            const target = ev.target;
            if (!target) return;
            if (target.matches && target.matches('.answer-radio')) {
                const qid = target.getAttribute('data-qid');
                const aid = target.getAttribute('data-aid');
                // update UI badges immediately
                updateStatusBadgesForQuestion(qid, true);
                // schedule save (debounced)
                scheduleSaveAnswer(qid, aid);
            }
        });
    } else {
        // fallback: attach to document (shouldn't be necessary)
        document.addEventListener('change', function (ev) {
            const target = ev.target;
            if (target && target.matches && target.matches('.answer-radio')) {
                const qid = target.getAttribute('data-qid');
                const aid = target.getAttribute('data-aid');
                updateStatusBadgesForQuestion(qid, true);
                scheduleSaveAnswer(qid, aid);
            }
        });
    }

    // update all badges
    function updateStatusBadges() {
        document.querySelectorAll('#questionStatus li').forEach(li => {
            const qid = li.getAttribute('data-qid');
            const checked = document.querySelector(`input[name="q_${qid}"]:checked`);
            const badge = li.querySelector('.status-badge');
            if (badge) {
                if (checked) {
                    badge.textContent = 'Đã làm';
                    badge.className = 'badge bg-success status-badge';
                } else {
                    badge.textContent = 'Chưa làm';
                    badge.className = 'badge bg-secondary status-badge';
                }
            }
        });
    }

    // update only one question's badge
    function updateStatusBadgesForQuestion(qid, done) {
        const li = document.querySelector(`#questionStatus li[data-qid="${qid}"]`);
        if (!li) return;
        const badge = li.querySelector('.status-badge');
        if (!badge) return;
        if (done) {
            badge.textContent = 'Đã làm';
            badge.className = 'badge bg-success status-badge';
        } else {
            badge.textContent = 'Chưa làm';
            badge.className = 'badge bg-secondary status-badge';
        }
    }

    // manual save all selected answers
    if (saveAllBtn) {
        saveAllBtn.addEventListener('click', async function () {
            saveAllBtn.disabled = true;
            const questions = Array.from(document.querySelectorAll('.question'));
            // send sequentially to avoid flooding server (or you can send parallel with Promise.all)
            for (const qDiv of questions) {
                const qid = qDiv.getAttribute('data-qid');
                const sel = qDiv.querySelector('input[type=radio]:checked');
                const aid = sel ? sel.value : null;
                try {
                    await saveAnswer(qid, aid);
                } catch (e) {
                    console.warn('Error saving question', qid, e);
                }
            }
            // feedback
            saveAllBtn.classList.add('btn-success');
            setTimeout(() => saveAllBtn.classList.remove('btn-success'), 800);
            // optional: small non-blocking toast instead of alert
            alert('Đã lưu tạm các câu đã chọn.');
            saveAllBtn.disabled = false;
        });
    }

    // submit
    if (submitBtn) {
        submitBtn.addEventListener('click', function () {
            if (!confirm('Bạn có chắc chắn muốn nộp bài? Sau khi nộp không thể sửa.')) return;
            // prevent double submit
            if (isSubmitting) return;
            isSubmitting = true;
            clearInterval(timerInterval);
            submitExam();
        });
    }

    // finish later
    if (finishBtn) {
        finishBtn.addEventListener('click', function () {
            if (!confirm('Bạn sẽ rời trang làm bài. Lượt thi vẫn đang chạy. Bạn có chắc?')) return;
            window.location.href = '/';
        });
    }

    async function submitExam() {
        // prevent double submit
        if (isSubmitting) return;
        isSubmitting = true;
        submitBtn.disabled = true;

        const token = getRequestVerificationToken();
        const fd = new FormData();
        if (token) fd.append('__RequestVerificationToken', token);
        if (attemptId !== null) fd.append('attemptId', attemptId);

        const res = await postFormData(submitUrl, fd, 10000);
        if (!res) {
            alert('Không thể kết nối server, vui lòng thử lại.');
            isSubmitting = false;
            submitBtn.disabled = false;
            return;
        }

        if (res.redirected) {
            window.location.href = res.url;
            return;
        }

        if (res.ok) {
            const ct = res.headers.get('content-type') || '';
            if (ct.indexOf('application/json') !== -1) {
                try {
                    const j = await res.json();
                    if (j && j.redirectUrl) {
                        window.location.href = j.redirectUrl;
                        return;
                    }
                } catch (e) {
                    console.warn('Could not parse JSON response', e);
                }
            }
            // fallback: reload page (server may have set session status)
            location.reload();
        } else {
            // read possible error message
            let msg = 'Nộp bài thất bại. Vui lòng thử lại.';
            try {
                const txt = await res.text();
                if (txt) msg += '\n' + txt;
            } catch (e) { /* ignore */ }
            alert(msg);
            isSubmitting = false;
            submitBtn.disabled = false;
        }
    }

    // auto submit when time runs out
    function autoSubmit() {
        if (isSubmitting) return;
        alert('Hết thời gian, hệ thống sẽ tự nộp bài.');
        submitExam();
    }

    // jump from sidebar: scroll inside questionsList if it is the scroll container,
    // otherwise fallback to scrollIntoView on the element (which may scroll page).
    // Scroll target to center of questionsList (more robust)
    document.querySelectorAll('.question-link').forEach(a => {
        a.addEventListener('click', function (ev) {
            ev.preventDefault();
            const href = this.getAttribute('href');
            const target = document.querySelector(href);
            if (!target) return;

            if (questionsList && getComputedStyle(questionsList).overflowY !== 'visible') {
                // nếu .question trực tiếp nằm trong #questionsList (không có nhiều nested offsetParent),
                // offsetTop là cách ổn định để tính vị trí trong container
                const relativeTop = target.offsetTop; // distance from top of container's content
                const centerOffset = Math.round(relativeTop - (questionsList.clientHeight / 2) + (target.clientHeight / 2));
                questionsList.scrollTo({ top: Math.max(0, centerOffset), behavior: 'smooth' });
            } else {
                // fallback: scroll whole page and center block if possible
                target.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }

            target.classList.add('border-primary');
            setTimeout(() => target.classList.remove('border-primary'), 1200);
        });
    });


    // initial status
    updateStatusBadges();

    // optional: beforeunload warn if not submitted (you may want to toggle this)
    window.addEventListener('beforeunload', function (e) {
        if (!isSubmitting) {
            const msg = 'Bạn chưa nộp bài. Rời trang sẽ làm mất tiến độ.';
            e.returnValue = msg;
            return msg;
        }
    });
})();


async function submitExam() {
    if (isSubmitting) return;
    isSubmitting = true;
    submitBtn.disabled = true;

    const attemptId = window.takeExamInit.attemptId;

    // Lấy tất cả câu trả lời đã chọn
    const answers = Array.from(document.querySelectorAll('.question')).map(qDiv => {
        const qid = parseInt(qDiv.getAttribute('data-qid'));
        const sel = qDiv.querySelector('input[type=radio]:checked');
        const aid = sel ? parseInt(sel.value) : null;
        return { QuestionId: qid, SelectedAnswerId: aid };
    });

    // Gửi POST bằng fetch
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    const payload = { AttemptId: attemptId, Answers: answers };

    const res = await fetch(window.takeExamInit.submitUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify(payload)
    });

    if (!res.ok) {
        alert('Nộp bài thất bại, vui lòng thử lại.');
        isSubmitting = false;
        submitBtn.disabled = false;
        return;
    }

    // Chuyển sang trang kết quả
    const json = await res.json();
    if (json.redirectUrl) {
        window.location.href = json.redirectUrl;
    } else {
        location.reload();
    }

    if (submitBtn) {
        submitBtn.addEventListener('click', function () {
            if (!confirm('Bạn có chắc chắn muốn nộp bài?')) return;
            submitExam();
        });
    }
}
