document.querySelectorAll('[data-print]').forEach(button => button.addEventListener('click', () => window.print()));
const form = document.querySelector('[data-discovery-form]');
if (form) {
    let dirty = false;
    const conditionals = [...form.querySelectorAll('[data-when]')];
    const stickyActions = form.querySelector('.sticky-actions');

    function isAnswered(question) {
        const control = question.querySelector('input:not([type="hidden"]),select,textarea');
        return control && !control.disabled && control.value.trim().length > 0;
    }

    function updateProgress() {
        let sectionRequired = 0;
        let sectionAnswered = 0;
        form.querySelectorAll('[data-question-group]').forEach(group => {
            const questions = [...group.querySelectorAll('.question:not([hidden])')];
            const required = questions.filter(question => question.dataset.required === 'true');
            const answered = required.filter(isAnswered).length;
            const progress = group.querySelector('[data-group-progress]');
            if (progress) progress.textContent = `${answered} / ${required.length} required`;
            group.classList.toggle('is-complete', required.length > 0 && answered === required.length);
            group.hidden = questions.length === 0;
            sectionRequired += required.length;
            sectionAnswered += answered;
        });
        const progress = form.previousElementSibling?.querySelector?.('[data-section-progress]') || document.querySelector('[data-section-progress]');
        const progressText = form.previousElementSibling?.querySelector?.('[data-section-progress-text]') || document.querySelector('[data-section-progress-text]');
        if (progress) {
            progress.max = Math.max(sectionRequired, 1);
            progress.value = sectionAnswered;
        }
        if (progressText) progressText.textContent = `${sectionAnswered} / ${sectionRequired}`;
    }

    function updateConditions() {
        conditionals.forEach(section => {
            const dependency = section.dataset.when;
            if (!dependency) return;
            const input = document.getElementById(dependency);
            // Dependencies on earlier sections are resolved by the server.
            if (input) section.hidden = !input.value || (section.dataset.notEquals === 'true' ? input.value === section.dataset.equals : input.value !== section.dataset.equals);
            section.querySelectorAll('input,select,textarea').forEach(control => control.disabled = section.hidden);
        });
        updateProgress();
    }
    form.addEventListener('input', event => {
        dirty = true;
        form.querySelector('[data-dirty]').textContent = 'Unsaved changes';
        stickyActions?.classList.add('is-dirty');
        if (event.target instanceof HTMLTextAreaElement) {
            event.target.style.height = 'auto';
            event.target.style.height = `${Math.min(event.target.scrollHeight, 320)}px`;
        }
        updateConditions();
    });
    form.addEventListener('submit', () => { dirty = false; });
    window.addEventListener('beforeunload', event => { if (dirty) { event.preventDefault(); event.returnValue = ''; } });
    form.querySelectorAll('textarea').forEach(textarea => {
        if (textarea.value) {
            textarea.style.height = 'auto';
            textarea.style.height = `${Math.min(textarea.scrollHeight, 320)}px`;
        }
    });
    updateConditions();
}

