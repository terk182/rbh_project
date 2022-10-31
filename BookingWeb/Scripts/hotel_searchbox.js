$(document).ready(function () {

    $('#searchBtn').click(function () {
        //if ($('#destinationCode').val() === "") {
        //    $('#destination').val('');
        //}

        $('#submit').click();
    });

    //City
    var easyACOptions = {

        url: function (phrase) {
            return $('#city_url').val() + '?keyword=' + phrase + '&language=' + $('#lang').val();
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
        template: {
            type: "custom",
            method: function (value, item) {
                if (item.Key.indexOf('HID') >= 0) {
                    return '<img src="/Images/icon_gogo/hotel.svg" style="width: 10px;"> ' + value;
                } else {
                    return '<img src="/Images/icon_gogo/pin.svg" style="width: 10px;"> ' + value;
                }

            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    $("#destination").easyAutocomplete(easyACOptions);

    //Date Picker
    var today = new Date();
    var defaultDepDateText = $('#checkin').val().split('/');
    var defaultDepDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]));
    console.log(defaultDepDate);
    var defaultRetDateText = $('#checkout').val().split('/');
    var defaultRetDate = new Date(parseInt(defaultRetDateText[2]), parseInt(defaultRetDateText[1]) - 1, parseInt(defaultRetDateText[0]));

    var depDatepicker = $('#checkin').datepicker({
        language: $('#web_lang').val(),
        minDate: addDays(today, 0),
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false,
        onSelect: function onSelect(fd, date) {
            if (retDatepicker) {
                var retDate = retDatepicker.data('datepicker');
                var ret = retDate.selectedDates[0];
                if (date >= ret) {
                    retDate.selectDate(addDays(date, 1));
                }
                retDate.update('minDate', addDays(date, 1));

                ret = retDate.selectedDates[0];
                var night = (ret - date) / (24 * 60 * 60 * 1000);
                $('#nights').html(night);
            }
        }
    });
    var depDateDate = depDatepicker.data('datepicker');
    depDateDate.selectDate(defaultDepDate);

    var retDatepicker = $('#checkout').datepicker({
        language: $('#web_lang').val(),
        minDate: defaultDepDate,
        maxDate: addDays(today, 365),
        autoClose: true,
        toggleSelected: false,
        onSelect: function onSelect(fd, date) {
            var depDate = depDatepicker.data('datepicker');
            var dep = depDate.selectedDates[0];
            var night = (date - dep) / (24 * 60 * 60 * 1000);
            $('#nights').html(night);
        }
    });
    var retDateDate = retDatepicker.data('datepicker');
    retDateDate.selectDate(defaultRetDate);

    $('body').on('click', function (e) {
        if (!$('#navbarDropdown').is(e.target)
            && $('#navbarDropdown').has(e.target).length === 0
            && $('.open').has(e.target).length === 0
        ) {
            $('#navbarDropdown').removeClass('show');
        }
    });

    getRoomText();

    $('.td-check-in').click(function () {
        depDateDate.show();
    });
    $('.td-check-out').click(function () {
        retDateDate.show();
    });
});


var addDays = function (date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
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
