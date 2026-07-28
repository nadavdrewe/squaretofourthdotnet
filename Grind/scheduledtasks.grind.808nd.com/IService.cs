using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interfaces.service.grind._808nd.com
{
    public interface IService
    {
        void Start();

        void Stop();

        void Continue();

        void Pause();

        bool RunScheduledTasks();
    }
}
