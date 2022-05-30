namespace Jarvis.Shared.Components.Toaster.Models;

public record Toast
{
    public Guid Id = Guid.NewGuid();

    public string Title { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string HeaderCssClass { get; init; } = string.Empty;

    public DateTimeOffset TimeToBurn { get; init; } = DateTimeOffset.Now.AddSeconds(5);

    public bool IsBurnt => TimeToBurn < DateTimeOffset.Now;

    public readonly DateTimeOffset Posted = DateTimeOffset.Now;

    public static Toast CreateToast(
        string title,
        string message,
        ToastType toastType,
        int delayBeforeDismiss = 5)
    {
        return new Toast
        {
            Title = title,
            Message = message,
            HeaderCssClass = $"bg-{toastType.ToString().ToLowerInvariant()}",
            TimeToBurn = DateTimeOffset.Now.AddSeconds(delayBeforeDismiss)
        };
    }
}

public enum ToastType
{
    Primary = 0,
    Secondary = 1,
    Dark = 2,
    Light = 3,
    Success = 4,
    Danger = 5,
    Warning = 6,
    Info = 7
}
