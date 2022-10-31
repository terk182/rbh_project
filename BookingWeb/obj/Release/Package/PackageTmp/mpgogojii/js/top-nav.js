jQuery(window).scroll(function() {
  var win = jQuery(window);
  var winH = win.height();

  if (jQuery(this).scrollTop() > winH) {
    jQuery(".menu").addClass("fixed-top");
    jQuery(".container.search-bar").addClass("search-box-fixed");
    jQuery(".nav-tabs-container").addClass("fixed-top fixed-top-nav");
    jQuery(".nav-tabs-container .nav-tabs .nav-item .nav-link").addClass(
      "back-to-top"
    );
    jQuery(".navbar__home .logo-show").removeClass("d-none");
    //jQuery(".navbar-toggler .logo").removeClass("d-none");
    return false;
  } else {
    jQuery(".menu").removeClass("fixed-top");
    jQuery(".container.search-bar").removeClass("search-box-fixed");
    jQuery(".nav-tabs-container").removeClass("fixed-top fixed-top-nav");
    jQuery(".nav-tabs-container .nav-tabs .nav-item .nav-link").removeClass(
      "back-to-top"
    );
    jQuery(".navbar__home .logo-show").addClass("d-none");
    //jQuery(".navbar-toggler .logo").addClass("d-none");
    return false;
  }
});

function screenClass() {
  if (jQuery(window).width() < 1201) {
    jQuery(".user-profile").addClass("mobile-profile");
    jQuery(".user-profile").insertBefore(".nav-item-first");
  } else {
    jQuery(".user-profile").removeClass("mobile-profile");
    jQuery(".user-profile").insertAfter(".nav-item.dropdown.ml-0");
  }
}

// Fire.
screenClass();

// And recheck when window gets resized and refreshed
jQuery(window).bind("load resize", function() {
  screenClass();
});

//Back to top (when tabs are clicked)
jQuery(".back-to-top").live("click", function() {
  jQuery("html, body").animate({ scrollTop: 0 }, 600);
  //return false;
});

jQuery(function() {
  jQuery(".navbar-toggler").on("click", function(e) {
    var menuBtn = jQuery(e.currentTarget);
    if (menuBtn.attr("aria-expanded") === "true") {
      jQuery(this).attr("aria-expanded", "false");
      jQuery(".nav-tabs-container.fixed-top").removeClass("d-none");
    } else {
      jQuery(this).attr("aria-expanded", "true");
      jQuery(".nav-tabs-container.fixed-top").addClass("d-none");
    }
  });
});

jQuery(window).scroll(function() {
  if (
    jQuery(window).scrollTop() + jQuery(window).height() >
    jQuery(document).height() - 200
  ) {
    console.log("scrolled");
    jQuery(".chat-bot").addClass("d-none");
    jQuery("#fb-root").addClass("d-none");
  } else {
    jQuery(".chat-bot").removeClass("d-none");
    jQuery("#fb-root").removeClass("d-none");
  }
});
