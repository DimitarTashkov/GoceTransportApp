(function () {
    var icon = document.getElementById('darkModeIcon');
    var btn = document.getElementById('darkModeToggle');
    var html = document.documentElement;

    function applyTheme(theme) {
        html.setAttribute('data-bs-theme', theme);
        icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        localStorage.setItem('theme', theme);
    }

    applyTheme(localStorage.getItem('theme') || 'light');

    btn.addEventListener('click', function () {
        applyTheme(html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark');
    });
})();
