using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.Interfaces
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
