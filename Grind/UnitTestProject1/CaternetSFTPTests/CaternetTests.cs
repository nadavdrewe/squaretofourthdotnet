using AlexPilotti.FTPS.Client;
using NUnit.Framework;
using Revel._808nd.com.CaternetFTPClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Revel._808nd.com.CaternetFTPClient.CaternetFTPClient;
using static System.Net.WebRequestMethods;

namespace UnitTestProject1.CaternetSFTPTests
{
    [TestFixture]
    public class CaternetTests
    {


        string ftpServer = "ftp.caternetclub.co.uk";
        string establihsmentIdForPath = "1";
        string username = "caternetexport@grind.co.uk";
        string password = "JX7p5d0rhS0i";
        string entirePathForTransfer = "";
        string fullFileNameAndPath = @"C:\test\testXML.xml";

        [SetUp]
        public void Setup()
        {
            entirePathForTransfer = "ftp://" + ftpServer + "/In/" + establihsmentIdForPath + "/TillSales/" + new FileInfo(fullFileNameAndPath).Name;
        }

        [Test]
        public void Test_Second_Attempt()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // comparable to modern browsers              
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(
    entirePathForTransfer);

              
                ftpClient.Credentials = new System.Net.NetworkCredential(username, password);
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;
                ftpClient.EnableSsl = true;
                System.IO.FileInfo fi = new System.IO.FileInfo(fullFileNameAndPath);
                ftpClient.ContentLength = fi.Length;
                byte[] buffer = new byte[4097];
                int bytes = 0;
                int total_bytes = (int)fi.Length;
                System.IO.FileStream fs = fi.OpenRead();
                ftpClient.KeepAlive = false;
                ftpClient.Timeout = -1;
                System.IO.Stream rs = ftpClient.GetRequestStream();
                while (total_bytes > 0)
                {
                    bytes = fs.Read(buffer, 0, buffer.Length);
                    rs.Write(buffer, 0, bytes);
                    total_bytes = total_bytes - bytes;
                }
                fs.Close();
                rs.Close();

                
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                string value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [Test]
        public void Test_Third_Attempt()
        {
            try
            {
                using (FTPSClient client = new FTPSClient())
                {
                    // Connect to the server, with mandatory SSL/TLS 
                    // encryption during authentication and 
                    // optional encryption on the data channel 
                    // (directory lists, file transfers)
                    client.Connect(ftpServer,
                                   new NetworkCredential(username,
                                                         password),
                                   ESSLSupportMode.CredentialsRequested
                                   );

                    // Download a file                    
                    client.GetCurrentDirectory();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [Test]
        public void Test_Fourth_Attempt()
        {
            try
            {
              
              
            
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
