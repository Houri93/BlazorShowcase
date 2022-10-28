using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using System.Drawing;

namespace BlazorShowcase.Pages.Graphics;

public sealed partial class Chart : IAsyncDisposable
{
    private readonly string id = Guid.NewGuid().ToString();
    private bool created = false;

    private static readonly string[] automaticColors = new string[] {
        "418CF0", "FCB441", "DF3A02", "056492", "BFBFBF", "1A3B69", "FFE382",
        "129CDD", "CA6B4B", "005CDB", "F3D288", "506381", "F1B9A8", "E0830A",
        "7893BE",
        "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
    };

    [Inject] IJSRuntime Js { get; set; }
    [Parameter] public Series[] Series { get; set; } = Array.Empty<Series>();
    [Parameter] public bool AutoSetSeriesColors { get; set; }
    [Parameter] public string LabelY { get; set; } = string.Empty;
    [Parameter] public string LabelY2 { get; set; } = string.Empty;
    [Parameter] public string LabelX { get; set; } = string.Empty;
    [Parameter] public bool ShowFrozenToggle { get; set; }
    [Parameter] public bool Frozen { get; set; }
    [Parameter] public bool ShowPointsCount { get; set; }
    [Parameter] public int StreamPointsCount { get; set; }

    protected override void OnInitialized()
    {
        if (AutoSetSeriesColors)
        {
            for (int i = 0; i < Series.Length; i++)
            {
                Series[i].Color = ColorTranslator.FromHtml($"#{automaticColors[i]}");
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Create();
        }
    }

    private async Task Create()
    {
        if (created)
        {
            return;
        }
        await Js.InvokeVoidAsync("chart.create", id, Series, Series.Any(a => a.UseY2), LabelX, LabelY, LabelY2, StreamPointsCount);
        created = true;
    }

    private async Task StreamPointsCountChanged(int value)
    {
        StreamPointsCount = value;

        if (created)
        {
            await Js.InvokeVoidAsync("chart.setStreamPointsCount", id, StreamPointsCount);
        }
    }

    public async Task AddPoint(int seriesIndex, double x, double y)
    {
        if (!created)
        {
            return;
        }

        if (Frozen)
        {
            return;
        }

        await Js.InvokeVoidAsync("chart.addPoint", id, seriesIndex, x, y);
    }

    public async Task AddPoints(int seriesIndex, double[] xs, double[] ys)
    {
        if (!created)
        {
            return;
        }

        if (Frozen)
        {
            return;
        }

        await Js.InvokeVoidAsync("chart.addPoints", id, seriesIndex, xs, ys);
    }

    public async Task ClearSeriesPoints(int seriesIndex)
    {
        if (!created)
        {
            return;
        }

        if (Frozen)
        {
            return;
        }

        await Js.InvokeVoidAsync("chart.clearSeriesPoints", id, seriesIndex);
    }

    public async Task ClearPoints()
    {
        if (!created)
        {
            return;
        }

        await Js.InvokeVoidAsync("chart.clearPoints", id);
    }

    public async ValueTask DisposeAsync()
    {
        created = false;
        //       await Js.InvokeVoidAsync("chart.destroy", id);
    }

}
