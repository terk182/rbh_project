jQuery(document).ready(function() {
  jQuery("body").on("click", function(e) {
    if (
      !jQuery("#bundle-form #navbarDropdown").is(e.target) &&
      jQuery("#bundle-form #navbarDropdown").has(e.target).length === 0 &&
      jQuery(".open").has(e.target).length === 0
    ) {
      jQuery("#bundle-form #navbarDropdown").removeClass("show");
    }
  });
  jQuery("#bundle-form .dropdown-menu").on("click.bs.dropdown", function(e) {
    e.stopPropagation();
    e.preventDefault();
  });
  //City
  var easyACOptions = {
    url: function(phrase) {
      return (
        mpgogojii.bundle_url +
        "?search=bundle&type=city&keyword=" +
        phrase +
        "&language=" +
        jQuery("#bundle-form #lang").val()
      );
    },

    getValue: "name",
    list: {
      maxNumberOfElements: 20,
      showAnimation: {
        type: "fade",
        time: 400
      },

      hideAnimation: {
        type: "fade",
        time: 400
      }
    },
    requestDelay: 0,
    adjustWidth: true
  };
  var easyHCOptions = {
    url: function(phrase) {
      return (
        mpgogojii.hotel_url +
        "?hotel=N&search=bundle&type=hotel&keyword=" +
        phrase +
        "&language=" +
        jQuery("#bundle-form #lang").val()
      );
    },

    getValue: "Value",
    list: {
      maxNumberOfElements: 20,
      showAnimation: {
        type: "fade",
        time: 400
      },

      hideAnimation: {
        type: "fade",
        time: 400
      },
      onSelectItemEvent: function() {
        var value = jQuery("#bundle-destination").getSelectedItemData().Key;

        jQuery("#bundle-destinationCode").val(value);
      }
    },
    requestDelay: 500,
    adjustWidth: false
  };
  jQuery("#bundle-origin").easyAutocomplete(easyACOptions);
  jQuery("#bundle-destination").easyAutocomplete(easyHCOptions);

  //Date Picker
  var today = new Date();
  var defaultDepDateText = jQuery("#bundle-form #bundle-departdate")
    .val()
    .split("/");
  var defaultDepDate = new Date(
    parseInt(defaultDepDateText[2]),
    parseInt(defaultDepDateText[1]) - 1,
    parseInt(defaultDepDateText[0])
  );
  console.log(defaultDepDate);
  var defaultRetDateText = jQuery("#bundle-form #bundle-returndate")
    .val()
    .split("/");
  var defaultRetDate = new Date(
    parseInt(defaultRetDateText[2]),
    parseInt(defaultRetDateText[1]) - 1,
    parseInt(defaultRetDateText[0])
  );

  var depDatepicker = jQuery("#bundle-form #bundle-departdate").datepicker({
    language: jQuery("#bundle-form #web_lang").val(),
    minDate: addDays(today, 3),
    maxDate: addDays(today, 365),
    autoClose: true,
    firstDay: 1,
    toggleSelected: false,
    onSelect: function onSelect(fd, date) {
      if (retDatepicker) {
        var retDate = retDatepicker.data("datepicker");
        var ret = retDate.selectedDates[0];
        if (date > ret) {
          retDate.selectDate(addDays(date, 2));
        }
        retDate.update("minDate", date);
      }
      if (checkinDatepicker) {
        var checkinDate = checkinDatepicker.data("datepicker");
        checkinDate.selectDate(date);
        checkinDate.update("minDate", date);
      }
      if (checkoutDatepicker) {
        var checkoutDate = checkoutDatepicker.data("datepicker");
        var checkout = checkoutDate.selectedDates[0];
        if (date > checkout) {
          checkoutDate.selectDate(addDays(date, 2));
        }
        checkoutDate.update("minDate", date);
      }
    }
  });
  var depDateDate = depDatepicker.data("datepicker");
  depDateDate.selectDate(defaultDepDate);

  jQuery(".td-datepicker-departdate").on("click", function() {
    jQuery("#bundle-form #bundle-departdate").focus();
  });

  jQuery(".td-datepicker-returndate").on("click", function() {
    jQuery("#bundle-form #bundle-returndate").focus();
  });

  var retDatepicker = jQuery("#bundle-form #bundle-returndate").datepicker({
    language: jQuery("#bundle-form #web_lang").val(),
    minDate: defaultDepDate,
    maxDate: addDays(today, 365),
    autoClose: true,
    toggleSelected: false,
    onSelect: function onSelect(fd, date) {
      if (checkinDatepicker) {
        var checkinDate = checkinDatepicker.data("datepicker");
        checkinDate.update("maxDate", date);
      }
      if (checkoutDatepicker) {
        var checkoutDate = checkoutDatepicker.data("datepicker");
        checkoutDate.update("maxDate", date);
      }
    }
  });
  var retDateDate = retDatepicker.data("datepicker");
  retDateDate.selectDate(defaultRetDate);

  var defaultcheckinDateText = jQuery("#bundle-form #checkin")
    .val()
    .split("/");
  var defaultcheckinDate = new Date(
    parseInt(defaultcheckinDateText[2]),
    parseInt(defaultcheckinDateText[1]) - 1,
    parseInt(defaultcheckinDateText[0])
  );
  console.log(defaultcheckinDate);
  var defaultcheckoutDateText = jQuery("#bundle-form #checkout")
    .val()
    .split("/");
  var defaultcheckoutDate = new Date(
    parseInt(defaultcheckoutDateText[2]),
    parseInt(defaultcheckoutDateText[1]) - 1,
    parseInt(defaultcheckoutDateText[0])
  );

  var checkinDatepicker = jQuery("#bundle-form #checkin").datepicker({
    language: jQuery("#bundle-form #web_lang").val(),
    minDate: defaultDepDate,
    maxDate: defaultRetDate,
    autoClose: true,
    firstDay: 1,
    toggleSelected: false,
    onSelect: function onSelect(fd, date) {
      if (checkoutDatepicker) {
        var checkoutDate = checkoutDatepicker.data("datepicker");
        var checkout = checkoutDate.selectedDates[0];
        if (date > checkout) {
          checkoutDate.selectDate(addDays(date, 2));
        }
        checkoutDate.update("minDate", date);
      }
    }
  });
  var checkinDateDate = checkinDatepicker.data("datepicker");
  checkinDateDate.selectDate(defaultcheckinDate);

  var checkoutDatepicker = jQuery("#bundle-form #checkout").datepicker({
    language: jQuery("#bundle-form #web_lang").val(),
    minDate: defaultDepDate,
    maxDate: defaultRetDate,
    autoClose: true,
    firstDay: 1,
    toggleSelected: false
  });
  var checkoutDateDate = checkoutDatepicker.data("datepicker");
  checkoutDateDate.selectDate(defaultcheckoutDate);
  jQuery("#bundle-form #spanShowOption").click(function() {
    if (jQuery("#bundle-form #divOption").is(":visible")) {
      jQuery("#bundle-form #spanShowOption").html(
        jQuery("#bundle-form #SHOW_OPTION").val()
      );
      jQuery("#bundle-form #divOption").slideUp();
    } else {
      jQuery("#bundle-form #spanShowOption").html(
        jQuery("#bundle-form #HIDE_OPTION").val()
      );
      jQuery("#bundle-form #divOption").slideDown();
    }
  });

  jQuery("#bundle-form #adult").change(function() {
    jQuery("#bundle-form #spanADT").html(jQuery("#bundle-form #adult").val());
    var adt = parseInt(jQuery("#bundle-form #adult").val());
    var chd = parseInt(jQuery("#bundle-form #child").val());
    var inf = parseInt(jQuery("#bundle-form #infant").val());

    if (adt + chd > 9) {
      chd = 0;
    }

    var maxChd = 9 - adt;
    jQuery("#bundle-form #child").empty();
    for (var i = 0; i <= maxChd; i++) {
      jQuery("#bundle-form #child").append(
        '<option value="' + i + '">' + i + "</option>"
      );
    }
    jQuery("#bundle-form #child").val(chd);
    jQuery("#bundle-form #spanCHD").html(jQuery("#bundle-form #child").val());

    if (inf > adt) {
      inf = 0;
    }
    jQuery("#bundle-form #infant").empty();
    for (var i = 0; i <= adt; i++) {
      jQuery("#bundle-form #infant").append(
        '<option value="' + i + '">' + i + "</option>"
      );
    }
    jQuery("#bundle-form #infant").val(inf);
    jQuery("#bundle-form #spanINF").html(jQuery("#bundle-form #infant").val());
    totalPax();
  });

  jQuery("#bundle-form #child").change(function() {
    jQuery("#bundle-form #spanCHD").html(jQuery("#bundle-form #child").val());
    totalPax();
  });

  jQuery("#bundle-form #infant").change(function() {
    jQuery("#bundle-form #spanINF").html(jQuery("#bundle-form #infant").val());
    totalPax();
  });

  jQuery("#bundle-form #svc_class").change(function() {
    jQuery("#bundle-form #spanClass").html(
      jQuery("#svc_class option:selected").text()
    );
  });

  jQuery("#bundle-form #airline").change(function() {
    jQuery("#bundle-form #spanAirline").html(
      jQuery("#airline option:selected").text()
    );
  });
});

