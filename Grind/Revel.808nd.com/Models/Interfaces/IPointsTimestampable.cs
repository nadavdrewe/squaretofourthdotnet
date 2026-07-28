using System;

namespace Revel._808nd.com.Interfaces
{
    public interface IPointsTimestampable
    {
        DateTime date { get; set; }
        int total_points_on_date { get; set; }
        //whatever identifier you want to use, cast to your own type
        string identifier { get; set; }

    }
}
