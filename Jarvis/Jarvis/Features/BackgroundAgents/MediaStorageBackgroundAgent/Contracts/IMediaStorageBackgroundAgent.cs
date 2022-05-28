using Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Models;

namespace Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Contracts;

public interface IMediaStorageBackgroundAgent
{
    MediaStorageStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();
}
