// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
var ctx = document.getElementById("myPieChartMember");
var  Facebook = document.getElementById("member_Facebook").value;
var  Gogojii =  document.getElementById("member_Gogojii").value;
var myPieChart = new Chart(ctx, {
  type: 'doughnut',
  data: {
    labels: ["Facebook", "Gogojii"],
    datasets: [{
      data: [Facebook, Gogojii],
      backgroundColor: ['#4e73df', '#f6c23e'],
      hoverBackgroundColor: ['#2e59d9', '#fbb605'],
      hoverBorderColor: "rgba(234, 236, 244, 1)",
    }],
  },
  options: {
    maintainAspectRatio: false,
    tooltips: {
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 10,
    },
    legend: {
      display: false
    },
    cutoutPercentage: 80,
  },
});
