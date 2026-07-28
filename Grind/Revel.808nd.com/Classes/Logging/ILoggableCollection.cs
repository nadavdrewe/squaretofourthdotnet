using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes.Logging
{
    public interface ILoggableCollection
    {
        IEnumerable<IIdentifiable> TheCollection { get; set; }
        string SystemLocation { get; set; }
        DateTime? WhenLogged { get; set; }
        string CollectionDescription { get; set; }
    }

    public class LoggableCollection : ILoggableCollection
    {
        public IEnumerable<IIdentifiable> TheCollection { get; set; }
        public string SystemLocation { get; set; }
        public DateTime? WhenLogged { get; set; }
        public string CollectionDescription { get; set; }

        public LoggableCollection(IEnumerable<IIdentifiable> theCollection, string systemLocation, DateTime? whenLogged, string collectionDescription)
        {
            this.SystemLocation = systemLocation;
            this.WhenLogged = whenLogged;
            this.TheCollection = theCollection;
            CollectionDescription = collectionDescription;
        }
    }
}
