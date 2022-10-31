$(document).ready(function () {

    $('body').on('click', function (e) {
        if (!$('#navbarDropdown').is(e.target)
            && $('#navbarDropdown').has(e.target).length === 0
            && $('.open').has(e.target).length === 0
        ) {
            $('#navbarDropdown').removeClass('show');
        }
    });
    $('.dropdown-menu').on("click.bs.dropdown", function (e) { e.stopPropagation(); e.preventDefault(); });
    //City
    var easyFromOptions = {

        url: function (phrase) {
            return $('#city_url').val() + '?keyword=' + phrase + '&language=' + $('#lang').val() + '&anotherCode=';
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
            onSelectItemEvent: function () {
                var value = $("#fromText").getSelectedItemData().Key;
                var code = value.split('_');
                $("#fromType").val(code[0]);
                $("#fromCode").val(code[1]);
                $("#fromCountry").val(code[2]);
            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    var easyToOptions = {

        url: function (phrase) {
            return $('#city_url').val() + '?keyword=' + phrase + '&language=' + $('#lang').val() + '&fromCountry=' + $('#fromCountry').val() + '&anotherCode=' + $('#fromType').val();
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
            onSelectItemEvent: function () {
                var value = $("#toText").getSelectedItemData().Key;

                var code = value.split('_');
                $("#toType").val(code[0]);
                $("#toCode").val(code[1]);
            },
            template: {
                type: "custom",
                method: function (value, item) {
                    if (item.Key[0] === 'A') {
                        return "<i class='fas fa-plane'></i>" + value;
                    } else {
                        return "<i class='fas fa-building'></i>" + value;
                    }
                }
            }
        },
        requestDelay: 0,
        adjustWidth: false
    };

    $("#fromText").easyAutocomplete(easyFromOptions);
    $("#toText").easyAutocomplete(easyToOptions);

    //Date Picker
    var today = new Date();
    var depDT = $('#outbound').val().split(' ');
    var defaultDepDateText = depDT[0].split('/');
    var defaultDepTimeText = depDT[1].split(':');
    var defaultDepDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]), parseInt(defaultDepTimeText[0]), parseInt(defaultDepTimeText[1]), 0, 0);
    console.log(defaultDepDate);

    var retDT = $('#inbound').val().split(' ');
    var defaultRetDateText = retDT[0].split('/');
    var defaultRetTimeText = retDT[1].split(':');
    var defaultRetDate = new Date(parseInt(defaultRetDateText[2]), parseInt(defaultRetDateText[1]) - 1, parseInt(defaultRetDateText[0]), parseInt(defaultRetTimeText[0]), parseInt(defaultRetTimeText[1]), 0, 0);

    var depDatepicker = $('#outbound').datepicker({
        language: $('#web_lang').val(),
        minDate: addDays(today, 3),
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false,
        timepicker: true,
        onSelect: function onSelect(fd, date) {
            if (retDatepicker) {
                var retDate = retDatepicker.data('datepicker');
                var ret = retDate.selectedDates[0];
                if (date > ret) {
                    var newDate = addDays(date, 2);
                    newDate.setHours(10, 0, 0, 0);
                    retDate.selectDate(newDate);
                }
                retDate.update('minDate', date);
            }
        }
    });
    var depDateDate = depDatepicker.data('datepicker');
    depDateDate.selectDate(defaultDepDate);

    var minRetDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]), 0, 0, 0, 0);
    console.log(defaultDepDate);

    var retDatepicker = $('#inbound').datepicker({
        language: $('#web_lang').val(),
        minDate: minRetDate,
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false,
        timepicker: true
    });
    var retDateDate = retDatepicker.data('datepicker');
    retDateDate.selectDate(defaultRetDate);
   
    $('[type=radio][name=tripType]').change(changeTripType);
    changeTripType();


    $('.date-dep').click(function () {
        depDateDate.show();
    });
    $('.date-ret').click(function () {
        retDateDate.show();
    });
});

var changeTripType = function () {
    if ($('input:radio[name=tripType]:checked').val() == 'R') {
        $("#inbound").show();
        $("#onewaydate").hide();
        $(".date-ret").show();
        
    } else {
        $("#inbound").hide();
        $("#onewaydate").show();
        $(".date-ret").hide();
    }
};
$('#adults').change(function () {
    totalPax();
});

$('#children').change(function () {
    totalPax();
});

$('#infant').change(function () {
    totalPax();
});

var totalPax = function () {
    var adt = parseInt($('#adults').val());
    var chd = parseInt($('#children').val());
    var inf = parseInt($('#infants').val());
    var total = adt + chd + inf;
    $('#paxNo').html(total);
};

var addDays = function (date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    result.setHours(0, 0, 0, 0);
    return result;
};