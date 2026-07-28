using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.SelimaFTPClient
{
    public class SelimaFTPClient
    {
        public void Upload(string ftpServer, string userName, string password, string fullFileNameAndPath, string fileNameToCreateOnServer)
        {

            var entirePathForTransfer = "ftp://" + ftpServer + "/" + fileNameToCreateOnServer;

            //+ "/In/" + establihsmentIdForPath + "/TillSales/" + new FileInfo(fullFileNameAndPath).Name;

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;// | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // comparable to modern browsers              
            FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(
entirePathForTransfer);

            ftpClient.Credentials = new System.Net.NetworkCredential(userName, password);
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
            string responseValue = uploadResponse.StatusDescription;
            uploadResponse.Close();

        }
    }
}
