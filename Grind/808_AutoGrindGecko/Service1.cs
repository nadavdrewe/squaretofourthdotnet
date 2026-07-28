using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Mvc;
using GeckoboardTestWebApp.Controllers;
using Nito.AsyncEx;

namespace _808_AutoGrindGecko
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "808GrindGecko";
        }



        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            System.Diagnostics.Debugger.Break();
            try
            {
                AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception ex)
            {
                
                
            }


            try
            {
                var timer = new Timer(500000);
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
            
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var args = new string[1];
            AsyncContext.Run(() => MainAsync(args));
        }



        public async static Task<int> MainAsync(string[] args)
        {
          
            var tc = new TestController();
            var ok = await tc.MAINTAINANCE_Today_OrderSync();
            System.Console.WriteLine("Today - Update Order Complete");

            return 0;
        }




        protected override void OnStop()
        {

            Console.WriteLine("Shutting Down");
        }
    }
}
