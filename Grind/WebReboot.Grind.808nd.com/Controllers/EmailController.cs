using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aspose.Cells.GridWeb.Data;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Services;

namespace Web.Grind._808nd.com.Controllers
{
    public class EmailController : Controller
    {
        private GrindContext db;
        public EmailController()
        {
            db = new GrindContext();
        }
        //
        // GET: /Email/
        public async void SendTestEmailToNadav()
        {
            var to = new List<string>();
            to.Add("emailnadz@gmail.com");

            MailService mail = new MailService(to, "This is a test email. It tests the scheduler");
            mail.SendEmail();


        }


        public int SaveSheet(Aspose.Cells.GridWeb.GridWeb theGrid)
        {

            var path = HttpRuntime.AppDomainAppPath + "SavedFiles\\" + DateTime.Now.ToLongDateString() +
                       ".xlsx";



            theGrid.WebWorksheets.SaveToExcelFile(path, FileFormatType.Excel2007);

            return 0;

        }

        [HttpPost]
        public void EmailToAll()
        {
            //


            int count = 0;

            if (count < 1)
            {
                var branches = db.Establishments.Where(y => !y.name.ToLower().Contains("parent")).ToList();

                string sheet = (string)Session["activeFilename"].ToString();
                string savePath = sheet;

                sheet = sheet.Split('\\')[sheet.Split('\\').Count() - 1].Split(' ')[0];

                var toSend = db.CashupNotifiers.ToList();
                var to = new List<string>();
                var listOfNamesSentTo = "";

                foreach (var notee in toSend.Where(x => x.UniversalContact == true))
                {
                    to.Add(notee.NotificationAddress);
                    listOfNamesSentTo += notee.NotificationAddress + ", ";
                }

                if (!to.Contains(User.Identity.Name))
                {
                    to.Add(User.Identity.Name);
                    listOfNamesSentTo += User.Identity.Name + ", ";
                }

                //add relevant manager
                var branch = branches.Where(x => x.name.ToLower().Contains(sheet.ToLower())).FirstOrDefault();
                var sendees = db.CashupNotifiers.Where(x => x.DBKEY_establishment_id == branch.DBKEY_establishment_id && x.UniversalContact == false).ToList();

                foreach (var send in sendees)
                {
                    to.Add(send.NotificationAddress);
                }


                var syncingService = new SheetSyncing();

                var pathList = new List<string>();
                pathList.Add(savePath);
                MailService mail = new MailService(to, "Grind Nightly Cashup Spreadsheet", null, pathList);
                mail.SendEmail();

                mail.TheAttachmentFile.Dispose();

                count++;

                var grind = sheet.Split('\\')[sheet.Split('\\').Count() - 1].Split(' ')[0];
                db.SystemLogs.Add(new SystemLog
                {
                    WhenCreated = DateTime.Now,
                    Note = savePath + " sent to all users - " + listOfNamesSentTo,
                    Type = "CASHUP_EMAIL",
                    WhoTriggered = User.Identity.Name,
                });

                db.SaveChanges();


            }

        }


        [HttpPost]
        public void EmailToUser()
        {
            int count = 0;

            if (count < 1)
            {
                string sheet = (string)Session["activeFilename"];


                var to = new List<string>();
                to.Add(User.Identity.Name);

                var syncingService = new SheetSyncing();
                var savePath = sheet;

                var pathList = new List<string>();
                pathList.Add(savePath);
                MailService mail = new MailService(to, "Grind Nightly Cashup Spreadsheet", null, pathList);
                mail.SendEmail();

                mail.TheAttachmentFile.Dispose();

                count++;

                db.SystemLogs.Add(new SystemLog
                {
                    WhenCreated = DateTime.Now,
                    Note = sheet + " sent to single user - " + User.Identity.Name,
                    Type = "CASHUP_EMAIL",
                    WhoTriggered = User.Identity.Name,
                });

                db.SaveChanges();

            }

        }

        /*  public override EmailToAll(string username, Aspose.Cells.GridWeb.GridWeb theGrid)
          {
              SaveSheet(theGrid);
              EmailToAll(username);

              return 0;
          }*/

        public void CustomEmailMessage(string customerMessage)
        {
            string message = customerMessage;

            var to = new List<string>();
            to.Add("emailnadz@gmail.com");

            MailService mail = new MailService(to, message, message);
            mail.SendEmail(); ;
        }

        public void SyncCompleted(int cardsok, int multiplierOK, int createdTimestamps)
        {
            string message = "The scheduler ran: " +
            cardsok + " red cards were reset, " + multiplierOK + " cards were multiplied, " + createdTimestamps +
            " timestamps were created";

            var to = new List<string>();
            to.Add("emailnadz@gmail.com");

            MailService mail = new MailService(to, message);
            mail.SendEmail(); ;
        }

        public void SyncFailed()
        {

            string message = "THE GRIND 3am SYNC FAILED - The scheduler ran:";


            var to = new List<string>();
            to.Add("emailnadz@gmail.com");

            MailService mail = new MailService(to, message);
            mail.SendEmail(); ;
        }

        public void SendMessageNadavIgnoreSendExeceptions(string subject, string htmlbody = null, string recepient = null)
        {

            var to = new List<string>();
            if (String.IsNullOrWhiteSpace(recepient))
            {
                to.Add("emailnadz@gmail.com");
            }
            else
            {
                to.Add(recepient);
            }


            MailService mail = new MailService(to, subject, htmlbody);
            try
            {
                mail.SendEmail();
            }
            catch (Exception)
            {
                //repress
            }
        }

        public void SendMessageGrindErrorAndNadavIgnoreSendExeceptions(string subject, string htmlbody = null, string recepient = null)
        {

            var to = new List<string>();
            if (String.IsNullOrWhiteSpace(recepient))
            {
                to.Add("emailnadz@gmail.com");
                to.Add("error@grind.co.uk");
            }
            else
            {
                to.Add(recepient);
            }


            MailService mail = new MailService(to, subject, htmlbody);
            try
            {
                mail.SendEmail();
            }
            catch (Exception)
            {
                //repress
            }
        }

        public void SendMessage(string recepient, string subject, string htmlbody = null)
        {

            var to = new List<string>();
            to.Add(recepient);
            to.Add("emailnadz@gmail.com");

            MailService mail = new MailService(to, subject, htmlbody);
            mail.SendEmail(); ;
        }


    }
}