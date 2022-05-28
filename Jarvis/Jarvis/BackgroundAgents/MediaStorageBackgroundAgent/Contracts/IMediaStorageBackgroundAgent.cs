using Jarvis.BackgroundAgents.MediaStorageBackgroundAgent.Models;

namespace Jarvis.BackgroundAgents.MediaStorageBackgroundAgent.Contracts;

public interface IMediaStorageBackgroundAgent
{
    MediaStorageStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();
}
