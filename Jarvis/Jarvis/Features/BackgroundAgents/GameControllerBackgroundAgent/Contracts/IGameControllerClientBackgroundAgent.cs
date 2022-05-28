using Jarvis.Features.BackgroundAgents.GameControllerBackgroundAgent.Models;

namespace Jarvis.Features.BackgroundAgents.GameControllerBackgroundAgent.Contracts;

public interface IGameControllerClientBackgroundAgent
{
    GameControllerClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoopForControllerDetection();

    void StartBackgroundLoopForCommands();

    void StartBackgroundLoopForSoundAndMouseManagement();
}
