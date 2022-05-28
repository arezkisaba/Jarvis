using Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Models;

namespace Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Contracts;

public interface IGameControllerClientBackgroundAgent
{
    GameControllerClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoopForControllerDetection();

    void StartBackgroundLoopForCommands();

    void StartBackgroundLoopForSoundAndMouseManagement();
}
