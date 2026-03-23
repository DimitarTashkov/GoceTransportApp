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

            // If jQuery Validate is attached to this form and the form is
            // currently invalid, bail out — the validator will display errors
            // and block the real submit. We must NOT freeze the button here,
            // otherwise the user can never retry after fixing the fields.
            if (typeof $ !== 'undefined' && $(form).data('validator')) {
                if (!$(form).valid()) {
                    return;
                }
            }

            // Persist original label for bfcache/back-nav restore
            btn.setAttribute('data-original-html', btn.innerHTML);
            btn.disabled = true;
            btn.innerHTML =
                '<span class="spinner-border spinner-border-sm me-2"' +
                ' role="status" aria-hidden="true"></span>' +
                'Processing...';
        });
    });

    // ═══════════════════════════════════════════════════════
    // SECTION 5 — TOAST NOTIFICATIONS (auto-show & dismiss)
    // ═══════════════════════════════════════════════════════
    //
    // Bootstrap Toasts require explicit .show() call via JS.
    // We grab every .toast element rendered by _NotificationsPartial
    // and initialize + show them immediately.
    // The data-bs-delay="4500" attribute on each toast controls
    // the auto-hide timing — no extra logic needed here.
    //
    document.querySelectorAll('.toast').forEach(function (el) {
        bootstrap.Toast.getOrCreateInstance(el).show();
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
