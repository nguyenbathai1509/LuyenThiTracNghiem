function changeVideo(url, title) {
    document.getElementById('mainVideo').src = url;
    document.getElementById('mainVideoTitle').innerText = title;
}

$(document).ready(function(){
    $('.courses-slider').slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        arrows: false,   // tắt mũi tên
        dots: false,     // tắt chấm tròn
        infinite: true,
        autoplay: true,  // tự chạy
        autoplaySpeed: 2500, // thời gian giữa các slide (ms)
        speed: 800,      // tốc độ chuyển slide
        responsive: [
            {
                breakpoint: 1200,
                settings: { slidesToShow: 3 }
            },
            {
                breakpoint: 992,
                settings: { slidesToShow: 2 }
            },
            {
                breakpoint: 576,
                settings: { slidesToShow: 1 }
            }
        ]
    });
});