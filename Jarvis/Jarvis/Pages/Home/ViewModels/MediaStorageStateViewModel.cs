using Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Models;
using Jarvis.Technical.Helpers;

namespace Jarvis.Pages.Home.ViewModels;

public sealed class MediaStorageStateViewModel
{
    public IEnumerable<string> DefaultChartColors = new List<string>() { "#E89725", "#25CFAF", "#DC3545", "#259DE8", "#54C6FF", "#28A745", "#CA45FF", "#4B594E", "#C4DB2E" };

    public IEnumerable<(string DisplayName, long Size, string Color)> Folders { get; private set; }

    public long UsedSpace { get; private set; }

    public long FreeSpace { get; private set; }

    public long TotalSpace { get; private set; }

    public string Title { get; private set; }

    public string PieChartCssStyle { get; private set; }

    public MediaStorageStateViewModel(
        MediaStorageStateModel model)
    {
        Update(model);
    }

    public void Update(
        MediaStorageStateModel model)
    {
        Folders = model?.Folders?.Select((obj, i) => (obj.DisplayName, obj.Size, DefaultChartColors.ElementAt(i))).ToList();
        UsedSpace = model?.UsedSpace ?? 0;
        FreeSpace = model?.FreeSpace ?? 0;
        TotalSpace = model?.TotalSpace ?? 0;

        if (Folders != null)
        {
            ComputeTitle();
            ComputePieChartCssStyle();
        }
    }

    private void ComputeTitle()
    {
        Title = $"{FormatHelper.GetStringWithUnitFromSize(UsedSpace)} / {FormatHelper.GetStringWithUnitFromSize(TotalSpace)}";
    }

    private void ComputePieChartCssStyle()
    {
        var lastEnd = 0d;
        var replaceBy = string.Empty;
        for (var i = 0; i < Folders.Count(); i++)
        {
            var folderTotalSpace = Folders.Sum(obj => obj.Size);
            var (DisplayName, Size, Color) = Folders.ElementAt(i);
            var style = $"{Color} {lastEnd / folderTotalSpace * 100}% {(lastEnd + Size) / folderTotalSpace * 100}%";

            if (i < Folders.Count() - 1)
            {
                style += ",";
            }

            replaceBy += style;
            lastEnd += Size;
        }

        PieChartCssStyle = $"background: conic-gradient({replaceBy});";
    }
}
