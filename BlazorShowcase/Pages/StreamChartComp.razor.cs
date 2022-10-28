using BlazorShowcase.Pages.Graphics;
using BlazorShowcase.Streamers;

using Microsoft.AspNetCore.Components;

namespace BlazorShowcase.Pages;

public sealed partial class StreamChartComp : IDisposable
{
    private readonly Series[] series = new Series[WaveGeneratorServiceHelpers.WavesCount];
    private Chart chart;

    [Inject] IWaveGeneratorService WaveGeneratorService { get; set; }

    protected override void OnInitialized()
    {
        for (int i = 0; i < series.Length; i++)
        {
            series[i] = new()
            {
                Label = $"Phase {i + 1}",
                ShowLine = true,
                PointRadius = 0,
                LineWidth = 1,
            };
        }

        WaveGeneratorService.PointAdded += WaveGeneratorService_PointAdded;
    }

    private void WaveGeneratorService_PointAdded(int waveIndex, double time, double value)
    {
        InvokeAsync(async () => await chart.AddPoint(waveIndex, time, value));
    }

    public void Dispose()
    {
        WaveGeneratorService.PointAdded -= WaveGeneratorService_PointAdded;
    }
}
