"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

function showToast(title, message, type) {
    const toastId = "toast_" + Date.now();
    const bgClass = type === "danger"  ? "text-bg-danger"
                  : type === "warning" ? "text-bg-warning"
                  : type === "success" ? "text-bg-success"
                  :                      "text-bg-primary";

    const html = `
        <div id="${toastId}" class="toast align-items-center ${bgClass} border-0 shadow" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="8000">
            <div class="d-flex">
                <div class="toast-body fw-semibold">
                    ${title}<br><span class="fw-normal">${message}</span>
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>`;

    const container = document.getElementById("toastContainer");
    if (!container) return;
    container.insertAdjacentHTML("beforeend", html);
    const toastEl = document.getElementById(toastId);
    bootstrap.Toast.getOrCreateInstance(toastEl).show();
    toastEl.addEventListener("hidden.bs.toast", () => toastEl.remove());
}

// Сценарий 1: Промяна на LiveStatus в разписание
connection.on("ReceiveStatusUpdate", function (scheduleId, newStatus) {
    // Ако пътникът е на страницата на точно това разписание → показва банер
    const scheduleWrapper = document.getElementById('scheduleDetailsWrapper');
    const pageScheduleId = scheduleWrapper ? scheduleWrapper.dataset.scheduleId : null;
    if (pageScheduleId && pageScheduleId === scheduleId) {
        const existing = document.getElementById("liveStatusBanner");
        if (existing) existing.remove();
        const banner = `
            <div id="liveStatusBanner" class="alert alert-danger d-flex align-items-center gap-2 rounded-3 fw-semibold mt-3" role="alert">
                <i class="fas fa-circle-exclamation fa-lg"></i>
                <span>${newStatus}</span>
            </div>`;
        const target = document.querySelector("main") || document.querySelector("section") || document.body;
        target.insertAdjacentHTML("afterbegin", banner);
    }

    const toastType = (newStatus.toLowerCase().includes("анулир") ||
                       newStatus.toLowerCase().includes("cancel"))
                      ? "danger" : "warning";
    showToast("🚌 Промяна в разписание", newStatus, toastType);
});

// Сценарий 2: Нов отзив — само до собственика на организацията
connection.on("ReceiveNewReview", function (organizationName, rating) {
    const stars = "★".repeat(rating) + "☆".repeat(5 - rating);
    showToast("⭐ Нов отзив", stars + " за <strong>" + organizationName + "</strong>", "success");
});

// Сценарий 3: Системна нотификация — само до администраторите
connection.on("ReceiveSystemAlert", function (message) {
    showToast("🔔 Системно известие", message, "primary");
});

// Сценарий 4: Организация добавена в любими
connection.on("ReceiveFavoriteAdded", function (orgName) {
    showToast("❤️ Добавено в любими", "<strong>" + orgName + "</strong> е добавена в любимите ти.", "success");
});

// Сценарий 5: Потвърждение за закупен билет
connection.on("ReceivePurchaseConfirmation", function (fromCity, toCity, orgName) {
    showToast("🎫 Билетът е закупен", fromCity + " → " + toCity + " | " + orgName, "success");
});

// Сценарий 6: Напомняне 30 минути преди потегляне
connection.on("ReceiveDepartureReminder", function (fromCity, toCity) {
    showToast("🚌 Потеглянето е след 30 мин!", fromCity + " → " + toCity, "warning");
});

// Сценарий 7: Persistent notification — обнови камбанката
connection.on("ReceiveNotification", function () {
    loadNotifications();
});

// ─── Persistent Inbox (камбанка) ────────────────────────────────────────────

function loadNotifications() {
    // Камбанката рендира се само за логнати потребители
    if (!document.getElementById('notificationDropdown')) return;

    $.getJSON('/Notification/GetMyNotifications', function (data) {
        var badge = document.getElementById('notifBadge');
        var menu  = document.getElementById('notifDropdownMenu');
        var empty = document.getElementById('notifEmptyState');

        // Изтриваме старите редове (без хедъра и empty state)
        $(menu).find('.notif-item').remove();

        if (!data || data.length === 0) {
            if (badge)  { badge.classList.add('d-none'); badge.textContent = ''; }
            if (empty)  { empty.classList.remove('d-none'); }
            return;
        }

        if (empty)  { empty.classList.add('d-none'); }
        if (badge)  { badge.classList.remove('d-none'); badge.textContent = data.length > 9 ? '9+' : data.length; }

        data.forEach(function (n) {
            var timeAgo = formatTimeAgo(new Date(n.createdOn));
            var href    = n.link ? n.link : '#';
            var li = document.createElement('li');
            li.className = 'notif-item border-bottom';
            li.innerHTML =
                '<a class="dropdown-item py-2 d-flex flex-column gap-1 notif-link" href="' + href + '" data-notif-id="' + n.id + '">' +
                    '<span class="small fw-semibold text-wrap" style="white-space:normal">' + escapeHtml(n.content) + '</span>' +
                    '<span class="text-muted" style="font-size:0.7rem">' + timeAgo + '</span>' +
                '</a>';
            menu.appendChild(li);
        });
    }).fail(function () {
        console.error('[Notifications] Failed to load notifications.');
    });
}

// Кликване върху известие → маркирай като прочетено
$(document).on('click', '.notif-link', function (e) {
    var id   = $(this).data('notif-id');
    var href = $(this).attr('href');

    $.post('/Notification/MarkAsRead', { id: id }, function () {
        loadNotifications();
    });

    if (href && href !== '#') {
        e.preventDefault();
        window.location.href = href;
    }
});

function escapeHtml(text) {
    return $('<div>').text(text).html();
}

function formatTimeAgo(date) {
    var now  = new Date();
    var diff = Math.floor((now - date) / 1000);
    if (diff < 60)   return 'преди малко';
    if (diff < 3600) return 'преди ' + Math.floor(diff / 60) + ' мин';
    if (diff < 86400) return 'преди ' + Math.floor(diff / 3600) + ' ч';
    return 'преди ' + Math.floor(diff / 86400) + ' д';
}

// Зареди при отваряне на страницата
$(document).ready(function () {
    loadNotifications();
});

connection.start().catch(err => console.error("[SignalR] Connection error:", err));
