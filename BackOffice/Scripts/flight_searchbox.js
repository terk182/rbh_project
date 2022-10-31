  $(document).ready(function () {
  //City
    var easyACFlightOptions = {

        url: function (phrase) {
            return $('#flight_city_url').val() + '?keyword=' + phrase + '&language=en';
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
    $("#flight_origin").easyAutocomplete(easyACFlightOptions);
    $("#flight_destination").easyAutocomplete(easyACFlightOptions);
	

    $('#flightSearch_adult').change(function () {
        var adt = parseInt($('#flightSearch_adult').val());
        var chd = parseInt($('#flightSearch_child').val());
        var inf = parseInt($('#flightSearch_infant').val());

        if (adt + chd > 9) {
            chd = 0;
        }

        var maxChd = 9 - adt;
        $('#flightSearch_child').empty();
        $('#flightSearch_child').attr({"max" : maxChd});
        if (inf > adt) {
            inf = 0;
        }
        $('#flightSearch_infant').empty();
        $('#flightSearch_child').attr({"max" : adt});
    });
	//Date Picker
            var today = new Date();
            var defaultDepDateText = $('#departdate').val().split('/');
            var defaultDepDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]));
            console.log(defaultDepDate);
            var defaultRetDateText = $('#returndate').val().split('/');
            var defaultRetDate = new Date(parseInt(defaultRetDateText[2]), parseInt(defaultRetDateText[1]) - 1, parseInt(defaultRetDateText[0]));

            var depDatepicker = $('#departdate').datepicker({
                minDate: today,
                //maxDate: addDays(today, 365),
                language: 'en',
                autoClose: true,
                toggleSelected: false,
                onSelect: function onSelect(fd, date) {
                    if (retDatepicker) {
                        var retDate = retDatepicker.data('datepicker');
                        var ret = retDate.selectedDates[0];
                        //alert(ret);
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
                minDate: today,
                //maxDate: addDays(today, 365),
                language: 'en',
                autoClose: true,
                toggleSelected: false
            });
            var retDateDate = retDatepicker.data('datepicker');
            retDateDate.selectDate(defaultRetDate);

});