using Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.Contracts;
using Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService;
using Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService.Contracts;

namespace Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent;

public class IPResolverBackgroundAgent : IIPResolverBackgroundAgent
{
    private readonly IEnumerable<IIPResolverService> _ipResolverServices;
    private CancellationTokenSource _cancellationTokenSource;

    public string CurrentState { get; set; }

    public event EventHandler StateChanged;

    public IPResolverBackgroundAgent()
    {
        _ipResolverServices = new List<IIPResolverService>()
        {
            new WhatIsMyPublicIPService(),
            new MonIPService()
        };
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
        foreach (var ipResolverService in _ipResolverServices)
        {
            try
            {
                var currentStateTemp = await ipResolverService.GetAsync();
                if (currentStateTemp != CurrentState)
                {
                    StateChanged?.Invoke(currentStateTemp, EventArgs.Empty);
                }

                CurrentState = currentStateTemp;
                break;
            }
            catch (Exception)
            {
                // IGNORE
            }
        }
    }
}