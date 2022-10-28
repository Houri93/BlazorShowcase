using System.Diagnostics;

namespace BlazorShowcase.Streamers;

public sealed class WaveGeneratorService : IWaveGeneratorService
{
    public event Action<int, double, double> PointAdded;

    public void BeginGeneratingTask()
    {
        Task.Run(async () =>
        {
            var sw = Stopwatch.StartNew();
            while (true)
            {
                for (int i = 0; i < WaveGeneratorServiceHelpers.WavesCount; i++)
                {
                    var sec = sw.Elapsed.TotalSeconds;
                    var sinValue = Math.Sin(sec * Math.PI) + i;
                    PointAdded?.Invoke(i, sec, sinValue);
                }

                await Task.Delay(WaveGeneratorServiceHelpers.SamplingDelayMs);
            }
        });
    }
}
