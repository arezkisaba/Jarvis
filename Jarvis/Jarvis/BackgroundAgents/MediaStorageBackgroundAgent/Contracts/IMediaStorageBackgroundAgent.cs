namespace Jarvis;

public interface IMediaStorageBackgroundAgent
{
    MediaStorageStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();
}
