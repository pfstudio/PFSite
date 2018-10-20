// 图表辅助函数
var utils = {
  colors: {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
  },
  transparentize: function (color, opacity) {
    var alpha = opacity === undefined ? 0.5 : 1 - opacity;
    return Color(color).alpha(alpha).rgbString();
  }
};

// 图表选项
var myChartOptions = {
  responsive: true,
  title: {
    display: true,
    text: '本周签到'
  },
  legend: {
    display: false
  },
  tooltips: {
    position: 'average',
    intersect: false
  },
  scales: {
    xAxes: [{
      scaleLabel: {
        display: true,
        labelString: '日期'
      },
      gridLines: {
        drawOnChartArea: false
      }
    }],
    yAxes: [{
      scaleLabel: {
        display: true,
        labelString: '时长/Hour'
      },
      ticks: {
        suggestedMin: 0,
        suggestedMax: 8
      }
    }]
  }
};