const cacheName = "DefaultCompany-WebAR_Image Tracker-0.1.0";
const contentToCache = [
    "Build/WebAR.loader.js",
    "Build/WebAR.framework.js.br",
    "Build/WebAR.data.br",
    "Build/WebAR.wasm.br",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    // Kích hoạt ngay SW mới, không chờ tab cũ đóng -> tránh chạy build cũ.
    self.skipWaiting();
});

self.addEventListener('activate', function (e) {
    e.waitUntil((async function () {
        // Xóa toàn bộ cache cũ để build mới không bị kẹt file cũ.
        const keys = await caches.keys();
        await Promise.all(keys.filter(k => k !== cacheName).map(k => caches.delete(k)));
        await self.clients.claim();
    })());
});

self.addEventListener('fetch', function (e) {
    // Network-first: luôn ưu tiên lấy bản mới nhất từ mạng,
    // chỉ dùng cache khi offline. Nhờ vậy build mới luôn được áp dụng.
    e.respondWith((async function () {
        try {
            const response = await fetch(e.request);
            const cache = await caches.open(cacheName);
            cache.put(e.request, response.clone());
            return response;
        } catch (err) {
            const cached = await caches.match(e.request);
            if (cached) return cached;
            throw err;
        }
    })());
});
