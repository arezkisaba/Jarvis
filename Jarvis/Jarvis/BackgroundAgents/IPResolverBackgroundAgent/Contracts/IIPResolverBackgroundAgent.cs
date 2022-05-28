namespace Jarvis.BackgroundAgents.IPResolverBackgroundAgent.Contracts;

public interface IIPResolverBackgroundAgent
{
    string CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task UpdateCurrentStateAsync();
}
