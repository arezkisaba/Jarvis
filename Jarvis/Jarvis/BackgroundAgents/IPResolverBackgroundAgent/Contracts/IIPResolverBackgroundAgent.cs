namespace Jarvis;

public interface IIPResolverBackgroundAgent
{
    string CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task UpdateCurrentStateAsync();
}