var totalPax = function() {
  var adt = parseInt(jQuery("#bundle-form #adult").val());
  var chd = parseInt(jQuery("#bundle-form #child").val());
  var inf = parseInt(jQuery("#bundle-form #infant").val());
  var total = adt + chd + inf;
  jQuery("#bundle-form #paxNo").html(total);
  jQuery("#bundle-form #spanPaxNo").html(total);
};
var setOption = function() {
  jQuery("#bundle-form #spanADT").html(jQuery("#bundle-form #adult").val());
  jQuery("#bundle-form #spanCHD").html(jQuery("#bundle-form #child").val());
  jQuery("#bundle-form #spanINF").html(jQuery("#bundle-form #infant").val());
  jQuery("#bundle-form #spanClass").html(
    jQuery("#svc_class option:selected").text()
  );
  jQuery("#bundle-form #spanAirline").html(
    jQuery("#airline option:selected").text()
  );
};

var addDays = function(date, days) {
  var result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
};
jQuery(document).ready(function() {
  //City

  //Date Picker

  jQuery("body").on("click", function(e) {
    if (
      !jQuery("#bundle-form #navbarDropdown").is(e.target) &&
      jQuery("#bundle-form #navbarDropdown").has(e.target).length === 0 &&
      jQuery("#bundle-form .open").has(e.target).length === 0
    ) {
      jQuery("#bundle-form #navbarDropdown").removeClass("show");
    }
  });

  jQuery("#bundle-form #diffDate").change(getDiffDate);

  getRoomText();
  getDiffDate();
});

