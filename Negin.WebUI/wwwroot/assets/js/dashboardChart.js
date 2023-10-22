/**
 * ---------------------------------------
 * This demo was created using amCharts 5.
 * 
 * For more information visit:
 * https://www.amcharts.com/
 * 
 * Documentation is available at:
 * https://www.amcharts.com/docs/v5/
 * ---------------------------------------
 */

// Create root element
// https://www.amcharts.com/docs/v5/getting-started/#Root_element
var root = am5.Root.new("chartdiv");


// Set themes
// https://www.amcharts.com/docs/v5/concepts/themes/
root.setThemes([
    am5themes_Animated.new(root)
]);
root.numberFormatter.setAll({
    numberFormat: "#.0a",
    numericFields: ["valueY"],
    bigNumberPrefixes: [
        { "number": 1e+3, "suffix": "K" },
        { "number": 1e+6, "suffix": "M" },
        { "number": 1e+9, "suffix": "B" }
    ],
    strokeOpacity: 0.5
});

// Create chart
// https://www.amcharts.com/docs/v5/charts/xy-chart/
var chart = root.container.children.push(am5xy.XYChart.new(root, {
    //panX: true,
    //panY: true,
    //wheelX: "panX",
    //wheelY: "zoomX",
    pinchZoomX: true
}));

// Add cursor
// https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {}));
cursor.lineX.set("visible", false);


// Create axes
// https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
var xRenderer = am5xy.AxisRendererX.new(root, { minGridDistance: 30 });
xRenderer.labels.template.setAll({
    rotation: -90,
    centerY: am5.p50,
    centerX: am5.p100,
    paddingRight: 15
});

xRenderer.grid.template.setAll({
    location: 1
})


var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
    maxDeviation: 0.3,
    categoryField: "month",
    renderer: xRenderer,
    tooltip: am5.Tooltip.new(root, {})
}));

var legend = chart.children.push(
    am5.Legend.new(root, {
        centerX: am5.p50,
        x: am5.p50,
        y: am5.p100,
        centerY: am5.p50,
    })
);
chart.appear(1000, 100);

