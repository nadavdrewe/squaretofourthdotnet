using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspose.Cells;
using Aspose.Cells.GridWeb;
using WebGrease.Css.Extensions;
using Revel._808nd.com;
using FileFormatType = Aspose.Cells.GridWeb.Data.FileFormatType;
using Web.Grind._808nd.com.Services;


namespace Web.Grind._808nd.com.Pages
{

    [Authorize(Roles = "user, admin")]
    public partial class MainCashup : System.Web.UI.Page
    {

        /*SheetSyncing syncingService { get; set; }*/

        public MainCashup()
        {
                

        }

        [Authorize(Roles = "user, admin")]
        protected void Page_Load(object sender, EventArgs e)
        {

            //set time to 12 - 3 am and should return day before today
            var testDay = Revel._808nd.com.Classes.RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now);



            if (!IsPostBack)
            {
                var syncingService = new SheetSyncing();
                var standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\cash.xlsx";
                var path = standardTemplate;

                //check if this weeks sheet exists, if not, create and make it the open sheet            
                if (Session["activeFilename"] != null)
                {
                    path = (string) Session["activeFilename"];
                    GridWeb1.WebWorksheets.ImportExcelFile(path);

                }
                else
                {
                    var ok = false;
                    ok = syncingService.DoesThisWeeksSheetExist();
                    if (ok)
                    {
                        //load as default
                        Session["activeFilename"] = HttpRuntime.AppDomainAppPath + @"SavedFiles\" +
                                                    syncingService.GetFileNameForThisWeeksSheet() + ".xlsx";
                        path = HttpRuntime.AppDomainAppPath + @"SavedFiles\" +
                               syncingService.GetFileNameForThisWeeksSheet() + ".xlsx";
                        GridWeb1.WebWorksheets.ImportExcelFile(path);
                    }
                    else
                    {
                        //create a new one, save it, and load that
                        var aspose = new Workbook(standardTemplate);
                        var savePath = HttpRuntime.AppDomainAppPath + @"SavedFiles\" +
                                       syncingService.GetFileNameForThisWeeksSheet() + ".xlsx";
                        aspose.Save(savePath, SaveFormat.Xlsx);

                        //load the active file as the newly created file
                        Session["activeFilename"] = savePath;
                        GridWeb1.WebWorksheets.ImportExcelFile(savePath);
                    }

                }


                //if not admin
                if (!User.IsInRole("admin"))
                {

                  

                    //Do cell locking
                    GridWeb1.WebWorksheets.ImportExcelFile(path);



                    var day1 = ((int) (DateTime.Now.AddDays(1).DayOfWeek + 7))%7;
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        day1 = 7;
                    }


                    for (int i = 0; i < GridWeb1.WebWorksheets.Count; i++)
                    {
                        GridWeb1.WebWorksheets[i].SetAllCellsReadonly();
                    }



                    //set editables for today
                    GridWeb1.WebWorksheets[day1].SetAllCellsReadonly();
                    GridWeb1.WebWorksheets[day1].SetEditableRange(3, 1, 9, 1);

                    GridWeb1.WebWorksheets[day1].SetEditableRange(16, 1, 9, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(28, 1, 9, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(40, 1, 8, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(52, 1, 2, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(57, 1, 6, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(66, 1, 2, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(76, 1, 5, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(83, 1, 10, 1);

                    //do the rest
                    GridWeb1.WebWorksheets[day1].SetEditableRange(52, 0, 2, 1);
                    GridWeb1.WebWorksheets[day1].SetEditableRange(83, 0, 10, 1);
                } //end cell locking


                var files =
                    Directory.GetFiles(HttpRuntime.AppDomainAppPath + "SavedFiles")
                        .OrderByDescending(d => new FileInfo(d).LastWriteTime).ToSafeReadOnlyCollection();


                List<string> justFileNames = new List<string>();

                foreach (var file in files)
                {

                    justFileNames.Add(Path.GetFileName(file));
                }

                ListBox1.DataSource = justFileNames;
                ListBox1.DataBind();




                //set editables final

                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetAllCellsReadonly();
             /*   GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(3, 1, 9, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(16, 1, 9, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(28, 1, 9, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(40, 1, 8, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(52, 1, 2, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(57, 1, 6, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(66, 1, 2, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(76, 1, 5, 8);
                GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetEditableRange(83, 1, 10, 8);*/


                //set filName
                var array = ((string)Session["activeFilename"]).Split('\\');
                var filename = array[array.Count() - 1];

                Label1.Text = "Logged in as: " + User.Identity.Name +   " - NOW EDITING:" + filename;

            }
        }


        [System.Web.Services.WebMethod]
        public  string OpenCashupFile(string filename)
        {
            var path = HttpRuntime.AppDomainAppPath + "//SavedFiles//" + filename;
            Session["activeFilename"] = path;
            /*Response.Redirect(Request.RawUrl);*/
            return path;
        }

        protected void GridWeb1_SaveCommand(object sender, EventArgs e)
        {
            /*var filename = Web.Grind._808nd.com.Extensions.DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday)
                    .ToLongDateString();
            var path = HttpRuntime.AppDomainAppPath + "SavedFiles\\"+ filename +".xlsx";*/
            var path = "";

            if (Session["activeFilename"] != null)
            {
                path = (string)Session["activeFilename"];

            }
            else
            {
                path = HttpRuntime.AppDomainAppPath + "SavedFiles\\" + "Temp_" + DateTime.Now.ToLongDateString() +
                       ".xlsx";
            }

            
            GridWeb1.WebWorksheets.SaveToExcelFile(path, FileFormatType.Excel2007);
            Response.Redirect("~/Pages/MainCashup.aspx");

        }

        protected int DownloadFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            long sz = fi.Length;

            Response.ClearContent();
            Response.ContentType = Path.GetExtension(filePath);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(filePath)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(filePath);
            Response.End();

            return 0;
        }

        protected void Button1_OnClick(object sender, EventArgs e)
        {
               Session["activeFilename"] = HttpRuntime.AppDomainAppPath + "XL\\cash.xlsx";            
        }
    }
}