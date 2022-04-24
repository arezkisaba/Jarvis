namespace Jarvis;

public interface ICECService
{
    Task SwitchToComputerHDMISourceAsync(
        int hdmiSource);
}
