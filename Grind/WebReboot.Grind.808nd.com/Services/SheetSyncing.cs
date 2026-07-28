using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Aspose.Cells;
using Web.Grind._808nd.com.Extensions;
using FileFormatType = Aspose.Cells.GridWeb.Data.FileFormatType;

namespace Web.Grind._808nd.com.Services
{
    public class SheetSyncing
    {

     


        public  string CreateNewSavedWorksheet(string sheetName)
        {
            string standardTemplate = "";
            string findGrindName= sheetName.ToLower();
            

            if(findGrindName.Contains("london"))
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\londoncash.xlsx";
            }
            else if(findGrindName.Contains("shoreditch"))
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\shoreditchcash.xlsx";
            }
            else if (findGrindName.Contains("soho"))
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\sohocash.xlsx";
            }
            else if (findGrindName.Contains("holborn"))
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\holborncash.xlsx";
            }
            else if (findGrindName.Contains("stratford"))
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\stratfordcash.xlsx";
            }
            else if (findGrindName.Contains("radio"))
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\radiocash.xlsx";
            }
            //default
            else
            {
                standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\cash.xlsx";
            }


            //create the sheet
            SheetSyncing syncingService = new SheetSyncing();

            var aspose = new Workbook(standardTemplate);
            var savePath = sheetName;
                
               
            aspose.Save(savePath, SaveFormat.Xlsx);
            return savePath;
        }



        public string GetFullWorkingNameForAnyWorksheet(string grindName, string Datepart)
        {
            var sync = new SheetSyncing();

            var savePath = HttpRuntime.AppDomainAppPath + @"SavedFiles\"
                            + grindName
                            + " - " 
                            + Datepart 
                            + ".xlsx";

            return savePath;
        }

        public List<string> ReturnThisWeeksSheetsThatDontExist(List<string>  branches)
        {
            List<string> thisWeeksSheets = new List<string>();

            List<string> theGrinds = branches;
                        

            var listToReturn = new List<string>();

            //get all sheets we need
            foreach (var grind in theGrinds)
            {
                thisWeeksSheets.Add(GetFullWorkingNameForAnyWorksheet(grind, GetFileNameDatePartForThisWeeksSheet()));
            }

            //get all sheets we have
            var allSheets = GetAllSheets();


            List<string> sheetsThatDontExist = new List<string>();

            //compare them and store the ones we don't have
            foreach (var thisWeeksSheet in thisWeeksSheets)
            {
                var nameandpath = thisWeeksSheet.Split('\\').ElementAt(thisWeeksSheet.Split('\\').Count() - 1);
                var nameonly = nameandpath.Split('.').ElementAt(0);
                if (!allSheets.Contains(nameonly))
                { 
                    sheetsThatDontExist.Add(thisWeeksSheet);
                }
            }

            //return ones we don't have
            return sheetsThatDontExist;
        }

        public List<string> GetAllSheets()
        {
               var files =
                Directory.GetFiles(HttpRuntime.AppDomainAppPath + "SavedFiles")
                    .OrderByDescending(d => new FileInfo(d).LastWriteTime).ToList();

            var justName = new List<string>();

            foreach (var file in files)
            {
                justName.Add(Path.GetFileNameWithoutExtension(file));    
            }
            return justName;

        }

        public string GetFileNameDatePartForThisWeeksSheet()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            var filename =
                Web.Grind._808nd.com.Extensions.DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday)
                    .ToLongDateString();



            return filename;

        }


      


    }
}