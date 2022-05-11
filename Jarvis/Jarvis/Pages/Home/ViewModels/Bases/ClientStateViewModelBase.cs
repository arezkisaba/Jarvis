namespace Jarvis;

public abstract class ClientStateViewModelBase
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public ClientStateViewModelBase(
        ClientStateModelBase model)
    {
        UpdateInternalData(model);
    }

    public void UpdateInternalData(
        ClientStateModelBase model)
    {
        Title = model.Title;
        Subtitle = model.Subtitle;
        IsActive = model.IsActive;
    }
}
