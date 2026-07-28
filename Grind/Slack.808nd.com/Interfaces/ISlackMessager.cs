using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slack._808nd.com.Interfaces
{
    interface ISlackMessager
    {
        Task<string> SendMessage(string message, string channel, string username);

    }
}
