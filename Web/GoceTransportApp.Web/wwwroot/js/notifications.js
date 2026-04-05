"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

var isBg = document.documentElement.lang === 'bg';

function t(en, bg) {
    return isBg ? bg : en;
}

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

// Scenario 1: Schedule live status change — only show to users on that schedule's details page
connection.on("ReceiveStatusUpdate", function (scheduleId, newStatus) {
    const scheduleWrapper = document.getElementById('scheduleDetailsWrapper');
    const pageScheduleId = scheduleWrapper ? scheduleWrapper.dataset.scheduleId : null;
    if (!pageScheduleId || pageScheduleId !== scheduleId) return;

    const existing = document.getElementById("liveStatusBanner");
    if (existing) existing.remove();
    const banner = `
        <div id="liveStatusBanner" class="alert alert-danger d-flex align-items-center gap-2 rounded-3 fw-semibold mt-3" role="alert">
            <i class="fas fa-circle-exclamation fa-lg"></i>
            <span>${newStatus}</span>
        </div>`;
    const target = document.querySelector("main") || document.querySelector("section") || document.body;
    target.insertAdjacentHTML("afterbegin", banner);

    const toastType = (newStatus.toLowerCase().includes("анулир") ||
                       newStatus.toLowerCase().includes("cancel"))
                      ? "danger" : "warning";
    showToast(t("🚌 Schedule Update", "🚌 Промяна в разписание"), newStatus, toastType);
});

// Scenario 2: New review — only to organization owner
connection.on("ReceiveNewReview", function (organizationName, rating) {
    const stars = "★".repeat(rating) + "☆".repeat(5 - rating);
    showToast(t("⭐ New Review", "⭐ Нов отзив"), stars + " " + t("for", "за") + " <strong>" + organizationName + "</strong>", "success");
});

// Scenario 3: System notification — only to admins
connection.on("ReceiveSystemAlert", function (message) {
    showToast(t("🔔 System Notification", "🔔 Системно известие"), message, "primary");
});

// Scenario 4: Organization added to favorites
connection.on("ReceiveFavoriteAdded", function (orgName) {
    showToast(t("❤️ Added to Favorites", "❤️ Добавено в любими"), "<strong>" + orgName + "</strong> " + t("has been added to your favorites.", "е добавена в любимите ти."), "success");
});

// Scenario 5: Ticket purchase confirmation
connection.on("ReceivePurchaseConfirmation", function (fromCity, toCity, orgName) {
    showToast(t("🎫 Ticket Purchased", "🎫 Билетът е закупен"), fromCity + " → " + toCity + " | " + orgName, "success");
});

// Scenario 6: Departure reminder 30 minutes before
connection.on("ReceiveDepartureReminder", function (fromCity, toCity) {
    showToast(t("🚌 Departure in 30 min!", "🚌 Потеглянето е след 30 мин!"), fromCity + " → " + toCity, "warning");
});

// Scenario 7: Persistent notification — update bell
connection.on("ReceiveNotification", function () {
    loadNotifications();
});

// ─── Persistent Inbox (bell) ────────────────────────────────────────────────

function loadNotifications() {
    if (!document.getElementById('notificationDropdown')) return;

    $.getJSON('/Notification/GetMyNotifications', function (data) {
        var badge = document.getElementById('notifBadge');
        var menu  = document.getElementById('notifDropdownMenu');
        var empty = document.getElementById('notifEmptyState');

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

// Click on notification → mark as read
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
    if (diff < 60)    return t('just now', 'преди малко');
    if (diff < 3600)  return t(Math.floor(diff / 60) + ' min ago', 'преди ' + Math.floor(diff / 60) + ' мин');
    if (diff < 86400) return t(Math.floor(diff / 3600) + ' h ago',  'преди ' + Math.floor(diff / 3600) + ' ч');
    return t(Math.floor(diff / 86400) + ' d ago', 'преди ' + Math.floor(diff / 86400) + ' д');
}

// Load on page open
$(document).ready(function () {
    loadNotifications();
});

connection.start().catch(err => console.error("[SignalR] Connection error:", err));
