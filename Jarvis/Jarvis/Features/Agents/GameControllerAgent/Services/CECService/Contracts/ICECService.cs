namespace Jarvis.Features.Agents.GameControllerAgent.Services.CECService.Contracts;

public interface ICECService
{
    Task SwitchToComputerHDMISourceAsync(
        int hdmiSource);
}
