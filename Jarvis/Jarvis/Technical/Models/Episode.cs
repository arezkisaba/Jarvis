namespace Jarvis.Technical.Models;

public class Episode
{
    public string IdMediaCenter { get; set; }

    public int Number { get; set; }

    public bool IsCompletedOnMediaDatabase { get; set; }

    public bool IsCompletedOnMediaCenter { get; set; }

    public Season Season { get; set; }

    public Episode(
        string idMediaCenter,
        int number,
        bool isCompletedOnMediaDatabase,
        bool isCompletedOnMediaCenter,
        Season season)
    {
        IdMediaCenter = idMediaCenter;
        Number = number;
        IsCompletedOnMediaDatabase = isCompletedOnMediaDatabase;
        IsCompletedOnMediaCenter = isCompletedOnMediaCenter;
        Season = season;
    }
}
