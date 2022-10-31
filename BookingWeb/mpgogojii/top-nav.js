jQuery(window).scroll(function() {
    var win = jQuery(window);
    var winH = win.height();

    if (jQuery(this).scrollTop() > winH) {
        return jQuery(".menu").addClass("fixed-top");
    } else {
        return jQuery(".menu").removeClass("fixed-top");
    }
});

function screenClass() {
    if (jQuery(window).width() < 1201) {
        jQuery(".user-profile").addClass("mobile-profile");
        //jQuery(".nav-register").removeClass("nav-item--sign-up");
        jQuery(".user-profile").insertBefore(".nav-item-first");
    } else {
        jQuery(".user-profile").removeClass("mobile-profile");
        //jQuery(".nav-register").addClass("nav-item--sign-up");
        jQuery(".user-profile").insertAfter(".nav-item.dropdown.ml-0");
    }
}

// Fire.
screenClass();

// And recheck when window gets resized and refreshed
jQuery(window).bind("load resize", function() {
    screenClass();
});