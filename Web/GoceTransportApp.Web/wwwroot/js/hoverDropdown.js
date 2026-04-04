(function () {
    ['profileDropdownWrapper', 'exploreDropdownWrapper'].forEach(function (id) {
        var wrapper = document.getElementById(id);
        if (!wrapper) return;
        var menu = wrapper.querySelector('.dropdown-menu');
        var toggle = wrapper.querySelector('[data-bs-toggle="dropdown"]');
        var hideTimer;
        wrapper.addEventListener('mouseenter', function () {
            clearTimeout(hideTimer);
            menu.classList.add('show');
            if (toggle) toggle.setAttribute('aria-expanded', 'true');
        });
        wrapper.addEventListener('mouseleave', function () {
            hideTimer = setTimeout(function () {
                menu.classList.remove('show');
                if (toggle) toggle.setAttribute('aria-expanded', 'false');
            }, 150);
        });
    });
})();
