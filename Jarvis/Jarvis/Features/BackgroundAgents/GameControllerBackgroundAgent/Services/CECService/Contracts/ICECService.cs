namespace Jarvis.Features.BackgroundAgents.GameControllerBackgroundAgent.Services.CECService.Contracts;

public interface ICECService
{
    Task SwitchToComputerHDMISourceAsync(
        int hdmiSource);
}
