namespace Jarvis;

public sealed class MediaStorageStateViewModel
{
    public IEnumerable<string> DefaultChartColors = new List<string>() { "#E89725", "#CA45FF", "#259DE8", "#DC3545", "#54C6FF", "#28A745", "#4B594E", "#C4DB2E" };

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
        Title = $"{GetStringWithUnitFromSize(UsedSpace)} / {GetStringWithUnitFromSize(TotalSpace)}";
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

    private string GetStringWithUnitFromSize(
        long size)
    {
        var unit = "B";
        var finalSize = Convert.ToDouble(size);
        var coef = 1024d;

        if (finalSize / coef > 1)
        {
            finalSize /= coef;
            unit = "KB";
        }

        if (finalSize / coef > 1)
        {
            finalSize /= coef;
            unit = "MB";
        }

        if (finalSize / coef > 1)
        {
            finalSize /= coef;
            unit = "GB";
        }

        if (finalSize / coef > 1)
        {
            finalSize /= coef;
            unit = "TB";
        }

        return $"{finalSize.ToString("0.##")} {unit}";
    }
}
