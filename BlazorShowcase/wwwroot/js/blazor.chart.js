
window.chart = {};

window.chart.create = function (id, series, twoY, labelX, labelY, labelY2, streamPointsCount)
{
    let config = {
        type: "scatter",
        data: { datasets: [] },
        options: {
            parsing: false,
            normalized: true,
            animation: false,
            spanGaps: true,
            interaction: {
                intersect: false,
                mode: 'nearest',
            },
            scales: {
                x: {
                    type: 'linear',
                    display: true,
                    position: 'bottom',
                    grid: { drawOnChartArea: true, },
                    ticks: { stepSize: 0.05, count: 50, },
                    title: { display: true, text: labelX, },

                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    grid: { drawOnChartArea: true, },
                    title: { display: true, text: labelY, },
                },
            },
            plugins: {
                legend: {
                    labels: { usePointStyle: true, },
                },
                tooltip: {
                    usePointStyle: true,
                    callbacks: {
                        label: context =>
                        {
                            let label = context.dataset.label || '';
                            label += `: (${context.label}, ${context.formattedValue})`;
                            return label;
                        },
                    },
                },
            },
        }
    };

    if (twoY)
    {
        config.options.scales.y2 = {
            type: 'linear',
            display: true,
            position: 'right',
            grid: { drawOnChartArea: false, },
            title: { display: true, text: labelY2, },
        };
    }

    for (let i = 0; i < series.length; i++)
    {
        let s = series[i];

        config.data.datasets.push({
            label: s.label,
            borderColor: s.jsColor,
            backgroundColor: s.jsColor,
            data: [],
            showLine: s.showLine,
            borderWidth: s.lineWidth,
            pointRadius: s.pointRadius,
            yAxisID: s.useY2 ? 'y2' : 'y',
            hidden: s.hidden,
        });
    }

    let chart = new Chart(id, config);
    chart.streamPointsCount = streamPointsCount;
}

window.chart.addPoint = function (id, seriesIndex, x, y)
{
    let chart = Chart.getChart(id);
    let data = chart.data.datasets[seriesIndex].data;
    data.push({ x, y });

    if (chart.streamPointsCount > 0)
    {
        while (data.length > chart.streamPointsCount)
        {
            data.shift();
        }
    }

    chart.update();
}

window.chart.addPoints = function (id, seriesIndex, xs, ys)
{
    let chart = Chart.getChart(id);
    let data = chart.data.datasets[seriesIndex].data;

    for (let i = 0; i < xs.length; i++)
    {
        data.push({ x: xs[i], y: ys[i] });
    }

    if (chart.streamPointsCount > 0)
    {
        while (data.length > chart.streamPointsCount)
        {
            data.shift();
        }
    }

    chart.update();
}

window.chart.clearSeriesPoints = function (id, seriesIndex)
{
    let chart = Chart.getChart(id);
    chart.data.datasets[seriesIndex].data = [];
    chart.update();
}

window.chart.clearPoints = function (id)
{
    let chart = Chart.getChart(id);
    let datasets = chart.data.datasets;
    for (let i = 0; i < datasets.length; i++)
    {
        datasets[i].data = [];
    }
    chart.update();
}

window.chart.setStreamPointsCount = function (id, streamPointsCount)
{
    let chart = Chart.getChart(id);
    chart.streamPointsCount = streamPointsCount;
}

window.chart.destroy = function (id)
{
    let chart = Chart.getChart(id);
    chart.destroy();
}