namespace BlazorShowcase.Streamers;

public interface IWaveGeneratorService
{
    event Action<int, double, double> PointAdded;

    void BeginGeneratingTask();
}