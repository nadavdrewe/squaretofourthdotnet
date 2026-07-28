using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;
using Revel._808nd.com.ObjectCreationFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class BaseService : IDisposable
    {
        protected RevelWebserviceDataReader _webReader;
        protected RevelDBWriter _dbwriter;
        protected RevelDBReader _dbReader;

        protected string RevelAPIKEY { get; set; }
        protected string RevelBaseURL { get; set; }
        protected RevelContextBase _db { get; set; }
        protected GenericFactory _genericObjectCreatorFactory { get; set; }

        protected Establishment revOrg;

        public BaseService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db)
        {
            this.RevelAPIKEY = RevelAPIKEY;
            this.RevelBaseURL = RevelBaseURL;
            this._db = db;

            revOrg = new Establishment(1, RevelBaseURL,
              RevelAPIKEY,
              new Uri(RevelBaseURL));

            _genericObjectCreatorFactory = new GenericFactory();
            _webReader = new RevelWebserviceDataReader(revOrg);
            _dbwriter = new RevelDBWriter(_db);
            //_dbReader = new RevelDBReader(est);

        }

        public void Dispose()
        {
            _genericObjectCreatorFactory = null;
            _webReader = null;
            _dbwriter = null;
            revOrg = null;
            _db = null;
        }
      

    }
}
