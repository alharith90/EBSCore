window.drawOrgChart = (data) => {
    data = JSON.parse(data);
    Highcharts.chart('OrgChart', {
        chart: { inverted: true },
        title: false,
        series: [{
            type: 'organization',
            name: false,
            keys: ['from', 'to'],
            data: data,
            nodes: data,
            colorByPoint: false,
            height: 75,
            levels: [
                { level: 0, color: 'silver', dataLabels: { color: 'black' } },
                { level: 1, color: 'silver', dataLabels: { color: 'black' } },
                { level: 2, color: '#980104', dataLabels: { color: 'white' } },
                { level: 4, color: '#359154', dataLabels: { color: 'white' } }
            ]
        }],
        tooltip: false,
        exporting: {
            allowHTML: true,
            sourceWidth: 800,
            sourceHeight: 600
        }
    });
};
