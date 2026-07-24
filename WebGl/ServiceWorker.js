const cacheName = "DefaultCompany-gmtk-game-jam-2026-0.1.0";
const contentToCache = [
    "Build/6c9c6c67483b4331c1cec394f7f99f76.loader.js",
    "Build/29786f2a135b10c3269f011f77ce7ab2.framework.js",
    "Build/8e33aa70b998f87ef151bca1f62a0e28.data",
    "Build/c762aea786d419c88badebe9e04f0188.wasm",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
