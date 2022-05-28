namespace Jarvis.Shared.Components.Modaler;

public record Modal
{
    public Guid Id = Guid.NewGuid();
    
    public string Title { get; init; } = string.Empty;
    
    public string Message { get; init; } = string.Empty;
    
    public string HeaderCssClass { get; init; } = string.Empty;

    public static Modal CreateModal(
        string title,
        string message,
        ModalType modalType)
    {
        return new Modal
        {
            Title = title,
            Message = message,
            HeaderCssClass = $"bg-{modalType.ToString().ToLowerInvariant()}"
        };
    }
}

public enum ModalType
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
