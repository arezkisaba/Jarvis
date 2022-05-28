using Lib.Core;

namespace Jarvis.Technical.Models.Domain;

public class TvShow
{
    public string IdMediaDatabase { get; set; }

    public string IdMediaCenter { get; set; }

    public string Title { get; set; }

    public int? Year { get; }

    public string Language { get; }

    public IEnumerable<string> SearchTitles { get; }

    public IList<Season> Seasons { get; }

    public string StorageTitle => Title.TransformForStorage();

    public TvShow(
        string idMediaDatabase,
        string idMediaCenter,
        string title,
        int? year,
        string language,
        IEnumerable<string> searchTitles)
    {
        IdMediaDatabase = idMediaDatabase;
        IdMediaCenter = idMediaCenter;
        Title = title;
        Year = year;
        Language = language;
        SearchTitles = searchTitles;
        Seasons = new List<Season>();
    }
}
