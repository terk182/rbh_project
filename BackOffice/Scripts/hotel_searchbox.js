$(document).ready(function () {
    //City
    var easyACOptions = {

        url: function (phrase) {
            return $('#city_url').val() + '?keyword=' + phrase + '&language=en' ;
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

                $("#hotel_BOL_HotelCode").val(value.replace("HID_","")).trigger("change");
            }
        },
        template: {
            type: "custom",
            method: function (value, item) {
                if (item.Key.indexOf('HID') >= 0) {
                    return '<img src="../../Images/hotel.svg" style="width: 10px;"> ' + value;
                } else {
                    return '<img src="../../Images/pin.svg" style="width: 10px;"> ' + value;
                }

            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    $("#destination").easyAutocomplete(easyACOptions);
	
	//City
    var easyACEXTOptions = {

        url: function (phrase) {
            return $('#city_ext_url').val() + '?keyword=' + phrase + '&language=en' ;
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
                var value = $("#destinationEXT").getSelectedItemData().Key;

                $("#hotel_EXT_HotelOID").val(value.replace("HID_","")).trigger("change");
            }
        },
        template: {
            type: "custom",
            method: function (value, item) {
                if (item.Key.indexOf('HID') >= 0) {
                    return '<img src="../../Images/hotel.svg" style="width: 10px;"> ' + value;
                } else {
                    return '<img src="../../Images/pin.svg" style="width: 10px;"> ' + value;
                }

            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    $("#destinationEXT").easyAutocomplete(easyACEXTOptions);

 
 //City
    var easyACHotProOptions = {

        url: function (phrase) {
            return $('#city_url').val() + '?keyword=' + phrase + '&language=en' ;
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
                var value = $("#destinationHot").getSelectedItemData().Key;
				
			    if(value.indexOf('HID') >= 0) {
					$("#hotelSearch_HotelCode").val(value.replace("HID_","")).trigger("change");
					$("#hotelSearch_HotelSource").val(value.replace("HID_","").length>30?"EXT":"BOL").trigger("change");
					$("#hotelSearch_Destination_DestinationCode").val('');
					$("#hotelSearch_Destination_DestinationName").val('');
				}else{
					$("#hotelSearch_Destination_DestinationCode").val(value).trigger("change");
					$("#hotelSearch_Destination_DestinationName").val( $("#destinationHot").val()).trigger("change");
					$("#hotelSearch_HotelCode").val('');
					$("#hotelSearch_HotelSource").val('');
				}
			}
        },
        template: {
            type: "custom",
            method: function (value, item) {
                if (item.Key.indexOf('HID') >= 0) {
                    return '<img src="../../Images/hotel.svg" style="width: 10px;"> ' + value;
                } else {
                    return '<img src="../../Images/pin.svg" style="width: 10px;"> ' + value;
                }

            }
        },
        requestDelay: 0,
        adjustWidth: false
    };
    $("#destinationHot").easyAutocomplete(easyACHotProOptions);
	

	
 //Date Picker
            var today = new Date();
            var defaultDepDateText = $('#HotelCheckIn').val().split('/');
            var defaultDepDate = new Date(parseInt(defaultDepDateText[2]), parseInt(defaultDepDateText[1]) - 1, parseInt(defaultDepDateText[0]));
            console.log(defaultDepDate);
            var defaultRetDateText = $('#HotelCheckOut').val().split('/');
            var defaultRetDate = new Date(parseInt(defaultRetDateText[2]), parseInt(defaultRetDateText[1]) - 1, parseInt(defaultRetDateText[0]));

            var depDatepicker = $('#HotelCheckIn').datepicker({
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
            var retDatepicker = $('#HotelCheckOut').datepicker({
                minDate: today,
                //maxDate: addDays(today, 365),
                language: 'en',
                autoClose: true,
                toggleSelected: false
            });
            var retDateDate = retDatepicker.data('datepicker');
            retDateDate.selectDate(defaultRetDate);
			
       

});

        var addDays = function (date, days) {
            var result = new Date(date);
            result.setDate(result.getDate() + days);
            return result;
        }