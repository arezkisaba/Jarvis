namespace Jarvis.Models.Domain;

public class Season
{
    public string IdMediaCenter { get; }

    public int Number { get; }

    public string Language { get; }

    public TvShow TvShow { get; }

    public IList<Episode> Episodes { get; }

    public bool IsCompletedOnMediaDatabase
    {
        get
        {
            if (Episodes == null)
            {
                return false;
            }

            return Episodes.All(obj => obj.IsCompletedOnMediaDatabase);
        }
    }

    public bool IsCompletedOnMediaCenter
    {
        get
        {
            if (Episodes == null)
            {
                return false;
            }

            return Episodes.All(obj => obj.IsCompletedOnMediaCenter);
        }
    }

    public int EpisodesCollectedCount
    {
        get
        {
            var episodesCount = 0;
            foreach (var episode in Episodes)
            {
                episodesCount++;
            }

            return episodesCount;
        }
    }

    public Season(
        string idMediaCenter,
        int number,
        string language,
        TvShow tvShow)
    {
        IdMediaCenter = idMediaCenter;
        Number = number;
        Language = language;
        TvShow = tvShow;
        Episodes = new List<Episode>();
    }
}
