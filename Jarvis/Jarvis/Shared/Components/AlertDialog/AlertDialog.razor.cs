using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.AlertDialog;

public partial class AlertDialog : BlazorComponentBase
{
    [Parameter]
    public bool IsDismissable { get; set; }

    [Parameter]
    public string Body { get; set; }

    [Parameter]
    public AlertDialogType Type { get; set; }

    public string TypeCssClass { get; set; }

    public string VisibilityCssClass { get; set; } = "show d-none";

    protected override void OnInitialized()
    {
        switch (Type)
        {
            case AlertDialogType.Success:
                TypeCssClass = "alert-success";
                break;
            case AlertDialogType.Information:
                TypeCssClass = "alert-information";
                break;
            case AlertDialogType.Warning:
                TypeCssClass = "alert-warning";
                break;
            case AlertDialogType.Error:
                TypeCssClass = "alert-error";
                break;
        }
    }

    public void Show()
    {
        VisibilityCssClass = VisibilityCssClass.Replace("show d-none", "show");
    }

    public void Hide()
    {
        VisibilityCssClass = VisibilityCssClass.Replace("show", "show d-none");
    }
}

public enum AlertDialogType
{
    Success = 0,
    Information = 1,
    Warning = 2,
    Error = 3
}