namespace Revel._808nd.com.Classes
{
    public interface IPointsLoggable
    {
        int orginal_points_total { get; set; }
        int new_points_total { get; set; }
        int pointsAdded { get; set; }
        int multiplier { get; set; }
        string card_number { get; set; }
        int pointSetToRefreshInBucket { get; set; }
    }
}
