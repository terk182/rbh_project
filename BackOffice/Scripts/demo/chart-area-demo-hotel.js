// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

function number_format(number, decimals, dec_point, thousands_sep) {
  // *     example: number_format(1234.56, 2, ',', ' ');
  // *     return: '1 234,56'
  number = (number + '').replace(',', '').replace(' ', '');
  var n = !isFinite(+number) ? 0 : +number,
    prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
    sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
    dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
    s = '',
    toFixedFix = function(n, prec) {
      var k = Math.pow(10, prec);
      return '' + Math.round(n * k) / k;
    };
  // Fix for IE parseFloat(0.55).toFixed(0) = 0;
  s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
  if (s[0].length > 3) {
    s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
  }
  if ((s[1] || '').length < prec) {
    s[1] = s[1] || '';
    s[1] += new Array(prec - s[1].length + 1).join('0');
  }
  return s.join(dec);
}

// Area Chart Example
var ctx = document.getElementById("myAreaChart");
var  hotel_booking_1 = document.getElementById("hotel_booking_1").value;
var  hotel_booking_2 =  document.getElementById("hotel_booking_2").value;
var  hotel_booking_3 =  document.getElementById("hotel_booking_3").value;
var  hotel_booking_4 =  document.getElementById("hotel_booking_4").value;
var  hotel_booking_5 = document.getElementById("hotel_booking_5").value;
var  hotel_booking_6 = document.getElementById("hotel_booking_6").value;
var  hotel_booking_7=  document.getElementById("hotel_booking_7").value;
var  hotel_booking_8 =  document.getElementById("hotel_booking_8").value;
var  hotel_booking_9=  document.getElementById("hotel_booking_9").value;
var  hotel_booking_10 =  document.getElementById("hotel_booking_10").value;
var  hotel_booking_11 =  document.getElementById("hotel_booking_11").value;
var  hotel_booking_12 =  document.getElementById("hotel_booking_12").value;

var  hotelExt_booking_1 = document.getElementById("hotelExt_booking_1").value;
var  hotelExt_booking_2 =  document.getElementById("hotelExt_booking_2").value;
var  hotelExt_booking_3 =  document.getElementById("hotelExt_booking_3").value;
var  hotelExt_booking_4 =  document.getElementById("hotelExt_booking_4").value;
var  hotelExt_booking_5 = document.getElementById("hotelExt_booking_5").value;
var  hotelExt_booking_6 = document.getElementById("hotelExt_booking_6").value;
var  hotelExt_booking_7=  document.getElementById("hotelExt_booking_7").value;
var  hotelExt_booking_8 =  document.getElementById("hotelExt_booking_8").value;
var  hotelExt_booking_9=  document.getElementById("hotelExt_booking_9").value;
var  hotelExt_booking_10 =  document.getElementById("hotelExt_booking_10").value;
var  hotelExt_booking_11 =  document.getElementById("hotelExt_booking_11").value;
var  hotelExt_booking_12 =  document.getElementById("hotelExt_booking_12").value;

var myLineChart = new Chart(ctx, {
  type: 'line',
  data: {
    labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    datasets: [{
      label: "Bed Online",
      lineTension: 0.3,
      backgroundColor: "rgba(78, 115, 223, 0.05)",
      borderColor: "rgba(78, 115, 223, 1)",
      pointRadius: 3,
      pointBackgroundColor: "rgba(78, 115, 223, 1)",
      pointBorderColor: "rgba(78, 115, 223, 1)",
      pointHoverRadius: 3,
      pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
      pointHoverBorderColor: "rgba(78, 115, 223, 1)",
      pointHitRadius: 10,
      pointBorderWidth: 2,
      data: [hotel_booking_1,hotel_booking_2,hotel_booking_3,hotel_booking_4,hotel_booking_5,hotel_booking_6,hotel_booking_7,hotel_booking_8,hotel_booking_9,hotel_booking_10,hotel_booking_11,hotel_booking_12],
    },
	{
      label: "Extranet",
      lineTension: 0.3,
      backgroundColor: "rgba(54, 185, 204, 0.05)",
      borderColor: "rgba(54, 185, 204, 1)",
      pointRadius: 3,
      pointBackgroundColor: "rgba(54, 185, 204, 1)",
      pointBorderColor: "rgba(54, 185, 204, 1)",
      pointHoverRadius: 3,
      pointHoverBackgroundColor: "rgba(54, 185, 204, 1)",
      pointHoverBorderColor: "rgba(54, 185, 204, 1)",
      pointHitRadius: 10,
      pointBorderWidth: 2,
      data: [hotelExt_booking_1,hotelExt_booking_2,hotelExt_booking_3,hotelExt_booking_4,hotelExt_booking_5,hotelExt_booking_6,hotelExt_booking_7,hotelExt_booking_8,hotelExt_booking_9,hotelExt_booking_10,hotelExt_booking_11,hotelExt_booking_12],
    }],
  },
  options: {
    maintainAspectRatio: false,
    layout: {
      padding: {
        left: 10,
        right: 25,
        top: 25,
        bottom: 0
      }
    },
    scales: {
      xAxes: [{
        time: {
          unit: 'date'
        },
        gridLines: {
          display: false,
          drawBorder: false
        },
        ticks: {
          maxTicksLimit: 7
        }
      }],
      yAxes: [{
        ticks: {
          maxTicksLimit: 5,
          padding: 10,
          // Include a dollar sign in the ticks
          callback: function(value, index, values) {
            return number_format(value);
          }
        },
        gridLines: {
          color: "rgb(234, 236, 244)",
          zeroLineColor: "rgb(234, 236, 244)",
          drawBorder: false,
          borderDash: [2],
          zeroLineBorderDash: [2]
        }
      }],
    },
    legend: {
      display: false
    },
    tooltips: {
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      titleMarginBottom: 10,
      titleFontColor: '#6e707e',
      titleFontSize: 14,
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      intersect: false,
      mode: 'index',
      caretPadding: 10,
      callbacks: {
        label: function(tooltipItem, chart) {
          var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
          return datasetLabel + ': ' + number_format(tooltipItem.yLabel);
        }
      }
    }
  }
});
