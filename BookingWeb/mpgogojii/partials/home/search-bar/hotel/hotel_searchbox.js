/**
 * This file I copy from gogojiiuat then replace all $ (dollar sign) with jQuery
 * For prevent error from conflict mode.
 * 1. Change $ => jQuery
 * 2. Change function name
 * 3. Change some control id
 */

jQuery(document).ready(function() {
  //City
  var easyACOptions = {
    url: function(phrase) {
      return (
        mpgogojii.hotel_url +
        "?search=hotel&keyword=" +
        phrase +
        "&language=" +
        jQuery("#hotel-form #lang").val()
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
        var value = jQuery("#hotel-destination").getSelectedItemData().Key;

        jQuery("#hotel-destinationCode").val(value);
      }
    },
    template: {
      type: "custom",
      method: function (value, item) {
	if (item.Key.indexOf('HID') >= 0) {
            return '<img src="' + mpgogojii.template_url+'/assets/img/hotel.svg" style="width: 10px;"> ' + value;
          } else {
            return '<img src="' + mpgogojii.template_url +'/assets/img/pin.svg" style="width: 10px;"> ' + value;
          }
      }
  },
    requestDelay: 0,
    adjustWidth: false
  };
  jQuery("#hotel-destination").easyAutocomplete(easyACOptions);

  //Date Picker
  var today = new Date();
  var defaultDepDateText = jQuery("#hotel-form #checkin")
    .val()
    .split("/");
  var defaultDepDate = new Date(
    parseInt(defaultDepDateText[2]),
    parseInt(defaultDepDateText[1]) - 1,
    parseInt(defaultDepDateText[0])
  );
  var defaultRetDateText = jQuery("#hotel-form #checkout")
    .val()
    .split("/");
  var defaultRetDate = new Date(
    parseInt(defaultRetDateText[2]),
    parseInt(defaultRetDateText[1]) - 1,
    parseInt(defaultRetDateText[0])
  );

  var depDatepicker = jQuery("#hotel-form #checkin").datepicker({
    language: jQuery("#hotel-form #web_lang").val(),
    minDate: addDays(today, 0),
    maxDate: addDays(today, 365),
    autoClose: true,
    toggleSelected: false,
    firstDay: 1,
    onSelect: function onSelect(fd, date) {
      if (retDatepicker) {
        var retDate = retDatepicker.data("datepicker");
        var ret = retDate.selectedDates[0];
        if (date >= ret) {
          retDate.selectDate(addDays(date, 1));
        }
        retDate.update("minDate", addDays(date, 1));

        ret = retDate.selectedDates[0];
        var night = (ret - date) / (24 * 60 * 60 * 1000);
        jQuery('#nights').html(night);
        jQuery('#night-text').html( (night <= 1)? mpgogojii.elkNIGHT:mpgogojii.elkNIGHTS);

      }
    }
  });

  jQuery(".td-datepicker-check-in").on("click", function() {
    jQuery("#hotel-form #checkin").focus();
  });

  jQuery(".td-datepicker-check-out").on("click", function() {
    jQuery("#hotel-form #checkout").focus();
  });

  var depDateDate = depDatepicker.data("datepicker");
  depDateDate.selectDate(defaultDepDate);

  var retDatepicker = jQuery("#hotel-form #checkout").datepicker({
    language: jQuery("#hotel-form #web_lang").val(),
    minDate: defaultDepDate,
    maxDate: addDays(today, 365),
    autoClose: true,
    toggleSelected: false,
        onSelect: function onSelect(fd, date) {
        var depDate = depDatepicker.data('datepicker');
        var dep = depDate.selectedDates[0];
        var night = (date - dep) / (24 * 60 * 60 * 1000);
        jQuery('#nights').html(night);
        jQuery('#night-text').html( (night <= 1)? mpgogojii.elkNIGHT:mpgogojii.elkNIGHTS );
    }
  });
  var retDateDate = retDatepicker.data("datepicker");
  retDateDate.selectDate(defaultRetDate);

  jQuery("body").on("click", function(e) {
    if (
      !jQuery("#hotel-form #navbarDropdown").is(e.target) &&
      jQuery("#hotel-form #navbarDropdown").has(e.target).length === 0 &&
      jQuery("#hotel-form .open").has(e.target).length === 0
    ) {
      jQuery("#hotel-form #navbarDropdown").removeClass("show");
    }
  });

  hotelGetRoomText();
});

var addDays = function(date, days) {
  var result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
};

var hotelGetRoomText = function() {
  var rooms = jQuery("#hotel-form #rooms")
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
  jQuery("#hotel-form #roomNo").html(rooms.length);
  jQuery("#hotel-form #guestNo").html(adt + chd);
};

var hotelSetRoomValue = function() {
  var rooms = "";
  var roomCount = parseInt(jQuery("#hotel-form #roomCount").val());
  for (var i = 1; i <= roomCount; i++) {
    rooms += rooms === "" ? "" : "|";
    rooms += jQuery("#hotel-form #adtRoom" + i).val();
    rooms += "," + jQuery("#hotel-form #chdRoom" + i).val();
    var chdCount = parseInt(jQuery("#hotel-form #chdRoom" + i).val());
    if (chdCount > 0) {
      for (var c = 1; c <= chdCount; c++) {
        rooms +=
          "," +
          jQuery("#hotel-form #chdAge" + i.toString() + c.toString()).val();
      }
    }
  }
  jQuery("#hotel-form #rooms").val(rooms);
};

var hotelUpdateRoom = function() {
  hotelSetRoomValue();
  hotelGetRoomText();
};

jQuery(document).ready(function() {
  jQuery("#hotel-form .minus").click(function() {
    var jQueryinput = jQuery(this)
      .parent()
      .find("input");
    var count = parseInt(jQueryinput.val()) - 1;
    count = count < 1 ? 1 : count;
    jQueryinput.val(count);
    jQueryinput.change();
    hotelSetRoom(count);
    hotelUpdateRoom();
    return false;
  });
  jQuery("#hotel-form .plus").click(function() {
    var jQueryinput = jQuery(this)
      .parent()
      .find("input");
    var count = parseInt(jQueryinput.val()) + 1;
    count = count > 5 ? 5 : count;
    jQueryinput.val(count);
    jQueryinput.change();
    hotelSetRoom(count);
    hotelUpdateRoom();
    return false;
  });
});

var hotelChildChange = function(roomIndex) {
  var chdCount = parseInt(jQuery("#hotel-form #chdRoom" + roomIndex).val());
  if (chdCount === 0) {
    jQuery("#hotel-form #row-age-" + roomIndex).hide();
  } else {
    var i;
    jQuery("#hotel-form #row-age-" + roomIndex).show();
    for (i = 1; i <= chdCount; i++) {
      jQuery("#hotel-form #chdAge" + roomIndex + i.toString()).show();
    }
    for (i = chdCount + 1; i <= 3; i++) {
      jQuery("#hotel-form #chdAge" + roomIndex + i.toString()).hide();
    }
  }
  hotelUpdateRoom();
};

var hotelSetRoom = function(roomCount) {
  for (i = 1; i <= roomCount; i++) {
    jQuery("#hotel-form .room-pax-" + i.toString()).show();
    hotelChildChange(i);
  }
  for (i = roomCount + 1; i <= 5; i++) {
    jQuery("#hotel-form .room-pax-" + i.toString()).hide();
  }
};
