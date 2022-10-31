
$(document).ready(function () {

    //City
    var easyACOptions = {

        url: function (phrase) {
            return $('#city_url').val() + '?keyword=' + phrase + '&language=' + $('#lang').val();
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
        requestDelay: 500,
        adjustWidth: false
    };
    $("#origin").easyAutocomplete(easyACOptions);
    $("#destination").easyAutocomplete(easyACOptions);

    //Date Picker
    var today = new Date();
    var defaultDepDateText = $('#departdate').val().split('/');
    var defaultDepDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]));
    console.log(defaultDepDate);
    var defaultRetDateText = $('#returndate').val().split('/');
    var defaultRetDate = new Date(parseInt(defaultRetDateText[2]), parseInt(defaultRetDateText[1]) - 1, parseInt(defaultRetDateText[0]));

    var depDatepicker = $('#departdate').datepicker({
        language: $('#web_lang').val(),
        minDate: addDays(today, 3),
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false,
        onSelect: function onSelect(fd, date) {
            if (retDatepicker) {
                var retDate = retDatepicker.data('datepicker');
                var ret = retDate.selectedDates[0];
                if (date > ret) {
                    retDate.selectDate(addDays(date, 2));
                }
                retDate.update('minDate', date);
            }
        }
    });
    var depDateDate = depDatepicker.data('datepicker');
    depDateDate.selectDate(defaultDepDate);

    var retDatepicker = $('#returndate').datepicker({
        language: $('#web_lang').val(),
        minDate: defaultDepDate,
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false
    });
    var retDateDate = retDatepicker.data('datepicker');
    retDateDate.selectDate(defaultRetDate);
    /*
    $('#departdate').pickadate({
        format: 'dd/mm/yyyy',
        formatSubmit: 'dd/mm/yyyy',
        hiddenName: true,
        selectMonths: true,
        selectYears: true,
        min: 3,
        max: 365,
        clear: '',
        onSet: function (dateText) {
            var $input = $("#returndate").pickadate();
            var picker = $input.pickadate('picker');
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
            $("#onewaydate").val($("#returndate").val());
        }
    });

    $('#returndate').pickadate({
        format: 'dd/mm/yyyy',
        formatSubmit: 'dd/mm/yyyy',
        hiddenName: true,
        selectMonths: true,
        selectYears: true,
        min: $("#departdate").val(),
        max: 365,
        clear: ''
    });
    */
    $('#spanShowOption').click(function () {
        if ($('#divOption').is(":visible")) {
            $('#spanShowOption').html($('#SHOW_OPTION').val());
            $('#divOption').slideUp();
        } else {
            $('#spanShowOption').html($('#HIDE_OPTION').val());
            $('#divOption').slideDown();
        }
    });

    $('#adult').change(function () {
        $('#spanADT').html($('#adult').val());
        var adt = parseInt($('#adult').val());
        var chd = parseInt($('#child').val());
        var inf = parseInt($('#infant').val());

        if (adt + chd > 9) {
            chd = 0;
        }

        var maxChd = 9 - adt;
        $('#child').empty();
        for (var i = 0; i <= maxChd; i++) {
            $('#child').append('<option value="' + i + '">' + i + '</option>');
        }
        $('#child').val(chd);
        $('#spanCHD').html($('#child').val());

        if (inf > adt) {
            inf = 0;
        }
        $('#infant').empty();
        for (var i = 0; i <= adt; i++) {
            $('#infant').append('<option value="' + i + '">' + i + '</option>');
        }
        $('#infant').val(inf);
        $('#spanINF').html($('#infant').val());

    });

    $('#child').change(function () {
        $('#spanCHD').html($('#child').val());
    });

    $('#infant').change(function () {
        $('#spanINF').html($('#infant').val());
    });

    $('#svc_class').change(function () {
        $('#spanClass').html($("#svc_class option:selected").text());
    });

    $('#airline').change(function () {
        $('#spanAirline').html($("#airline option:selected").text());
    });

    $('[type=radio][name=triptype]').change(changeTripType);
    changeTripType();
});

var changeTripType = function () {
    if ($('input:radio[name=triptype]:checked').val() == 'R') {
        $("#returndate").show();
        $("#onewaydate").hide();
    } else {
        $("#returndate").hide();
        $("#onewaydate").show();
    }
};

var totalPax = function () {
    var adt = parseInt($('#adult').val());
    var chd = parseInt($('#child').val());
    var inf = parseInt($('#infant').val());
    var total = adt + chd + inf;
    $('#spanPaxNo').html(total);
};
var setOption = function () {
    $('#spanADT').html($('#adult').val());
    $('#spanCHD').html($('#child').val());
    $('#spanINF').html($('#infant').val());
    $('#spanClass').html($("#svc_class option:selected").text());
    $('#spanAirline').html($("#airline option:selected").text());
};

var addDays = function (date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}