namespace Jarvis;

public sealed class ChartAreaModel
{
    public string DisplayName { get; private set; }

    public double Value { get; private set; }

    public string BackgroundColor { get; private set; }

    public ChartAreaModel(
        string displayName,
        double value,
        string backgroundColor)
    {
        DisplayName = displayName;
        Value = value;
        BackgroundColor = backgroundColor;
    }
}
