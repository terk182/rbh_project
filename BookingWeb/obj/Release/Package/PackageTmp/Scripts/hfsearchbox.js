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
        requestDelay: 0,
        adjustWidth: true
    };
    var easyHCOptions = {

        url: function (phrase) {
            return $('#hotel_city_url').val() + '?hotel=N&keyword=' + phrase + '&language=' + $('#lang').val();
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
                var value = $("#destination").getSelectedItemData().Key;

                $("#destinationCode").val(value);
            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    $("#origin").easyAutocomplete(easyACOptions);
    $("#destination").easyAutocomplete(easyHCOptions);

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
            if (checkinDatepicker) {
                var checkinDate = checkinDatepicker.data('datepicker');
                checkinDate.selectDate(date);
                checkinDate.update('minDate', date);
            }
            if (checkoutDatepicker) {
                var checkoutDate = checkoutDatepicker.data('datepicker');
                var checkout = checkoutDate.selectedDates[0];
                if (date > checkout) {
                    checkoutDate.selectDate(addDays(date, 2));
                }
                checkoutDate.update('minDate', date);
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
        toggleSelected: false,
        onSelect: function onSelect(fd, date) {
            if (checkinDatepicker) {
                var checkinDate = checkinDatepicker.data('datepicker');
                checkinDate.update('maxDate', date);
            }
            if (checkoutDatepicker) {
                var checkoutDate = checkoutDatepicker.data('datepicker');
                checkoutDate.update('maxDate', date);
            }
        }
    });
    var retDateDate = retDatepicker.data('datepicker');
    retDateDate.selectDate(defaultRetDate);


    $('.date-dep').click(function () {
        depDateDate.show();
    });
    $('.date-ret').click(function () {
        retDateDate.show();
    });

    var defaultcheckinDateText = $('#checkin').val().split('/');
    var defaultcheckinDate = new Date(parseInt(defaultcheckinDateText[2]), parseInt(defaultcheckinDateText[1]) - 1, parseInt(defaultcheckinDateText[0]));
    console.log(defaultcheckinDate);
    var defaultcheckoutDateText = $('#checkout').val().split('/');
    var defaultcheckoutDate = new Date(parseInt(defaultcheckoutDateText[2]), parseInt(defaultcheckoutDateText[1]) - 1, parseInt(defaultcheckoutDateText[0]));

    var checkinDatepicker = $('#checkin').datepicker({
        language: $('#web_lang').val(),
        minDate: defaultDepDate,
        maxDate: defaultRetDate,
        autoClose: true,
        toggleSelected: false,
        onSelect: function onSelect(fd, date) {
            if (checkoutDatepicker) {
                var checkoutDate = checkoutDatepicker.data('datepicker');
                var checkout = checkoutDate.selectedDates[0];
                if (date > checkout) {
                    checkoutDate.selectDate(addDays(date, 2));
                }
                checkoutDate.update('minDate', date);
            }
        }
    });
    var checkinDateDate = checkinDatepicker.data('datepicker');
    checkinDateDate.selectDate(defaultcheckinDate);

    var checkoutDatepicker = $('#checkout').datepicker({
        language: $('#web_lang').val(),
        minDate: defaultDepDate,
        maxDate: defaultRetDate,
        autoClose: true,
        toggleSelected: false
    });
    var checkoutDateDate = checkoutDatepicker.data('datepicker');
    checkoutDateDate.selectDate(defaultcheckoutDate);
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
        totalPax();
    });

    $('#child').change(function () {
        $('#spanCHD').html($('#child').val());
        totalPax();
    });

    $('#infant').change(function () {
        $('#spanINF').html($('#infant').val());
        totalPax();
    });

    $('#svc_class').change(function () {
        $('#spanClass').html($("#svc_class option:selected").text());
    });

    $('#airline').change(function () {
        $('#spanAirline').html($("#airline option:selected").text());
    });

});

var totalPax = function () {
    var adt = parseInt($('#adult').val());
    var chd = parseInt($('#child').val());
    var inf = parseInt($('#infant').val());
    var total = adt + chd + inf;
    $('#paxNo').html(total);
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
$(document).ready(function () {
    //City
    
    //Date Picker

    $('body').on('click', function (e) {
        if (!$('#navbarDropdown').is(e.target)
            && $('#navbarDropdown').has(e.target).length === 0
            && $('.open').has(e.target).length === 0
        ) {
            $('#navbarDropdown').removeClass('show');
        }
    });

    $('#diffDate').change(getDiffDate);

    getRoomText();
    getDiffDate();
});

var getDiffDate = function () {
    if (document.getElementById("diffDate").checked) {

        $('#diffDateCal').show();
        $('#differentDate').val('T');
    } else {
        $('#diffDateCal').hide();
        $('#differentDate').val('F');

    }
};

var getRoomText = function () {
    var rooms = $('#rooms').val().split('|');
    var adt = 0; var chd = 0;
    for (var i = 0; i < rooms.length; i++) {
        var content = rooms[i].split(',');
        adt += parseInt(content[0]);
        chd += parseInt(content[1]);
    }

    var txt = rooms.length + ' ' + roomTxt + ', ' + adt + ' ' + adtTxt + ', ' + chd + ' ' + chdTxt;
    $('#roomNo').html(rooms.length);
    $('#guestNo').html(adt + chd);
};

var setRoomValue = function () {
    var rooms = '';
    var roomCount = parseInt($('#roomCount').val());
    for (var i = 1; i <= roomCount; i++) {
        rooms += rooms === '' ? '' : '|';
        rooms += $('#adtRoom' + i).val();
        rooms += ',' + $('#chdRoom' + i).val();
        var chdCount = parseInt($('#chdRoom' + i).val());
        if (chdCount > 0) {
            for (var c = 1; c <= chdCount; c++) {
                rooms += ',' + $('#chdAge' + i.toString() + c.toString()).val();
            }
        }
    }
    $('#rooms').val(rooms);
};

var updateRoom = function () {
    setRoomValue();
    getRoomText();
};

$(document).ready(function () {
    $('.minus').click(function () {
        var $input = $(this).parent().find('input');
        var count = parseInt($input.val()) - 1;
        count = count < 1 ? 1 : count;
        $input.val(count);
        $input.change();
        setRoom(count);
        updateRoom();
        return false;
    });
    $('.plus').click(function () {
        var $input = $(this).parent().find('input');
        var count = parseInt($input.val()) + 1;
        count = count > 5 ? 5 : count;
        $input.val(count);
        $input.change();
        setRoom(count);
        updateRoom();
        return false;
    });
});

var childChange = function (roomIndex) {
    var chdCount = parseInt($('#chdRoom' + roomIndex).val());
    if (chdCount === 0) {
        $('#row-age-' + roomIndex).hide();
    } else {
        var i;
        $('#row-age-' + roomIndex).show();
        for (i = 1; i <= chdCount; i++) {
            $('#chdAge' + roomIndex + i.toString()).show();
        }
        for (i = chdCount + 1; i <= 3; i++) {
            $('#chdAge' + roomIndex + i.toString()).hide();
        }
    }
    updateRoom();
};

var setRoom = function (roomCount) {
    for (i = 1; i <= roomCount; i++) {
        $('.room-pax-' + i.toString()).show();
        childChange(i);
    }
    for (i = roomCount + 1; i <= 5; i++) {
        $('.room-pax-' + i.toString()).hide();
    }
};
