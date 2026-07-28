using employeeredirect.grind.railgunit.com.Models;
using Microsoft.AspNet.Identity.Owin;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using Revel._808nd.com.ObjectCreationFactories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace employeeredirect.grind.railgunit.com.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("ViewMenu");
        }

        [Authorize]
        public ActionResult ViewMenu()
        {
            return View();
        }
        
        public async Task SeedEmployees()
        {

            using (var db = new GrindContext())
            {
                //refresh all employees
                try
                {
                    string RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
                    string RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];

                    Establishment revOrg = new Establishment(1, "Grind",
                               RevelAPIKEY,
                               new Uri(RevelBaseURL));

                    RevelFactory revelFactory = new RevelFactory(revOrg);
                    var webReader = new RevelWebserviceDataReader(revOrg);
                    RevelDBWriter writer = new RevelDBWriter(db);
                    IRevelReaderAsync DBReader = new RevelDBReader(revOrg);

                    var emnplyeeInstance = new Employee();
                    var employees = await webReader.GetRevelWebserviceData<Employee>(emnplyeeInstance, emnplyeeInstance.theAddress, new GenericFactory());

                    var existingEmployees = db.Employees.ToList();
                    if (existingEmployees.Count() > 0)
                    { db.Employees.RemoveRange(existingEmployees); }


                    db.Employees.AddRange(employees.Where(x => x.active == true).ToList());
                    db.SaveChanges();



                    using (var userAuthDb = new EmployeeAuthDbContext())
                    {
                        var users = userAuthDb.Users.ToList();
                        //clear all existing users
                        users.ForEach(async x =>
                        {
                            await UserManager.DeleteAsync(x);
                        });

                        //create new users

                        foreach (var employee in db.Employees.Where(x => x.active))
                        {
                            if (!String.IsNullOrWhiteSpace(employee.pin))
                            {
                                var user = new ApplicationUser { UserName = employee.pin, Email = employee.pin };
                                var result = await UserManager.CreateAsync(user, employee.pin);
                            }

                        }

                    }
                }
                catch (Exception ex)
                {

                    throw;
                }



            }

        }
    }
}