const assistant = document.querySelector('[data-sap-assistant]');
if (assistant) {
    const assistantForm = assistant.querySelector('[data-assistant-form]');
    const question = assistant.querySelector('#assistant-question');
    const feed = assistant.querySelector('[data-assistant-feed]');
    const status = assistant.querySelector('[data-assistant-status]');
    const submit = assistantForm.querySelector('button[type="submit"]');
    const cancel = assistant.querySelector('[data-assistant-cancel]');
    const promptButtons = [...assistant.querySelectorAll('[data-assistant-prompt]')];
    const fieldButtons = [...document.querySelectorAll('[data-assistant-field]')];
    const assistedFields = new Set();
    const appliedValues = new Map();
    const history = [];
    let activeRequest = null;
    let applyingSuggestion = false;

    function addMessage(kind, title, text) {
        const message = document.createElement('div');
        message.className = `assistant-message ${kind}`;
        const strong = document.createElement('strong');
        strong.textContent = title;
        const body = document.createElement('p');
        body.textContent = text;
        message.append(strong, body);
        feed.appendChild(message);
        message.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        return message;
    }

    function currentAnswers() {
        const values = {};
        document.querySelectorAll('[name^="answers["]:not(:disabled)').forEach(control => {
            const match = control.name.match(/^answers\[(.+)]$/);
            if (match) values[match[1]] = control.value;
        });
        return values;
    }

    function syncAssistedFields() {
        const target = document.querySelector('[data-assistant-fields]');
        if (target) target.value = [...assistedFields].sort().join(',');
    }

    function setBusy(busy) {
        submit.disabled = busy;
        promptButtons.forEach(button => { button.disabled = busy; });
        fieldButtons.forEach(button => { button.disabled = busy; });
        cancel.hidden = !busy;
        assistant.setAttribute('aria-busy', busy ? 'true' : 'false');
    }

    function addFindings(findings, container) {
        if (!findings?.length) return;
        const list = document.createElement('div');
        list.className = 'assistant-findings';
        findings.forEach(finding => {
            const item = document.createElement('div');
            item.className = `assistant-finding ${finding.kind || 'missing'}`;
            const kind = document.createElement('strong');
            kind.textContent = finding.kind || 'note';
            const text = document.createElement('span');
            text.textContent = finding.text;
            item.append(kind, text);
            list.appendChild(item);
        });
        container.appendChild(list);
    }

    function addSuggestion(suggestion, container) {
        const card = document.createElement('div');
        card.className = 'assistant-suggestion';
        const label = document.querySelector(`label[for="${CSS.escape(suggestion.fieldId)}"]`)?.textContent?.replace('*', '').trim() || suggestion.fieldId;
        const heading = document.createElement('strong');
        heading.textContent = label;
        const value = document.createElement('p');
        value.textContent = suggestion.suggestedValue;
        const target = document.getElementById(suggestion.fieldId);
        if (!target) return;
        if (target.value && target.value !== suggestion.suggestedValue) {
            const existing = document.createElement('p');
            existing.className = 'assistant-existing';
            existing.textContent = `Current answer: ${target.value}`;
            card.appendChild(existing);
        }
        const rationale = document.createElement('small');
        rationale.textContent = suggestion.rationale;
        const use = document.createElement('button');
        use.type = 'button';
        use.className = 'assistant-apply';
        use.textContent = 'Use this draft';
        let replacementConfirmed = false;
        use.addEventListener('click', () => {
            if (target.value && target.value !== suggestion.suggestedValue && !replacementConfirmed) {
                replacementConfirmed = true;
                use.textContent = 'Confirm replace';
                use.classList.add('confirm');
                return;
            }
            const previousValue = target.value;
            applyingSuggestion = true;
            target.value = suggestion.suggestedValue;
            target.dispatchEvent(new Event('input', { bubbles: true }));
            target.dispatchEvent(new Event('change', { bubbles: true }));
            applyingSuggestion = false;
            assistedFields.add(suggestion.fieldId);
            appliedValues.set(suggestion.fieldId, suggestion.suggestedValue);
            syncAssistedFields();
            use.textContent = 'Added - save the form';
            use.disabled = true;
            const undo = document.createElement('button');
            undo.type = 'button';
            undo.className = 'assistant-undo';
            undo.textContent = 'Undo';
            undo.addEventListener('click', () => {
                if (target.value !== suggestion.suggestedValue) {
                    status.textContent = 'This answer changed after the draft was applied, so it was not overwritten.';
                    return;
                }
                applyingSuggestion = true;
                target.value = previousValue;
                target.dispatchEvent(new Event('input', { bubbles: true }));
                target.dispatchEvent(new Event('change', { bubbles: true }));
                applyingSuggestion = false;
                assistedFields.delete(suggestion.fieldId);
                appliedValues.delete(suggestion.fieldId);
                syncAssistedFields();
                replacementConfirmed = false;
                use.textContent = previousValue ? 'Replace with this draft' : 'Use this draft';
                use.classList.remove('confirm');
                use.disabled = false;
                undo.remove();
                status.textContent = 'Draft removed. Nothing has been saved.';
                target.focus();
            });
            card.appendChild(undo);
            target.focus();
        });
        card.append(heading, value, rationale, use);
        container.appendChild(card);
    }

    async function ask(prompt, mode = 'ask', fieldId = '') {
        prompt = (prompt || '').trim();
        if (prompt.length < 2 || activeRequest) { question.focus(); return; }
        const priorHistory = history.slice(-6);
        const title = fieldId ? `Help with ${document.querySelector(`[data-assistant-field="${CSS.escape(fieldId)}"]`)?.dataset.assistantLabel || fieldId}` : 'You';
        addMessage('user', title, prompt);
        history.push({ role: 'user', text: prompt });
        status.textContent = 'Thinking through this section...';
        activeRequest = new AbortController();
        setBusy(true);
        try {
            const token = assistantForm.querySelector('[name="__RequestVerificationToken"]').value;
            const response = await fetch(assistant.dataset.endpoint, {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                signal: activeRequest.signal,
                body: JSON.stringify({ message: prompt, mode, fieldId, draftAnswers: currentAnswers(), history: priorHistory })
            });
            const data = await response.json().catch(() => ({}));
            if (!response.ok) throw new Error(data.error || (response.status === 429 ? 'The guide is busy. Please wait a moment and try again.' : 'The guide could not answer just now.'));
            const message = addMessage('guide', data.isFallback ? 'Form guide' : 'Guide', data.answer || 'I need a little more detail to help with that.');
            history.push({ role: 'assistant', text: data.answer || '' });
            addFindings(data.findings, message);
            (data.warnings || []).forEach(warning => {
                const note = document.createElement('p');
                note.className = 'assistant-warning';
                note.textContent = warning;
                message.appendChild(note);
            });
            if ((data.followUpQuestions || []).length) {
                const followups = document.createElement('div');
                followups.className = 'assistant-followups';
                data.followUpQuestions.forEach(item => {
                    const button = document.createElement('button');
                    button.type = 'button';
                    button.textContent = item;
                    button.addEventListener('click', () => { question.value = item; question.focus(); });
                    followups.appendChild(button);
                });
                message.appendChild(followups);
            }
            (data.suggestions || []).forEach(item => addSuggestion(item, message));
            question.value = '';
            status.textContent = 'Nothing was saved or submitted.';
        } catch (error) {
            if (error.name === 'AbortError') {
                if (history.at(-1)?.role === 'user' && history.at(-1)?.text === prompt) history.pop();
                status.textContent = 'Request cancelled. Your form has not been changed.';
            } else {
                addMessage('error', 'Guide unavailable', error.message);
                status.textContent = 'Your form has not been changed.';
            }
        } finally {
            activeRequest = null;
            setBusy(false);
        }
    }

    assistantForm.addEventListener('submit', event => { event.preventDefault(); ask(question.value); });
    cancel.addEventListener('click', () => activeRequest?.abort());
    promptButtons.forEach(button => button.addEventListener('click', () => ask(button.dataset.assistantPrompt, button.dataset.assistantMode || 'ask')));
    fieldButtons.forEach(button => button.addEventListener('click', () => {
        assistant.scrollIntoView({ behavior: 'smooth', block: 'start' });
        ask(`Explain what this question needs, identify any missing facts, and draft an answer only if my existing requirements support it.`, 'field', button.dataset.assistantField);
    }));
    form?.addEventListener('input', event => {
        const fieldId = event.target?.id;
        if (!applyingSuggestion && fieldId && appliedValues.has(fieldId) && event.target.value !== appliedValues.get(fieldId)) {
            appliedValues.delete(fieldId);
            assistedFields.delete(fieldId);
            syncAssistedFields();
        }
    });
}
