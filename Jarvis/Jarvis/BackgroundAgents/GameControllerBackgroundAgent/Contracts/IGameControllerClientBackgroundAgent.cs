namespace Jarvis;

public interface IGameControllerClientBackgroundAgent
{
    GameControllerClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoopForControllerDetection();

    void StartBackgroundLoopForCommands();

    void StartBackgroundLoopForSoundAndMouseManagement();
}
