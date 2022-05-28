using Jarvis.Shared.Components.Modaler.Models;

namespace Jarvis.Shared.Components.Modaler.Services;

public class ModalerService : IDisposable
{
    private Modal _modal = null;

    public event EventHandler ModalerChanged;

    public bool HasModal => _modal != null;

    public ModalerService()
    {
    }

    public Modal GetModal()
    {
        return _modal;
    }

    public void AddModal(
        Modal modal)
    {
        _modal = modal;
        ModalerChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearModal()
    {
        _modal = null;
        ModalerChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
    }
}