(function () {
    var html = document.documentElement;

    function applyTheme(theme) {
        html.setAttribute('data-bs-theme', theme);
        document.querySelectorAll('.dark-mode-icon').forEach(function (icon) {
            icon.className = theme === 'dark' ? 'fas fa-sun dark-mode-icon' : 'fas fa-moon dark-mode-icon';
        });
        localStorage.setItem('theme', theme);
    }

    applyTheme(localStorage.getItem('theme') || 'light');

    document.querySelectorAll('.dark-mode-toggle-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            applyTheme(html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark');
        });
    });
})();
