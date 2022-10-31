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
var  amadeus_booking_1 = document.getElementById("amadeus_booking_1").value;
var  amadeus_booking_2 =  document.getElementById("amadeus_booking_2").value;
var  amadeus_booking_3 =  document.getElementById("amadeus_booking_3").value;
var  amadeus_booking_4 =  document.getElementById("amadeus_booking_4").value;
var  amadeus_booking_5 = document.getElementById("amadeus_booking_5").value;
var  amadeus_booking_6 = document.getElementById("amadeus_booking_6").value;
var  amadeus_booking_7=  document.getElementById("amadeus_booking_7").value;
var  amadeus_booking_8 =  document.getElementById("amadeus_booking_8").value;
var  amadeus_booking_9=  document.getElementById("amadeus_booking_9").value;
var  amadeus_booking_10 =  document.getElementById("amadeus_booking_10").value;
var  amadeus_booking_11 =  document.getElementById("amadeus_booking_11").value;
var  amadeus_booking_12 =  document.getElementById("amadeus_booking_12").value;

var  kiwi_booking_1 = document.getElementById("kiwi_booking_1").value;
var  kiwi_booking_2 =  document.getElementById("kiwi_booking_2").value;
var  kiwi_booking_3 =  document.getElementById("kiwi_booking_3").value;
var  kiwi_booking_4 =  document.getElementById("kiwi_booking_4").value;
var  kiwi_booking_5 = document.getElementById("kiwi_booking_5").value;
var  kiwi_booking_6 = document.getElementById("kiwi_booking_6").value;
var  kiwi_booking_7=  document.getElementById("kiwi_booking_7").value;
var  kiwi_booking_8 =  document.getElementById("kiwi_booking_8").value;
var  kiwi_booking_9=  document.getElementById("kiwi_booking_9").value;
var  kiwi_booking_10 =  document.getElementById("kiwi_booking_10").value;
var  kiwi_booking_11 =  document.getElementById("kiwi_booking_11").value;
var  kiwi_booking_12 =  document.getElementById("kiwi_booking_12").value;

var myLineChart = new Chart(ctx, {
  type: 'line',
  data: {
    labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
    datasets: [{
      label: "AMADEUS",
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
      data: [amadeus_booking_1,amadeus_booking_2,amadeus_booking_3,amadeus_booking_4,amadeus_booking_5,amadeus_booking_6,amadeus_booking_7,amadeus_booking_8,amadeus_booking_9,amadeus_booking_10,amadeus_booking_11,amadeus_booking_12],
    },
	{
      label: "KIWI",
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
      data: [kiwi_booking_1,kiwi_booking_2,kiwi_booking_3,kiwi_booking_4,kiwi_booking_5,kiwi_booking_6,kiwi_booking_7,kiwi_booking_8,kiwi_booking_9,kiwi_booking_10,kiwi_booking_11,kiwi_booking_12],
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
