using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace web.fourth.revel.com.Classes.Utility
{
    public static class Utility
    {

        public static string GetRandomImage(string theFullSystemPath)
        {

            var random = new Random(); // this should be placed in a static member variable, but is ok for this example
            var fileNames = System.IO.Directory.GetFiles(theFullSystemPath, "*.gif", SearchOption.AllDirectories);
            
            var randomFile = fileNames[random.Next(0, fileNames.Length)];

            return randomFile;
        }

    }
}