using Lib.Core;
using Lib.Win32;
using SharpDX.XInput;
using WindowsInput;
using WindowsInput.Native;

namespace Jarvis;

public class IPResolverBackgroundAgent : IIPResolverBackgroundAgent
{
    private readonly IIPResolverService _ipResolverService;
    private CancellationTokenSource _cancellationTokenSource;

    public string CurrentState { get; set; }

    public event EventHandler StateChanged;

    public IPResolverBackgroundAgent(
        IIPResolverService ipResolverService)
    {
        _ipResolverService = ipResolverService;
    }

    public void StartBackgroundLoop()
    {
        Task.Run(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

            do
            {
                await UpdateCurrentStateAsync();
            } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));
        });
    }

    public async Task UpdateCurrentStateAsync()
    {
        var currentStateTemp = await _ipResolverService.GetAsync();
        if (currentStateTemp != CurrentState)
        {
            StateChanged?.Invoke(currentStateTemp, EventArgs.Empty);
        }

        CurrentState = currentStateTemp;
    }
}
