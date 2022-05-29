using Jarvis.Features.Agents.GameControllerAgent.Models;

namespace Jarvis.Features.Agents.GameControllerAgent.Contracts;

public interface IGameControllerClientAgent
{
    GameControllerClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoopForControllerDetection();

    void StartBackgroundLoopForCommands();

    void StartBackgroundLoopForSoundAndMouseManagement();
}
