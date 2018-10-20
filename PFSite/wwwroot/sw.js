this.addEventListener('install', function (event) {
  console.log('[ServiceWorker/PFSite] Install');
  event.waitUntil(
    caches.open('pfstudio')
      .then(function (cache) {
        return cache.addAll([
          '/lib/bootstrap/css/bootstrap.css',
          '/lib/font-awesome/css/font-awesome.css',
          '/css/site.css',
          '/lib/jquery/jquery.js',
          '/lib/bootstrap/js/bootstrap.bundle.js',
          '/js/site.js'
        ]);
      })
  );
});

this.addEventListener('fetch', function (event) {
  console.log('Fetch event for ', event.request.url);
  console.log(event);
  event.respondWith(
    caches.match(event.request)
      .then(function (response) {
        if (response) {
          //console.log('Found ', event.request.url, ' in cache');
          return response;
        }
        //console.log('Network request for ', event.request.url);
        return fetch(event.request);
      }).catch(function (error) {
        console.log('Error ', error);
      })
  );
});