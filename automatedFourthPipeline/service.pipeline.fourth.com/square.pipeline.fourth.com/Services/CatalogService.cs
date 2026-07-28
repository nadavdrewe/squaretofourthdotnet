using Square;
using Square.Catalog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace square.pipeline.fourth.com.Services
{
    public class CatalogService : BaseService
    {
        public CatalogService(string apiToken, string baseUrl = null) : base(apiToken, baseUrl)
        {
        }

        public async Task<IEnumerable<CatalogObject>> GetCatalog()
        {
            var toReturn = new List<CatalogObject>();
            var catResponse = await _client.Catalog.ListAsync(new ListCatalogRequest());

            await foreach (var item in catResponse)
            {
                toReturn.Add(item);
            }

            return toReturn;
        }
    }
}
