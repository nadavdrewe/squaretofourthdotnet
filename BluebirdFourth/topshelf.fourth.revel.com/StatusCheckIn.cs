using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace topshelf.fourth.revel.com
{
    public class StatusCheckIn
    {
        readonly Timer _timer;

        public StatusCheckIn()
        {
            _timer = new Timer(5000) { AutoReset = true };
            _timer.Elapsed += TimeHappened;
        }

        static void TimeHappened(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Status Time: {0} checking in.",
                DateTime.Now);
            Console.WriteLine("Sender Type: {0}",
                sender.GetType());
            Console.WriteLine("Event Argument Type: {0}",
                eventArgs);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
