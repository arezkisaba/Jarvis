using Jarvis.Features.Agents.MediaStorageAgent.Models;

namespace Jarvis.Features.Agents.MediaStorageAgent.Contracts;

public interface IMediaStorageAgent
{
    MediaStorageStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();
}
