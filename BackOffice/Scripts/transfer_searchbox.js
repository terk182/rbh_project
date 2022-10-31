$(document).ready(function () {

//City
    var easyFromOptions = {

        url: function (phrase) {
            return $('#transfer_city_url').val() + '?keyword=' + phrase + '&language=en&anotherCode=';
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
                $("#transferSearch_fromType").val(code[0]);
                $("#transferSearch_fromCode").val(code[1]);
                $("#fromCountry").val(code[2]);
            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    var easyToOptions = {

        url: function (phrase) {
            return $('#transfer_city_url').val() + '?keyword=' + phrase + '&language=en&fromCountry=' + $('#fromCountry').val() + '&anotherCode=' + $('#transferSearch_fromType').val();
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
                $("#transferSearch_toType").val(code[0]);
                $("#transferSearch_toCode").val(code[1]);
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
        language: 'en',
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
        language: 'en',
        minDate: minRetDate,
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false,
        timepicker: true
    });
    var retDateDate = retDatepicker.data('datepicker');
    retDateDate.selectDate(defaultRetDate);
   
  
});