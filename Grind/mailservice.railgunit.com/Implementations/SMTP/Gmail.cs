using System;
using System.Collections.Generic;
using Aspose.Email.Mail;
using mailservice.railgunit.com.Interfaces;

namespace mailservice.railgunit.com.Implementations.SMTP
{
  

    /// <summary>
    /// Works with Gmail accounts set to 'less secure authorisation
    ///  </summary>
   
   
    public class GmailLessSecureMailService : IMailService
    {
        protected MailMessage theAsposeMessage;
        SmtpClient theSMTPClient = new SmtpClient();
        //wrapper 

       
        private Attachment TheAttachmentFile { get; set; }



      

        public GmailLessSecureMailService(string login, string password, 
            IEnumerable<string> toAddresses, string subject, string htmlMessage, string attachmentPath = "")
        {
       
            //create the message
            theAsposeMessage = new MailMessage();
            theSMTPClient.Host = "smtp.gmail.com";
            theSMTPClient.Username = login;
            theSMTPClient.Password = password;
            theSMTPClient.Port = 587;
            theSMTPClient.EnableSsl = true;
            theSMTPClient.SecurityMode = SmtpSslSecurityMode.Explicit;


            //ADDRESSING
            theAsposeMessage.From = login;
            
            foreach (var address in toAddresses)
            {
                theAsposeMessage.To.Add(address);
            }
            
            theAsposeMessage.Subject = subject;
            theAsposeMessage.HtmlBody = htmlMessage;


            //add the attachment
            try
            {
                if (attachmentPath != "")
                {
                    TheAttachmentFile = new Attachment(attachmentPath);
                    theAsposeMessage.Attachments.Add(TheAttachmentFile);

                }
            }
            catch (System.Exception exception)
            {

                throw new Exception("Couldn't attach file", exception) ;
            }
          
        }

        public bool SendEmail()
        {
            this.theSMTPClient.Send(this.theAsposeMessage);
            return true;
        }

    }
}
