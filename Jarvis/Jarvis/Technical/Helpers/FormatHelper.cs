namespace Jarvis.Technical.Helpers;

public static class FormatHelper
{
    public static string GetStringWithUnitFromSize(
        double size)
    {
        var unit = "B";
        var coef = 1024d;

        if (size / coef > 1)
        {
            size /= coef;
            unit = "KB";
        }

        if (size / coef > 1)
        {
            size /= coef;
            unit = "MB";
        }

        if (size / coef > 1)
        {
            size /= coef;
            unit = "GB";
        }

        if (size / coef > 1)
        {
            size /= coef;
            unit = "TB";
        }

        return $"{size.ToString("0.##")} {unit}";
    }
}
