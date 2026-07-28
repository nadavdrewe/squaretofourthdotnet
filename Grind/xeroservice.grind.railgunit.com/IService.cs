using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xeroservice.grind.railgunit.com
{
    public interface IService
    {
        void Start();

        void Stop();

        void Continue();

        void Pause();

        void ScheduleTasks();

    }
}
