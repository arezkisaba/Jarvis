using Lib.Core;

namespace Jarvis.Technical.Models.Domain;

public class Movie
{
    public string IdMediaDatabase { get; }

    public string IdMediaCenter { get; }

    public string Title { get; }

    public int? Year { get; }

    public string Language { get; }

    public bool IsCompletedOnMediaDatabase { get; }

    public bool IsCompletedOnMediaCenter { get; }

    public IEnumerable<string> SearchTitles { get; }

    public string StorageTitle => Title.TransformForStorage();

    public Movie(
        string idMediaDatabase,
        string idMediaCenter,
        string title,
        int? year,
        string language,
        bool isCompletedOnMediaDatabase,
        bool isCompletedOnMediaCenter,
        IEnumerable<string> searchTitles)
    {
        IdMediaDatabase = idMediaDatabase;
        IdMediaCenter = idMediaCenter;
        Title = title;
        Year = year;
        Language = language;
        IsCompletedOnMediaDatabase = isCompletedOnMediaDatabase;
        IsCompletedOnMediaCenter = isCompletedOnMediaCenter;
        SearchTitles = searchTitles;
    }
}
