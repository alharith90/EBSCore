window.RiskHeatmap = window.RiskHeatmap || {};

(function (ns) {
    ns.render = function (elementId, points, matrixSize, highThreshold, mediumThreshold) {
        if (!window.Highcharts || !elementId) {
            console.warn('Highcharts not available for risk heatmap');
            return;
        }
        var target = document.getElementById(elementId);
        if (!target) {
            console.warn('Heatmap target not found: ' + elementId);
            return;
        }

        var data = (points || []).map(function (p) {
            var likelihood = parseInt(p.Likelihood || p.likelihood || p.LIKELIHOOD || 0, 10);
            var impact = parseInt(p.Impact || p.impact || p.IMPACT || 0, 10);
            var count = parseInt(p.Count || p.count || 0, 10);
            return [likelihood - 1, impact - 1, count];
        });

        var maxAxis = matrixSize || 5;
        Highcharts.chart(elementId, {
            chart: {
                type: 'heatmap',
                plotBorderWidth: 1
            },
            title: { text: 'Risk Heatmap' },
            xAxis: {
                title: { text: 'Likelihood' },
                categories: Array.from({ length: maxAxis }, (_, i) => (i + 1).toString())
            },
            yAxis: {
                title: { text: 'Impact' },
                categories: Array.from({ length: maxAxis }, (_, i) => (i + 1).toString()),
                reversed: true
            },
            colorAxis: {
                stops: [
                    [0, '#7cb5ec'],
                    [mediumThreshold / (highThreshold || 1), '#f7a35c'],
                    [1, '#d9534f']
                ]
            },
            legend: {
                align: 'right',
                layout: 'vertical',
                margin: 0,
                verticalAlign: 'top'
            },
            tooltip: {
                pointFormat: 'Impact <b>{point.y + 1}</b><br/>Likelihood <b>{point.x + 1}</b><br/>Count <b>{point.value}</b>'
            },
            series: [{
                borderWidth: 1,
                data: data,
                dataLabels: {
                    enabled: true,
                    color: '#000000'
                }
            }]
        });
    };
})(window.RiskHeatmap);
