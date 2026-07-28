using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;

namespace api.grind._808nd.com.Controllers
{
    public class TestController : ApiController
    {

        public class Address
        {
            public string email { get; set; }

        }
        static string path = @"C:\test\";

        static string pathAndFile = @"C:\test\signupnames.txt";
        // GET: Test

        [HttpGet]
        [HttpPost]
        public IHttpActionResult Go(Address email)
        {

           
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                if (!System.IO.File.Exists(pathAndFile))
                {
                    var f = File.Create(pathAndFile);
                    f.Close();
                }
                using (var tw = new StreamWriter(pathAndFile, true))
                {
                    tw.WriteLine(email.email);
                    tw.Close();
                }


                return Ok();

            }
            catch (Exception ex)
            {


                return Ok();

            }
        }

        public IHttpActionResult TestSignup(string email)
        {

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                else if (System.IO.File.Exists(path))
                {
                    using (var tw = new StreamWriter(path, true))
                    {
                        tw.WriteLine(email);
                        tw.Close();
                    }
                }

                return Ok();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}