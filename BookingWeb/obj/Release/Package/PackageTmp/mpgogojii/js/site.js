jQuery(document).ready(function($) {
    // Datepicker
    $(".air-date-picker").datepicker();
    
    // Reset button
    $('.reset-input').on('click', function(e){
       $(this).parent().find('input[type=text]').val('').focus(); 
    });
    
});

/**
 * Convert Date from 00/00/0000 to 0000-00-00 
 * 
 * @param {type} input
 * @param {type} separatorFrom
 * @param {type} separatorTo
 * @returns {convDateFromYearMonthDayToDayMonthYear.c|@var;separatorTo|String}
 */
function convDateFromDayMonthYearToYearMonthDay(input, separatorFrom = "/", separatorTo = "-"){
    // input 24/12/2019 etc.
    var c = input.split(separatorFrom);
    
    return c[2]+separatorTo+c[1]+separatorTo+c[0];
}

