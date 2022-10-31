jQuery(document).ready(function() {
  jQuery("body").on("click", function(e) {
    if (
      !jQuery("#navbarDropdown").is(e.target) &&
      jQuery("#navbarDropdown").has(e.target).length === 0 &&
      jQuery(".open").has(e.target).length === 0
    ) {
      jQuery("#navbarDropdown").removeClass("show");
    }
  });
  jQuery("#flight-form .dropdown-menu").on("click.bs.dropdown", function(e) {
    e.stopPropagation();
    e.preventDefault();
  });
  //City
  var easyACOptions = {
    url: function(phrase) {
      return (
        mpgogojii.flight_url +
        "?search=flight&keyword=" +
        phrase +
        "&language=" +
        jQuery("#lang").val()
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
  jQuery("#flight-origin").easyAutocomplete(easyACOptions);
  jQuery("#flight-destination").easyAutocomplete(easyACOptions);

  //Date Picker
  var today = new Date();
  var defaultDepDateText = jQuery("#departdate")
    .val()
    .split("/");
  var defaultDepDate = new Date(
    parseInt(defaultDepDateText[2]),
    parseInt(defaultDepDateText[1]) - 1,
    parseInt(defaultDepDateText[0])
  );
  console.log(defaultDepDate);
  var defaultRetDateText = jQuery("#returndate")
    .val()
    .split("/");
  var defaultRetDate = new Date(
    parseInt(defaultRetDateText[2]),
    parseInt(defaultRetDateText[1]) - 1,
    parseInt(defaultRetDateText[0])
  );

  var depDatepicker = jQuery("#departdate").datepicker({
    language: jQuery("#web_lang").val(),
    minDate: addDays(today, 3),
    maxDate: addDays(today, 365),
    autoClose: true,
    toggleSelected: false,
    firstDay: 1,
    onSelect: function onSelect(fd, date) {
      if (retDatepicker) {
        var retDate = retDatepicker.data("datepicker");
        var ret = retDate.selectedDates[0];
        if (date > ret) {
          retDate.selectDate(addDays(date, 2));
        }
        retDate.update("minDate", date);
      }
    }
  });
  var depDateDate = depDatepicker.data("datepicker");
  depDateDate.selectDate(defaultDepDate);

  var retDatepicker = jQuery("#returndate").datepicker({
    language: jQuery("#web_lang").val(),
    minDate: defaultDepDate,
    maxDate: addDays(today, 365),
    autoClose: true,
    firstDay: 1,
    toggleSelected: false
  });
  var retDateDate = retDatepicker.data("datepicker");
  retDateDate.selectDate(defaultRetDate);

  jQuery(".td-datepicker-returndate").on("click", function() {
    jQuery("#returndate").focus();
  });

  jQuery(".td-datepicker-departdate").on("click", function() {
    jQuery("#departdate").focus();
  });
  /*
    jQuery('#departdate').pickadate({
        format: 'dd/mm/yyyy',
        formatSubmit: 'dd/mm/yyyy',
        hiddenName: true,
        selectMonths: true,
        selectYears: true,
        min: 3,
        max: 365,
        clear: '',
        onSet: function (dateText) {
            var jQueryinput = jQuery("#returndate").pickadate();
            var picker = jQueryinput.pickadate('picker');
            var newDate = new Date(dateText.select);
            var selectDate = new Date(newDate.valueOf());
            var dep = dateText.select;
            var ret = picker.get('select').pick;
            if (dep <= ret) {
                newDate = new Date(ret);
                selectDate.setDate(newDate.getDate());
            } else {
                selectDate.setDate(newDate.getDate() + 2);
            }
            picker.set({
                'min': newDate,
                'select': selectDate
            });
            jQuery("#onewaydate").val(jQuery("#returndate").val());
        }
    });

    jQuery('#returndate').pickadate({
        format: 'dd/mm/yyyy',
        formatSubmit: 'dd/mm/yyyy',
        hiddenName: true,
        selectMonths: true,
        selectYears: true,
        min: jQuery("#departdate").val(),
        max: 365,
        clear: ''
    });
    */
  jQuery("#spanShowOption").click(function() {
    if (jQuery("#divOption").is(":visible")) {
      jQuery("#spanShowOption").html(jQuery("#SHOW_OPTION").val());
      jQuery("#divOption").slideUp();
    } else {
      jQuery("#spanShowOption").html(jQuery("#HIDE_OPTION").val());
      jQuery("#divOption").slideDown();
    }
  });

  jQuery("#flight-form #adult").change(function() {
    jQuery("#spanADT").html(jQuery("#flight-form #adult").val());
    var adt = parseInt(jQuery("#flight-form #adult").val());
    var chd = parseInt(jQuery("#flight-form #child").val());
    var inf = parseInt(jQuery("#flight-form #infant").val());

    if (adt + chd > 9) {
      chd = 0;
    }

    var maxChd = 9 - adt;
    jQuery("#flight-form #child").empty();
    for (var i = 0; i <= maxChd; i++) {
      jQuery("#flight-form #child").append(
        '<option value="' + i + '">' + i + "</option>"
      );
    }
    jQuery("#flight-form #child").val(chd);
    jQuery("#spanCHD").html(jQuery("#flight-form #child").val());

    if (inf > adt) {
      inf = 0;
    }
    jQuery("#flight-form #infant").empty();
    for (var i = 0; i <= adt; i++) {
      jQuery("#flight-form #infant").append(
        '<option value="' + i + '">' + i + "</option>"
      );
    }
    jQuery("#flight-form #infant").val(inf);
    jQuery("#spanINF").html(jQuery("#flight-form #infant").val());
    flight_totalPax();
  });

  jQuery("#flight-form #child").change(function() {
    jQuery("#spanCHD").html(jQuery("#flight-form #child").val());
    flight_totalPax();
  });

  jQuery("#flight-form #infant").change(function() {
    jQuery("#spanINF").html(jQuery("#flight-form #infant").val());
    flight_totalPax();
  });

  jQuery("#svc_class").change(function() {
    jQuery("#spanClass").html(jQuery("#svc_class option:selected").text());
  });

  jQuery("#airline").change(function() {
    jQuery("#spanAirline").html(jQuery("#airline option:selected").text());
  });

  jQuery("[type=radio][name=triptype]").change(changeTripType);
  changeTripType();
});

var changeTripType = function() {
  if (jQuery("input:radio[name=triptype]:checked").val() == "R") {
    jQuery("#returndate").show();
    jQuery("#onewaydate").hide();
    jQuery(".date-ret").show();
  } else {
    jQuery("#returndate").hide();
    jQuery("#onewaydate").show();
    jQuery(".date-ret").hide();
  }
};

var flight_totalPax = function() {
  var adt = parseInt(jQuery("#flight-form #adult").val());
  var chd = parseInt(jQuery("#flight-form #child").val());
  var inf = parseInt(jQuery("#flight-form #infant").val());
  var total = adt + chd + inf;
  jQuery("#flight-form #paxNo").html(total);
  jQuery("#flight-form #spanPaxNo").html(total);
};
var flight_setOption = function() {
  jQuery("#spanADT").html(jQuery("#flight-form #adult").val());
  jQuery("#spanCHD").html(jQuery("#flight-form #child").val());
  jQuery("#spanINF").html(jQuery("#flight-form #infant").val());
  jQuery("#spanClass").html(jQuery("#svc_class option:selected").text());
  jQuery("#spanAirline").html(jQuery("#airline option:selected").text());
};

var addDays = function(date, days) {
  var result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
};
