/*===================================================
    Template Scripts
====================================================*/
(function($){ "use strict";

        // Preloader     
        $(window).on('load', function() {
            $('body').addClass('loaded');
        });

    $(document).ready(function() {
        
        // Main Header
        var primaryHeader = $('.primary-header'),
            headerClone = primaryHeader.clone();
        $('.header').after('<div class="sticky-header"></div>');
        $('.sticky-header').html(headerClone);
        var headerSelector = document.querySelector(".sticky-header");
        var triggerPoint = $('.header').height();
        var yOffset = 0;

        $(window).on('scroll', function () {
            yOffset = $(window).scrollTop();
            if (yOffset >= triggerPoint) {
                $('.sticky-header').addClass('sticky-fixed-top');
            } else {
                $('.sticky-header').removeClass('sticky-fixed-top');
            }
        });

        if ($('.primary-header').length) {
            $('.header .primary-header .burger-menu').on("click", function () {
                $(this).toggleClass('menu-open');
                $('.header .header-menu-wrap').slideToggle(300);
            });

            $('.sticky-header .primary-header .burger-menu').on("click", function () {
                $(this).toggleClass('menu-open');
                $('.sticky-header .header-menu-wrap').slideToggle(300);
            });
        }

        $('.header-menu-wrap ul li:has(ul)').each(function () {
            $(this).append('<span class="dropdown-plus"></span>');
            $(this).addClass('dropdown_menu');
        });

        $('.header-menu-wrap .dropdown-plus').on("click", function () {
            $(this).prev('ul').slideToggle(300);
            $(this,).toggleClass('dropdown-open');
            $('.header-menu-wrap ul li:has(ul)').toggleClass('dropdown-open');
        });

        $('.header-menu-wrap .dropdown_menu a').append('<span></span>');

        // Responsive Classes
        function responsiveClasses() {
            var body = $('body');
            if ($(window).width() < 992) {
                body.removeClass('viewport-lg');
                body.addClass('viewport-sm');
            } else {
                body.removeClass('viewport-sm');
                body.addClass('viewport-lg');
            }
        }

        // ResponsiveClasses();
        $(window).on("resize", function () {
            responsiveClasses();
        }).resize();

        // Popup Search Box
        $(function () {
            $('#popup-search-box').removeClass('toggled');

            $('.dl-search-icon').on('click', function (e) {
                e.stopPropagation();
                $('#popup-search-box').toggleClass('toggled');
                $("#popup-search").focus();
            });

            $('#popup-search-box input').on('click', function (e) {
                e.stopPropagation();
            });

            $('#popup-search-box, body').on('click', function () {
                $('#popup-search-box').removeClass('toggled');
            });
        });
        
        //Range Slider
        if( $('body').hasClass('shop') ){
            var slider = document.getElementById("price-range");
            var output = document.getElementById("price-output");
            output.innerHTML = "$" + slider.value;
            slider.oninput = function() {
              output.innerHTML =  "$"+this.value;
            }
        }
       
        // Swiper Slider For Shop
        var swiper = new Swiper(".product-gallary-thumb", {
            spaceBetween: 10,
            slidesPerView: 4,
            freeMode: true,
            watchSlidesProgress: true,
        });
        var swiper2 = new Swiper(".product-gallary", {
            spaceBetween: 10,
            loop: true,
            navigation: {
                nextEl: ".swiper-nav-next",
                prevEl: ".swiper-nav-prev",
            },
            thumbs: {
                swiper: swiper,
            },
        });
        
       //Product Carousel
        const productCarousel = document.querySelector(".product-carousel");
        const swiperOptions = {
            slidesPerView: 4,
            slidesPerGroup: 1,
            loop: true,
            grabCursor: true,
            speed: 500,
            spaceBetween: 20,
            mousewheel: false,
            initialSlide: 2,
            autoplay: {
                delay: 5000
            },
            navigation: {
                nextEl: '.dl-slider-button-next',
                prevEl: '.dl-slider-button-prev'
            },
            pagination: {
                el: '.dl-swiper-pagination',
                clickable: true,
                renderBullet: function (index, className) {
                    return '<span class="' + className + '">'+'<svg class="dl-circle-loader" width="20" height="20" viewBox="0 0 20 20">'+
                      '<circle class="path" cx="10" cy="10" r="5.5" fill="none" transform="rotate(-90 10 10)"'+
                      'stroke-opacity="1" stroke-width="2px"></circle>'+
                      '<circle class="solid-fill" cx="10" cy="10" r="3"></circle>'+
                      '</svg></span>';
                }
            },
            breakpoints: {
                // when window width is >= 320px
                320: {
                  slidesPerView: 1,
                  slidesPerGroup: 1,
                  spaceBetween: 10
                },
                // when window width is >= 767px
                767: {
                  slidesPerView: 2,
                  slidesPerGroup: 1,
                  spaceBetween: 10
                },
                // when window width is >= 1024px
                1024: {
                  slidesPerView: 4,
                  slidesPerGroup: 1,
                  spaceBetween: 20
                }
            }
        };

        new Swiper( productCarousel, swiperOptions );
        
        //Testimonial Carousel
        const testimonialCarousel = document.querySelector(".testimonial-carousel");
        const testimonialOptions = {
            slidesPerView: 4,
            slidesPerGroup: 1,
            loop: true,
            grabCursor: true,
            speed: 500,
            spaceBetween: 10,
            mousewheel: false,
            initialSlide: 2,
            autoplay: {
                delay: 5000
            },
            navigation: {
                nextEl: '.dl-slider-button-next',
                prevEl: '.dl-slider-button-prev'
            },
            pagination: {
                el: '.dl-swiper-pagination',
                clickable: true,
                renderBullet: function (index, className) {
                    return '<span class="' + className + '">'+'<svg class="dl-circle-loader" width="20" height="20" viewBox="0 0 20 20">'+
                      '<circle class="path" cx="10" cy="10" r="5.5" fill="none" transform="rotate(-90 10 10)"'+
                      'stroke-opacity="1" stroke-width="2px"></circle>'+
                      '<circle class="solid-fill" cx="10" cy="10" r="3"></circle>'+
                      '</svg></span>';
                }
            },
            breakpoints: {
                // when window width is >= 320px
                320: {
                  slidesPerView: 1,
                  slidesPerGroup: 1,
                  spaceBetween: 0
                },
                // when window width is >= 767px
                767: {
                  slidesPerView: 2,
                  slidesPerGroup: 1,
                  spaceBetween: 10
                },
                // when window width is >= 1024px
                1024: {
                  slidesPerView: 3,
                  slidesPerGroup: 1,
                  spaceBetween: 10
                }
            }
        };
        
        const testimonialSwiper = new Swiper( testimonialCarousel, testimonialOptions );
        
        // Venobox Active 
        new VenoBox({
            selector: '.img-popup',
            bgcolor: 'transparent',
            numeration: true,
            infinigall: true,
            spinner: 'plane',
        });
        
        // Scrool To Top
        var scrollTop = $("#scroll-top");
        $(window).on('scroll', function() {
            var topPos = $(this).scrollTop();
            if (topPos > 100) {
                $('#scrollup').removeClass('hide');
                $('#scrollup').addClass('show');

            } else {
                $('#scrollup').removeClass('show');
                $('#scrollup').addClass('hide');
            }
        });

        $(scrollTop).on("click", function() {
            $('html, body').animate({
                scrollTop: 0
            }, 50);
            return false;
        });
        
        // Current Year
        var currentYear  = new Date().getFullYear();
        $('#currentYear').append(currentYear);
        
     });

})(jQuery);