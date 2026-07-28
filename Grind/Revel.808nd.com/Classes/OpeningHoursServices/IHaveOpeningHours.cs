using System;
using System.Security.Cryptography.X509Certificates;

namespace Revel._808nd.com.Classes.OpeningHoursServices
{
    interface IHaveOpeningHours
    {
        DateTime OpeningDateTime { get; set; }

    }
}