var getDiffDate = function() {
  if (document.getElementById("diffDate").checked) {
    jQuery("#bundle-form #diffDateCal").show();
    jQuery("#bundle-form #differentDate").val("T");
  } else {
    jQuery("#bundle-form #diffDateCal").hide();
    jQuery("#bundle-form #differentDate").val("F");
  }
};

var getRoomText = function() {
  var rooms = jQuery("#bundle-form #rooms")
    .val()
    .split("|");
  var adt = 0;
  var chd = 0;
  for (var i = 0; i < rooms.length; i++) {
    var content = rooms[i].split(",");
    adt += parseInt(content[0]);
    chd += parseInt(content[1]);
  }

  var txt =
    rooms.length +
    " " +
    roomTxt +
    ", " +
    adt +
    " " +
    adtTxt +
    ", " +
    chd +
    " " +
    chdTxt;
  jQuery("#bundle-form #roomNo").html(rooms.length);
  jQuery("#bundle-form #guestNo").html(adt + chd);
};

var setRoomValue = function() {
  var rooms = "";
  var roomCount = parseInt(jQuery("#bundle-form #roomCount").val());
  for (var i = 1; i <= roomCount; i++) {
    rooms += rooms === "" ? "" : "|";
    rooms += jQuery("#bundle-form #adtRoom" + i).val();
    rooms += "," + jQuery("#bundle-form #chdRoom" + i).val();
    var chdCount = parseInt(jQuery("#bundle-form #chdRoom" + i).val());
    if (chdCount > 0) {
      for (var c = 1; c <= chdCount; c++) {
        rooms +=
          "," +
          jQuery("#bundle-form #chdAge" + i.toString() + c.toString()).val();
      }
    }
  }
  jQuery("#bundle-form #rooms").val(rooms);
};

var updateRoom = function() {
  setRoomValue();
  getRoomText();
};

jQuery(document).ready(function() {
  jQuery("#bundle-form .minus").click(function() {
    var $input = jQuery(this)
      .parent()
      .find("input");
    var count = parseInt($input.val()) - 1;
    count = count < 1 ? 1 : count;
    $input.val(count);
    $input.change();
    setRoom(count);
    updateRoom();
    return false;
  });
  jQuery("#bundle-form .plus").click(function() {
    var $input = jQuery(this)
      .parent()
      .find("input");
    var count = parseInt($input.val()) + 1;
    count = count > 5 ? 5 : count;
    $input.val(count);
    $input.change();
    setRoom(count);
    updateRoom();
    return false;
  });
});

var childChange = function(roomIndex) {
  var chdCount = parseInt(jQuery("#bundle-form #chdRoom" + roomIndex).val());
  if (chdCount === 0) {
    jQuery("#bundle-form #row-age-" + roomIndex).hide();
  } else {
    var i;
    jQuery("#bundle-form #row-age-" + roomIndex).show();
    for (i = 1; i <= chdCount; i++) {
      jQuery("#bundle-form #chdAge" + roomIndex + i.toString()).show();
    }
    for (i = chdCount + 1; i <= 3; i++) {
      jQuery("#bundle-form #chdAge" + roomIndex + i.toString()).hide();
    }
  }
  updateRoom();
};

var setRoom = function(roomCount) {
  for (i = 1; i <= roomCount; i++) {
    jQuery("#bundle-form .room-pax-" + i.toString()).show();
    childChange(i);
  }
  for (i = roomCount + 1; i <= 5; i++) {
    jQuery("#bundle-form .room-pax-" + i.toString()).hide();
  }
};
