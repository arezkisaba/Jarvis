namespace Jarvis.Features.Agents.IPResolverAgent.Contracts;

public interface IIPResolverAgent
{
    string CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task UpdateCurrentStateAsync();
}
