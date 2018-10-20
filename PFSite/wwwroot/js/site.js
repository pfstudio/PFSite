// 个人信息浮窗的触发事件
$('#user-info').popover({
  trigger: 'manual',
  offset: -50,
  html: true,
  title: '个人信息',
  content: $('#popover-content-user-info').html()
}).on('mouseenter', function () {
  var that = this;
  $(this).popover("show");
  $('.popover').on('mouseleave', function () {
    $(that).popover('hide');
    });
  }).on('mouseleave', function () {
    var that = this;
    setTimeout(function () {
      if (!$('.popover:hover').length) {
        $(that).popover('hide');
      }
    }, 100);
  });

// 注册service worker
if ('serviceWorker' in navigator) {
  window.addEventListener('load', function () {
    navigator.serviceWorker.register('/sw.js')
      .then(function (registration) {
        // 注册成功
        console.log('ServiceWorker registration successful with scope: ', registration.scope);
      })
      .catch(function (err) {

        // 注册失败:(
        console.log('ServiceWorker registration failed: ', err);
      });
  });
}