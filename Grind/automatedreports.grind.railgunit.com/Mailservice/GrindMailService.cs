using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;

namespace automatedreports.grind.railgunit.com
{
    public static class GrindMailService
    {
        public static void SendHTMLEmail(string username,
          string userRealFromName,
          string password,
          string toEmail,
          string toRealName,
          string subject,
          string HTMLbody,
          List<string> attachmentFileLocations = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userRealFromName, username));
            message.To.Add(new MailboxAddress(toRealName, toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = HTMLbody;

            if (attachmentFileLocations != null)
            {
                foreach (var fileString in attachmentFileLocations)
                {
                    try
                    {
                        builder.Attachments.Add(fileString);
                    }
                    catch (Exception)
                    {


                    }
                }
            }

            message.Body = builder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("mail.privateemail.com", 465, true);
                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(username, password);

                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }

        }
    }
}
