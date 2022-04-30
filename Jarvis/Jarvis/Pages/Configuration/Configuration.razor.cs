using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Jarvis.Pages.Configuration;

public partial class Configuration : BlazorPageComponentBase
{
    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

    public SecureAppSettingsViewModel SecureAppSettingsViewModel { get; set; }

    private EditContext editContext;

    protected override async Task OnInitializedAsync()
    {
        PageTitle = "Configuration";
        var secureAppSettings = await SecureAppSettingsService.ReadAsync();
        SecureAppSettingsViewModel = new SecureAppSettingsViewModel(secureAppSettings);
        editContext = new(SecureAppSettingsViewModel);
        ////editContext.OnFieldChanged += OnEditContextOnFieldChanged;
        ////editContext.OnValidationRequested += OnEditContextOnValidationRequested;
    }

    public void Dispose()
    {
        if (editContext is not null)
        {
            ////editContext.OnFieldChanged -= OnEditContextOnFieldChanged;
            ////editContext.OnValidationRequested -= OnEditContextOnValidationRequested;
        }
    }

    private void HandleOnValidSubmit()
    {
    }

    private void OnEditContextOnFieldChanged(
        object sender,
        FieldChangedEventArgs e)
    {
        ////if (editContext is not null)
        ////{
        ////    IsFormInvalid = !editContext.Validate();
        ////    StateHasChanged();
        ////}
    }

    private void OnEditContextOnValidationRequested(
        object sender,
        ValidationRequestedEventArgs e)
    {
    }
}
