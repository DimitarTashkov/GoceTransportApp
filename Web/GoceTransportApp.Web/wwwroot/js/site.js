// GoceTransportApp — Global UX Scripts
// ─────────────────────────────────────────────────────────
// Runs after DOM is ready (script loaded at bottom of <body>).

(function () {
    'use strict';

    // ═══════════════════════════════════════════════════════
    // SECTION 4 — FORM SUBMIT LOADING STATE
    // ═══════════════════════════════════════════════════════
    //
    // For every <form> on the page: on submit, find the primary
    // submit button, disable it to prevent double-submission, and
    // replace its content with a Bootstrap spinner + "Обработване...".
    //
    // We cache the original innerHTML so the browser back-button
    // scenario (bfcache restore) can re-enable the button.
    //
    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function () {
            var btn = form.querySelector('[type="submit"]');
            if (!btn || btn.disabled) return;

            // Persist original label for bfcache/back-nav restore
            btn.setAttribute('data-original-html', btn.innerHTML);
            btn.disabled = true;
            btn.innerHTML =
                '<span class="spinner-border spinner-border-sm me-2"' +
                ' role="status" aria-hidden="true"></span>' +
                'Обработване...';
        });
    });

    // Restore buttons if the browser restores the page from bfcache
    // (e.g. user hits Back after a successful POST redirect)
    window.addEventListener('pageshow', function (e) {
        if (e.persisted) {
            document.querySelectorAll('[data-original-html]').forEach(function (btn) {
                btn.innerHTML = btn.getAttribute('data-original-html');
                btn.disabled = false;
                btn.removeAttribute('data-original-html');
            });
        }
    });

})();
