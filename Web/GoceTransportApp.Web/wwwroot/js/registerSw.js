// PWA — Service Worker Registration + Install Prompt
(function () {
    'use strict';

    // ── Service Worker Registration ──────────────────────────────────────────
    if ('serviceWorker' in navigator) {
        window.addEventListener('load', function () {
            navigator.serviceWorker.register('/sw.js', { scope: '/' })
                .then(function (reg) {
                    // Check for updates every time the page loads
                    reg.update();
                })
                .catch(function () {
                    // SW registration failed — app still works normally
                });
        });
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    function isIos() {
        return /iphone|ipad|ipod/i.test(navigator.userAgent) && !window.MSStream;
    }

    function isInStandaloneMode() {
        return ('standalone' in window.navigator) && window.navigator.standalone;
    }

    function showHamburgerWrapper() {
        var wrapper = document.getElementById('pwaHamburgerWrapper');
        if (wrapper) wrapper.classList.remove('d-none');
    }

    function hideHamburgerWrapper() {
        var wrapper = document.getElementById('pwaHamburgerWrapper');
        if (wrapper) wrapper.classList.add('d-none');
    }

    function openIosModal() {
        var modalEl = document.getElementById('iosInstallModal');
        if (modalEl && typeof bootstrap !== 'undefined') {
            new bootstrap.Modal(modalEl).show();
        }
    }

    // ── iOS: show instructions modal ─────────────────────────────────────────
    if (isIos() && !isInStandaloneMode()) {
        // Show the hamburger install button (tapping it re-opens the modal)
        window.addEventListener('DOMContentLoaded', function () {
            showHamburgerWrapper();

            var hamburgerBtn = document.getElementById('pwaInstallBtnHamburger');
            if (hamburgerBtn) {
                hamburgerBtn.addEventListener('click', openIosModal);
            }

            // Auto-show the modal once per session if not already confirmed
            if (!localStorage.getItem('iosInstallConfirmed')) {
                setTimeout(openIosModal, 3000);
            }

            // Mark as confirmed only when the user taps "Разбрах"
            var confirmBtn = document.getElementById('iosInstallConfirmBtn');
            if (confirmBtn) {
                confirmBtn.addEventListener('click', function () {
                    localStorage.setItem('iosInstallConfirmed', '1');
                });
            }
        });
    }

    // ── Android/Chrome: beforeinstallprompt ──────────────────────────────────
    var deferredPrompt = null;
    var installBtn = document.getElementById('pwaInstallBtn');

    window.addEventListener('beforeinstallprompt', function (e) {
        e.preventDefault();
        deferredPrompt = e;

        // Show the desktop install button
        if (installBtn) {
            installBtn.classList.remove('d-none');
        }

        // Show the mobile hamburger install button
        showHamburgerWrapper();

        var hamburgerBtn = document.getElementById('pwaInstallBtnHamburger');
        if (hamburgerBtn) {
            hamburgerBtn.addEventListener('click', function () {
                if (!deferredPrompt) return;
                deferredPrompt.prompt();
                deferredPrompt.userChoice.then(function () {
                    deferredPrompt = null;
                    hideHamburgerWrapper();
                });
            });
        }
    });

    if (installBtn) {
        installBtn.addEventListener('click', function () {
            if (!deferredPrompt) return;
            deferredPrompt.prompt();
            deferredPrompt.userChoice.then(function () {
                deferredPrompt = null;
                installBtn.classList.add('d-none');
            });
        });
    }

    // Hide all install UI once the app is installed
    window.addEventListener('appinstalled', function () {
        if (installBtn) installBtn.classList.add('d-none');
        deferredPrompt = null;
        hideHamburgerWrapper();
    });
})();
