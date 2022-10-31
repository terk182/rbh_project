$(document).ready(function () {

  
    //City
    var easyACBundleOptions = {

        url: function (phrase) {
            return $('#bundle_city_url').val() + '?keyword=' + phrase + '&language=en';
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
    var easyHCBundleOptions = {

        url: function (phrase) {
            return $('#bundle_hotel_city_url').val() + '?hotel=N&keyword=' + phrase + '&language=en' ;
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
                var value = $("#bundle_destination").getSelectedItemData().Key;

                $("#bundleSearch_destinationCode").val(value);
            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    $("#bundle_depart").easyAutocomplete(easyACBundleOptions);
    $("#bundle_destination").easyAutocomplete(easyHCBundleOptions);
	
	//Date Picker
            var today = new Date();
            var defaultDepDateText = $('#bundle_departdate').val().split('/');
            var defaultDepDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]));
            console.log(defaultDepDate);
            var defaultRetDateText = $('#bundle_returndate').val().split('/');
            var defaultRetDate = new Date(parseInt(defaultRetDateText[2]), parseInt(defaultRetDateText[1]) - 1, parseInt(defaultRetDateText[0]));

            var depDatepicker = $('#bundle_departdate').datepicker({
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
            var retDatepicker = $('#bundle_returndate').datepicker({
                minDate: today,
                //maxDate: addDays(today, 365),
                language: 'en',
                autoClose: true,
                toggleSelected: false
            });
            var retDateDate = retDatepicker.data('datepicker');
            retDateDate.selectDate(defaultRetDate);

});