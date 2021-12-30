if ($(window).width() > 1200) {
    console.log($(window).width());
    $(document).ready(function () {
        $("a.fancybox").fancybox({
            clickSlide: 'false',
            autoScale: 'true',
            toolbar: false,
            smallBtn: true,
            iframe: {
                css: { width: "40%", height: "70%" }
            }
        })
    });
}
else if ($(window).width() <= 1200 && $(window).width() > 768) {
    console.log($(window).width());
    $(document).ready(function () {
        $("a.fancybox").fancybox({
            clickSlide: 'false',
            autoScale: 'true',
            toolbar: false,
            smallBtn: true,
            iframe: {
                css: { width: "70%", height: "85%" }
            }
        })
    });
}
else if ($(window).width() <= 768) {
    console.log($(window).width());
    $(document).ready(function () {
        $("a.fancybox").fancybox({
            clickSlide: 'false',
            autoScale: 'true',
            toolbar: false,
            smallBtn: true,
            iframe: {
                css: { width: "90%", height: "90%" }
            }
        })
    });
}
