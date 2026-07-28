using Square;
using System;
using System.Collections.Generic;
using System.Text;

namespace square.pipeline.fourth.com.Services
{
    public abstract class BaseService
    {
        protected SquareClient _client;

        public BaseService(string apiToken, string baseUrl = null)
        {
            _client = string.IsNullOrWhiteSpace(baseUrl)
                ? new SquareClient(apiToken)
                : new SquareClient(apiToken, new ClientOptions { BaseUrl = baseUrl });
        }
    }
}
