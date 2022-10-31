jQuery(document).ready(function() {
  jQuery("body").on("click", function(e) {
    if (
      !jQuery("#airport-transfer-form #navbarDropdown").is(e.target) &&
      jQuery("#airport-transfer-form #navbarDropdown").has(e.target).length ===
        0 &&
      jQuery(".open").has(e.target).length === 0
    ) {
      jQuery("#airport-transfer-form #navbarDropdown").removeClass("show");
    }
  });
  jQuery("#airport-transfer-form .dropdown-menu").on(
    "click.bs.dropdown",
    function(e) {
      e.stopPropagation();
      e.preventDefault();
    }
  );
  //City
  var easyFromOptions = {
    url: function(phrase) {
      return (
        mpgogojii.transfer_url +
        "?search=airport_transfer&keyword=" +
        phrase +
        "&language=" +
        jQuery("#airport-transfer-form #lang").val() +
        "&anotherCode="
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
        var value = jQuery(
          "#airport-transfer-form #fromText"
        ).getSelectedItemData().Key;
        var code = value.split("_");
        jQuery("#airport-transfer-form #fromType").val(code[0]);
        jQuery("#airport-transfer-form #fromCode").val(code[1]);
        jQuery("#airport-transfer-form #fromCountry").val(code[2]);
      }
    },
    requestDelay: 0,
    adjustWidth: false
  };
  var easyToOptions = {
    url: function(phrase) {
      return (
        mpgogojii.transfer_url +
        "?search=airport_transfer&keyword=" +
        phrase +
        "&language=" +
        jQuery("#airport-transfer-form #lang").val() +
        "&fromCountry=" +
        jQuery("#airport-transfer-form #fromCountry").val() +
        "&anotherCode=" +
        jQuery("#airport-transfer-form #fromType").val()
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
        var value = jQuery(
          "#airport-transfer-form #toText"
        ).getSelectedItemData().Key;

        var code = value.split("_");
        jQuery("#airport-transfer-form #toType").val(code[0]);
        jQuery("#airport-transfer-form #toCode").val(code[1]);
      },
      template: {
        type: "custom",
        method: function(value, item) {
          if (item.Key[0] === "A") {
            return "<i class='fas fa-plane'></i>" + value;
          } else {
            return "<i class='fas fa-building'></i>" + value;
          }
        }
      }
    },
    requestDelay: 500,
    adjustWidth: false
  };

  jQuery("#airport-transfer-form #fromText").easyAutocomplete(easyFromOptions);
  jQuery("#airport-transfer-form #toText").easyAutocomplete(easyToOptions);

  //Date Picker
  var today = new Date();
  var depDT = jQuery("#airport-transfer-outbound")
    .val()
    .split(" ");
  var defaultDepDateText = depDT[0].split("/");
  var defaultDepTimeText = depDT[1].split(":");
  var defaultDepDate = new Date(
    parseInt(defaultDepDateText[2]),
    parseInt(defaultDepDateText[1]) - 1,
    parseInt(defaultDepDateText[0]),
    parseInt(defaultDepTimeText[0]),
    parseInt(defaultDepTimeText[1]),
    0,
    0
  );

  var retDT = jQuery("#airport-transfer-inbound")
    .val()
    .split(" ");
  var defaultRetDateText = retDT[0].split("/");
  var defaultRetTimeText = retDT[1].split(":");
  var defaultRetDate = new Date(
    parseInt(defaultRetDateText[2]),
    parseInt(defaultRetDateText[1]) - 1,
    parseInt(defaultRetDateText[0]),
    parseInt(defaultRetTimeText[0]),
    parseInt(defaultRetTimeText[1]),
    0,
    0
  );

  var depDatepicker = jQuery("#airport-transfer-outbound").datepicker({
    language: jQuery("#airport-transfer-form #web_lang").val(),
    minDate: airportTransferAddDays(today, 3),
    maxDate: airportTransferAddDays(today, 365),
    autoClose: true,
    toggleSelected: false,
    timepicker: true,
    firstDay: 1,
    dateTimeSeparator: ", ",
    onSelect: function onSelect(fd, date) {
      if (retDatepicker) {
        var retDate = retDatepicker.data("datepicker");
        var ret = retDate.selectedDates[0];
        if (date > ret) {
          var newDate = airportTransferAddDays(date, 2);
          newDate.setHours(10, 0, 0, 0);
          retDate.selectDate(newDate);
        }
        retDate.update("minDate", date);
      }
    }
  });
  var depDateDate = depDatepicker.data("datepicker");
  depDateDate.selectDate(defaultDepDate);

  jQuery(".td-datepicker-outbound").on("click", function() {
    jQuery("#airport-transfer-outbound").focus();
  });

  jQuery(".td-datepicker-inbound").on("click", function() {
    jQuery("#airport-transfer-inbound").focus();
  });

  var minRetDate = new Date(
    parseInt(defaultDepDateText[2]),
    parseInt(defaultDepDateText[1]) - 1,
    parseInt(defaultDepDateText[0]),
    0,
    0,
    0,
    0
  );
  var retDatepicker = jQuery("#airport-transfer-inbound").datepicker({
    language: jQuery("#airport-transfer-form #web_lang").val(),
    minDate: minRetDate,
    maxDate: airportTransferAddDays(today, 365),
    autoClose: true,
    toggleSelected: false,
    timepicker: true,
    firstDay: 1,
    dateTimeSeparator: ", "
  });
  var retDateDate = retDatepicker.data("datepicker");
  retDateDate.selectDate(defaultRetDate);

  jQuery(".airport-transfer-triptype").change(airportTransferChangeTripType);
  airportTransferChangeTripType();
});

var airportTransferChangeTripType = function() {
  if (jQuery("input:radio[name=tripType]:checked").val() == "R") {
    jQuery("#airport-transfer-inbound").show();
    jQuery("#airport-transfer-form #onewaydate").hide();
    jQuery(".date-ret").show();
  } else {
    jQuery("#airport-transfer-inbound").hide();
    jQuery("#airport-transfer-form #onewaydate").show();
    jQuery(".date-ret").hide();
  }
};
jQuery("#airport-transfer-form #adults").change(function() {
  airportTransferTotalPax();
});

jQuery("#airport-transfer-form #children").change(function() {
  airportTransferTotalPax();
});

jQuery("#airport-transfer-form #infant").change(function() {
  airportTransferTotalPax();
});

var airportTransferTotalPax = function() {
  var adt = parseInt(jQuery("#airport-transfer-form #adults").val());
  var chd = parseInt(jQuery("#airport-transfer-form #children").val());
  var inf = parseInt(jQuery("#airport-transfer-form #infants").val());
  var total = adt + chd + inf;
  jQuery("#airport-transfer-form #paxNo").html(total);
};

var airportTransferAddDays = function(date, days) {
  var result = new Date(date);
  result.setDate(result.getDate() + days);
  result.setHours(0, 0, 0, 0);
  return result;
};
