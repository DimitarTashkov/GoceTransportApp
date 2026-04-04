function setView(mode) {
    var grid = document.getElementById('gridView');
    var list = document.getElementById('listView');
    var btnGrid = document.getElementById('btnGridView');
    var btnList = document.getElementById('btnListView');
    if (mode === 'list') {
        grid.classList.add('d-none');
        list.classList.remove('d-none');
        btnGrid.classList.remove('active');
        btnList.classList.add('active');
    } else {
        list.classList.add('d-none');
        grid.classList.remove('d-none');
        btnList.classList.remove('active');
        btnGrid.classList.add('active');
    }
    localStorage.setItem('scheduleView', mode);
}

(function () {
    var saved = localStorage.getItem('scheduleView');
    if (saved === 'list') setView('list');
})();
