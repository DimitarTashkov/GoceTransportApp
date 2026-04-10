// Service Worker — Goce Transport PWA
const CACHE_VERSION = 'v2';
const STATIC_CACHE = 'goce-static-' + CACHE_VERSION;
const DYNAMIC_CACHE = 'goce-dynamic-' + CACHE_VERSION;

// Static assets to pre-cache on install
const STATIC_ASSETS = [
    '/',
    '/manifest.json',
    '/favicon.ico',
    '/images/icon-192.png',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
    '/lib/jquery/dist/jquery.min.js',
    '/css/site.min.css',
    '/js/site.min.js',
    '/js/darkMode.js',
];

// Pages to cache for offline use
const OFFLINE_PAGES = [
    '/',
    '/schedule/index',
    '/organization/index',
    '/route/index',
    '/home/aboutus',
];

// ── Install: pre-cache static assets ──────────────────────────────────────────
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(STATIC_CACHE).then(cache => {
            return cache.addAll(STATIC_ASSETS).catch(() => {
                // Silently ignore assets that fail to cache
            });
        }).then(() => self.skipWaiting())
    );
});

// ── Activate: remove old caches ───────────────────────────────────────────────
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(
                keys
                    .filter(key => key !== STATIC_CACHE && key !== DYNAMIC_CACHE)
                    .map(key => caches.delete(key))
            );
        }).then(() => self.clients.claim())
    );
});

// ── Fetch: caching strategy ───────────────────────────────────────────────────
self.addEventListener('fetch', event => {
    const { request } = event;
    const url = new URL(request.url);

    // Skip non-GET, cross-origin, SignalR, and API calls
    if (request.method !== 'GET') return;
    if (url.origin !== location.origin) return;
    if (url.pathname.startsWith('/notificationHub')) return;
    if (url.pathname.startsWith('/notification/')) return;
    if (url.pathname.startsWith('/home/nextdepartures')) return;
    if (url.pathname.startsWith('/schedule/search')) return;

    // Static assets → Cache First
    if (isStaticAsset(url.pathname)) {
        event.respondWith(cacheFirst(request));
        return;
    }

    // Page navigations (standalone PWA + browser) → Network First with cache fallback
    // Use request.mode === 'navigate' (more reliable than Accept header check)
    // In standalone mode fetch(request) with navigate mode can fail on Android Chrome;
    // networkFirstWithFallback uses fetch(request.url) to avoid this.
    if (request.mode === 'navigate') {
        event.respondWith(networkFirstWithFallback(request));
        return;
    }

    // Other HTML requests (AJAX partials, etc.) → Network First
    if (request.headers.get('accept')?.includes('text/html')) {
        event.respondWith(networkFirst(request));
        return;
    }

    // Everything else → Network First
    event.respondWith(networkFirst(request));
});

function isStaticAsset(pathname) {
    return pathname.match(/\.(css|js|woff2?|ttf|eot|ico|png|jpg|jpeg|svg|webp|gif)(\?.*)?$/);
}

async function cacheFirst(request) {
    const cached = await caches.match(request);
    if (cached) return cached;

    try {
        const response = await fetch(request);
        if (response.ok) {
            const cache = await caches.open(STATIC_CACHE);
            cache.put(request, response.clone());
        }
        return response;
    } catch {
        return new Response('Offline', { status: 503 });
    }
}

async function networkFirst(request) {
    try {
        const response = await fetch(request);
        if (response.ok) {
            const cache = await caches.open(DYNAMIC_CACHE);
            cache.put(request, response.clone());
        }
        return response;
    } catch {
        const cached = await caches.match(request);
        return cached || new Response('Offline', { status: 503 });
    }
}

async function networkFirstWithFallback(request) {
    try {
        // Use request.url (string) + explicit credentials instead of the raw request object.
        // Passing a navigate-mode request directly to fetch() can silently fail in
        // standalone PWA mode on Android Chrome, causing blank pages / errors for
        // pages like /schedule or /organization.
        const response = await fetch(request.url, { credentials: 'same-origin' });
        if (response.ok) {
            const cache = await caches.open(DYNAMIC_CACHE);
            cache.put(request.url, response.clone());
        }
        return response;
    } catch {
        // Try exact page from cache (keyed by URL string)
        const cached = await caches.match(request.url) || await caches.match(request);
        if (cached) return cached;

        // Fallback to homepage
        const home = await caches.match('/');
        if (home) return home;

        return new Response(offlinePage(), {
            headers: { 'Content-Type': 'text/html; charset=utf-8' }
        });
    }
}

function offlinePage() {
    return `<!DOCTYPE html>
<html lang="bg">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Офлайн — Goce Transport</title>
  <style>
    body { font-family: system-ui, sans-serif; display: flex; align-items: center; justify-content: center;
           min-height: 100vh; margin: 0; background: #f8f9fa; text-align: center; }
    .box { padding: 2rem; max-width: 400px; }
    h1 { color: #0d6efd; font-size: 1.5rem; margin-bottom: .5rem; }
    p  { color: #6c757d; margin-bottom: 1.5rem; }
    button { background: #0d6efd; color: white; border: none; padding: .6rem 1.5rem;
             border-radius: 50px; cursor: pointer; font-size: 1rem; }
  </style>
</head>
<body>
  <div class="box">
    <div style="font-size:3rem;margin-bottom:1rem;">🚌</div>
    <h1>Няма интернет връзка</h1>
    <p>Изглежда сте офлайн. Проверете връзката и опитайте отново.</p>
    <button onclick="location.reload()">Опитай отново</button>
  </div>
</body>
</html>`;
}
