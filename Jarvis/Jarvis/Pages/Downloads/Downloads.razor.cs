using Microsoft.AspNetCore.Components;

namespace Jarvis.Pages.Downloads;

public partial class Downloads : BlazorPageComponentBase
{
    [Inject]
    public WeatherForecastService ForecastService { get; set; }

    public WeatherForecast[] forecasts;

    public int CurrentCount { get; set; } = 0;

    public void IncrementCount()
    {
        CurrentCount++;
        _ = InitInternalAsync();
    }

    public async Task InitInternalAsync()
    {
        forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
    }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = "Downloads";
        await InitInternalAsync();
    }

    protected override Task OnAfterRenderAsync(
        bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }
}
