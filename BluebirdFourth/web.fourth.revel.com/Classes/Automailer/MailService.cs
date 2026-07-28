using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Email.Mail;

namespace web.fourth.revel.com
{
    public class MailService
    {

        //define some props
        //the email message
        protected MailMessage theAsposeMessage;
        SmtpClient theSMTPClient = new SmtpClient();


        //wrapper 
        public string FromEmailAddress = @"railgunit.maintenance@gmail.com";
        public string LoginEmailAddress = @"railgunit.maintenance@gmail.com";
        //public string FromEmailAddress = "emailnadz@gmail.com";
        //public string LoginEmailAddress = "emailnadz@gmail.com";        
        public string EmailPassword = @"Diagonal23";

        public string ToEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAttachment { get; set; }
        public Attachment TheAttachmentFile { get; set; }


        public MailService(List<string> toAddresses, string subject, string body, string attachmentPath = "")
        {
            //set some defaults so i always get copied in
            ToEmailAddress = @"emailnadz@gmail.com";

            //create the message
            theAsposeMessage = new MailMessage();
            theSMTPClient.Host = "smtp.gmail.com";
            theSMTPClient.Username = LoginEmailAddress;
            theSMTPClient.Password = EmailPassword;
            theSMTPClient.Port = 587;
            theSMTPClient.EnableSsl = true;
            theSMTPClient.SecurityMode = SmtpSslSecurityMode.Explicit;


            //ADDRESSING
            theAsposeMessage.From = FromEmailAddress;
            theAsposeMessage.To = @"emailnadz@gmail.com";

            foreach (var address in toAddresses)
            {
                theAsposeMessage.To.Add(address);
            }



            theAsposeMessage.Subject = subject;

            //set some email body
            theAsposeMessage.HtmlBody = body;

            //add the attachment
            if (attachmentPath != "")
            {
                TheAttachmentFile = new Attachment(attachmentPath);
                theAsposeMessage.Attachments.Add(TheAttachmentFile);

            }






        }


        //define some methods

        //sets all vars ready to email
        public bool SetOutboundVariables()
        {

            return true;
        }



        //sends the actual email
        public bool SendEmail()
        {
            this.theSMTPClient.Send(this.theAsposeMessage);
            return true;
        }


        public int Mail(string emailTo, string message)
        {
            this.ToEmailAddress = emailTo;
            this.theAsposeMessage.Body = message;
            this.theSMTPClient.Send(this.theAsposeMessage);

            theAsposeMessage.Dispose();

            return 0;
        }


    }
}
