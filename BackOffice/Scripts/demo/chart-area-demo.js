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

var  flight_booking_1 = document.getElementById("flight_booking_1").value;
var  flight_booking_2 =  document.getElementById("flight_booking_2").value;
var  flight_booking_3 =  document.getElementById("flight_booking_3").value;
var  flight_booking_4 =  document.getElementById("flight_booking_4").value;
var  flight_booking_5 = document.getElementById("flight_booking_5").value;
var  flight_booking_6 = document.getElementById("flight_booking_6").value;
var  flight_booking_7=  document.getElementById("flight_booking_7").value;
var  flight_booking_8 =  document.getElementById("flight_booking_8").value;
var  flight_booking_9=  document.getElementById("flight_booking_9").value;
var  flight_booking_10 =  document.getElementById("flight_booking_10").value;
var  flight_booking_11 =  document.getElementById("flight_booking_11").value;
var  flight_booking_12 =  document.getElementById("flight_booking_12").value;

var  bundle_booking_1 = document.getElementById("bundle_booking_1").value;
var  bundle_booking_2 =  document.getElementById("bundle_booking_2").value;
var  bundle_booking_3 =  document.getElementById("bundle_booking_3").value;
var  bundle_booking_4 =  document.getElementById("bundle_booking_4").value;
var  bundle_booking_5 = document.getElementById("bundle_booking_5").value;
var  bundle_booking_6 = document.getElementById("bundle_booking_6").value;
var  bundle_booking_7=  document.getElementById("bundle_booking_7").value;
var  bundle_booking_8 =  document.getElementById("bundle_booking_8").value;
var  bundle_booking_9=  document.getElementById("bundle_booking_9").value;
var  bundle_booking_10 =  document.getElementById("bundle_booking_10").value;
var  bundle_booking_11 =  document.getElementById("bundle_booking_11").value;
var  bundle_booking_12 =  document.getElementById("bundle_booking_12").value;

var  transfer_booking_1 = document.getElementById("transfer_booking_1").value;
var  transfer_booking_2 =  document.getElementById("transfer_booking_2").value;
var  transfer_booking_3 =  document.getElementById("transfer_booking_3").value;
var  transfer_booking_4 =  document.getElementById("transfer_booking_4").value;
var  transfer_booking_5 = document.getElementById("transfer_booking_5").value;
var  transfer_booking_6 = document.getElementById("transfer_booking_6").value;
var  transfer_booking_7=  document.getElementById("transfer_booking_7").value;
var  transfer_booking_8 =  document.getElementById("transfer_booking_8").value;
var  transfer_booking_9=  document.getElementById("transfer_booking_9").value;
var  transfer_booking_10 =  document.getElementById("transfer_booking_10").value;
var  transfer_booking_11 =  document.getElementById("transfer_booking_11").value;
var  transfer_booking_12 =  document.getElementById("transfer_booking_12").value;

var myLineChart = new Chart(ctx, {
  type: 'line',
  data: {
    labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    datasets: [{
      label: "Hotel",
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
      label: "Flight",
      lineTension: 0.3,
      backgroundColor: "rgba(28, 200, 138, 0.05)",
      borderColor: "rgba(28, 200, 138, 1)",
      pointRadius: 3,
      pointBackgroundColor: "rgba(28, 200, 138, 1)",
      pointBorderColor: "rgba(28, 200, 138, 1)",
      pointHoverRadius: 3,
      pointHoverBackgroundColor: "rgba(28, 200, 138, 1)",
      pointHoverBorderColor: "rgba(28, 200, 138, 1)",
      pointHitRadius: 10,
      pointBorderWidth: 2,
      data: [flight_booking_1,flight_booking_2,flight_booking_3,flight_booking_4,flight_booking_5,flight_booking_6,flight_booking_7,flight_booking_8,flight_booking_9,flight_booking_10,flight_booking_11,flight_booking_12],
    },
	{
      label: "Bundle",
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
      data: [bundle_booking_1,bundle_booking_2,bundle_booking_3,bundle_booking_4,bundle_booking_5,bundle_booking_6,bundle_booking_7,bundle_booking_8,bundle_booking_9,bundle_booking_10,bundle_booking_11,bundle_booking_12],
    },
	{
      label: "Transfer",
      lineTension: 0.3,
      backgroundColor: "rgba(246, 194, 62, 0.05)",
      borderColor: "rgba(246, 194, 62, 1)",
      pointRadius: 3,
      pointBackgroundColor: "rgba(246, 194, 62, 1)",
      pointBorderColor: "rgba(246, 194, 62, 1)",
      pointHoverRadius: 3,
      pointHoverBackgroundColor: "rgba(246, 194, 62, 1)",
      pointHoverBorderColor: "rgba(246, 194, 62, 1)",
      pointHitRadius: 10,
      pointBorderWidth: 2,
      data: [transfer_booking_1,transfer_booking_2,transfer_booking_3,transfer_booking_4,transfer_booking_5,transfer_booking_6,transfer_booking_7,transfer_booking_8,transfer_booking_9,transfer_booking_10,transfer_booking_11,transfer_booking_12],
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
            return  number_format(value);
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
