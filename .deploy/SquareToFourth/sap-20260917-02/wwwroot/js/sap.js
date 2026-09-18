document.querySelectorAll('[data-print]').forEach(button => button.addEventListener('click', () => window.print()));
const form = document.querySelector('[data-discovery-form]');
if (form) {
    let dirty = false;
    const conditionals = [...form.querySelectorAll('[data-when]')];
    function updateConditions() {
        conditionals.forEach(section => {
            const dependency = section.dataset.when;
            if (!dependency) return;
            const input = document.getElementById(dependency);
            // Dependencies on earlier sections are resolved by the server.
            if (input) section.hidden = !input.value || (section.dataset.notEquals === 'true' ? input.value === section.dataset.equals : input.value !== section.dataset.equals);
            section.querySelectorAll('input,select,textarea').forEach(control => control.disabled = section.hidden);
        });
    }
    form.addEventListener('input', () => {
        dirty = true;
        form.querySelector('[data-dirty]').textContent = 'Unsaved changes';
        updateConditions();
    });
    form.addEventListener('submit', () => { dirty = false; });
    window.addEventListener('beforeunload', event => { if (dirty) { event.preventDefault(); event.returnValue = ''; } });
    updateConditions();
}
