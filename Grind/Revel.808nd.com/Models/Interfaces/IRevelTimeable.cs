using System;

namespace Revel._808nd.com.Interfaces
{
    public interface IRevelTimeable
    {
        DateTime? start_time { get; set; }

        DateTime? kitchen_completed { get; set; }

        DateTime? expedited { get; set; }

    }
}
