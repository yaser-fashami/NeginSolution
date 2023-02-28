"use strict";
var KTDoughnutList = function () {
    var initChart = function (data, element, labels) {
        if (!element) {
            return;
        }
        var config = {
            type: 'doughnut',
            data: {
                datasets: [{
                    data: data,
                    backgroundColor: ['#00A3FF', '#50CD89', '#E4E6EF']
                }],
                labels: labels
            },
            options: {
                chart: {
                    fontFamily: 'inherit'
                },
                borderWidth: 0,
                cutout: '75%',
                cutoutPercentage: 65,
                responsive: true,
                maintainAspectRatio: false,
                title: {
                    display: false
                },
                animation: {
                    animateScale: true,
                    animateRotate: true
                },
                stroke: {
                    width: 0
                },
                tooltips: {
                    enabled: true,
                    intersect: false,
                    mode: 'nearest',
                    bodySpacing: 5,
                    yPadding: 10,
                    xPadding: 10,
                    caretPadding: 0,
                    displayColors: false,
                    backgroundColor: '#20D489',
                    titleFontColor: '#ffffff',
                    cornerRadius: 4,
                    footerSpacing: 0,
                    titleSpacing: 0
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        };

        var myDoughnut = new Chart(element, config);
    }

    // Public methods
    return {
        init: function (data, element, labels) {
            initChart(data, element, labels);
        }
    }
}();
