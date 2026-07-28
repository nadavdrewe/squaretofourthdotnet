using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspose.Cells;
using Aspose.Cells.GridWeb;
using Web.Grind._808nd.com.Controllers;
using WebGrease.Css.Extensions;
using Revel._808nd.com;
using FileFormatType = Aspose.Cells.GridWeb.Data.FileFormatType;
using Web.Grind._808nd.com.Services;
using System.Timers;


namespace Web.Grind._808nd.com.Pages
{


    public partial class Cashup : System.Web.Mvc.ViewPage
    {

        /*SheetSyncing syncingService { get; set; }*/

        protected override void OnInit(EventArgs e)
        {
            // Mock the MVC view context
            // Get the HttpContext
            var httpContextBase = new HttpContextWrapper(HttpContext.Current);
            // Build the route data, pointing to the dummy controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", typeof(DummyController).Name);
            // Create the controller context
            var controllerContext = new ControllerContext(new RequestContext(httpContextBase, routeData), new DummyController());
            // Find the partial view
            var view = ViewEngines.Engines.FindPartialView(controllerContext, "~/Views/Shared/_Dummy.cshtml").View;
            // Mock the MVC view context
            ViewContext = new ViewContext(controllerContext, view, new ViewDataDictionary(), new TempDataDictionary(), httpContextBase.Response.Output);

            base.OnInit(e);

            // Init MVC helpers
            InitHelpers();
        }


        public Cashup()
        {


        }


        void currencyUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
     

           GridWeb1.WebWorksheets.SaveToExcelFile("h:\\test\\book1.xls");
        }


        public void TestCommand(object sender, EventArgs e)
        {
            var name = SheetName.Text;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            // Saves to the stream.
            GridWeb1.WebWorksheets.SaveToExcelFile(ms);

            // Sents the file to browser.
            Response.ContentType = "application/vnd.ms-excel";

            //Adds header.
            Response.AddHeader("content-disposition", "attachment; filename=" + name +".xls");

            // Writes file content to the response stream.
            Response.OutputStream.Write(ms.GetBuffer(), 0, (int)ms.Length);

            // OK.
            Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

     
   /*         System.Timers.Timer currencyUpdateTimer = new System.Timers.Timer(60000);
            currencyUpdateTimer.Elapsed += currencyUpdateTimer_Elapsed;
            currencyUpdateTimer.Enabled = true;
            GC.KeepAlive(currencyUpdateTimer);*/

            //set time to 12 - 3 am and should return day before today
            List<string> theGrinds = new List<string>();
            new List<string>();
            theGrinds.Add("Shoreditch Grind");
            theGrinds.Add("Holborn Grind");
            theGrinds.Add("Soho Grind");
            theGrinds.Add("London Grind");
            theGrinds.Add("Covent Grind");
            theGrinds.Add("Stratford Grind");
            theGrinds.Add("Radio Grind");

            GridWeb1.EnableMetalLightEffect = true;
            GridWeb1.SessionMode = SessionMode.Session;
            GridWeb1.OnSubmitClientFunction = "ConfirmFunctionWrap";

            if (!IsPostBack)
            {
                var syncingService = new SheetSyncing();
                var standardTemplate = HttpRuntime.AppDomainAppPath + "XL\\cash.xlsx";
                var path = standardTemplate;


                var sheetsWeNeedToCreate = syncingService.ReturnThisWeeksSheetsThatDontExist(theGrinds);

                foreach (var sheet in sheetsWeNeedToCreate)
                {
                    syncingService.CreateNewSavedWorksheet(sheet);
                }


                //always make shoreditch the open sheet
                if (Session["activeFilename"] != null)
                {
                    path = (string)Session["activeFilename"];
                    GridWeb1.WebWorksheets.ImportExcelFile(path);
                    Session["WebGrid"] = GridWeb1;

                }
                else
                {
                    var ok = false;
                    

                    try
                    {
                        path = syncingService.GetFullWorkingNameForAnyWorksheet(theGrinds.ElementAt(0),
                                    syncingService.GetFileNameDatePartForThisWeeksSheet());

                        //load as default
                        Session["activeFilename"] = syncingService.GetFullWorkingNameForAnyWorksheet(theGrinds.ElementAt(0),
                        syncingService.GetFileNameDatePartForThisWeeksSheet());

                        GridWeb1.WebWorksheets.ImportExcelFile(path);
                        Session["WebGrid"] = GridWeb1;
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }


                //if not admin
                if (!User.IsInRole("admin"))
                {

                    //Do cell locking
 


                    var today = Revel._808nd.com.Classes.RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now);

                    var day1 = ((int)(today.DayOfWeek + 7)) % 7;

                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        day1 = 7;
                    }


                    for (int i = 0; i < GridWeb1.WebWorksheets.Count; i++)
                    {
                        //GridWeb1.WebWorksheets[i].SetAllCellsReadonly();
                    }



                    //set editables for today
                    //GridWeb1.WebWorksheets[day1].SetAllCellsReadonly();
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(7, 1, 11, 1);

                    //GridWeb1.WebWorksheets[day1].SetEditableRange(22, 1, 11, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(36, 1, 11, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(50, 0, 8, 2);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(62, 1, 2, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(67, 1, 6, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(74, 1, 1, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(77, 1, 3, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(88, 1, 5, 1);
                    //GridWeb1.WebWorksheets[day1].SetEditableRange(95, 0, 10, 2);

                    //GridWeb1.WebWorksheets[GridWeb1.WebWorksheets.Count - 1].SetAllCellsReadonly();

                } //end cell locking



                //fill all files and bind
                var files =
                    Directory.GetFiles(HttpRuntime.AppDomainAppPath + "SavedFiles")
                        .OrderByDescending(d => new FileInfo(d).LastWriteTime).ToSafeReadOnlyCollection();


                /*List<string> justFileNames = new List<string>();*/
                List<string> shoreditchFileNames = new List<string>();
                List<string> sohoFileNames = new List<string>();
                List<string> londonFileNames = new List<string>();
                List<string> holbornFileNames = new List<string>();
                List<string> stratfordFileNames = new List<string>();
                List<string> radioFileNames = new List<string>();

                foreach (var file in files)
                {
                    if (Path.GetFileName(Path.GetFileName(file).ToLower()).Contains("shoreditch"))
                    {
                        //do shore
                        shoreditchFileNames.Add(file);
                    }
                    if (Path.GetFileName(Path.GetFileNameWithoutExtension(file).ToLower()).Contains("soho"))
                    {
                        //do shore
                        sohoFileNames.Add(file);
                    }
                    if (Path.GetFileName(Path.GetFileNameWithoutExtension(file).ToLower()).Contains("london"))
                    {
                        londonFileNames.Add(file);
                        //do shore
                    }
                    if (Path.GetFileName(Path.GetFileNameWithoutExtension(file).ToLower()).Contains("holborn"))
                    {
                        holbornFileNames.Add(file);
                        //do shore
                    }
                    if (Path.GetFileName(Path.GetFileNameWithoutExtension(file).ToLower()).Contains("stratford"))
                    {
                        stratfordFileNames.Add(file);
                        //do shore
                    }
                    if (Path.GetFileName(Path.GetFileNameWithoutExtension(file).ToLower()).Contains("radio"))
                    {
                        radioFileNames.Add(file);
                        //do shore
                    }

                }

                //shore
                ListBox1.DataSource = shoreditchFileNames;
                ListBox1.DataBind();

                //soho
                ListBox2.DataSource = sohoFileNames;
                ListBox2.DataBind();

                //london
                ListBox3.DataSource = londonFileNames;
                ListBox3.DataBind();

                //holborn
                ListBox4.DataSource = holbornFileNames;
                ListBox4.DataBind();

                //stratford
                ListBox5.DataSource = stratfordFileNames;
                ListBox5.DataBind();

                //radio
                ListBox6.DataSource = radioFileNames;
                ListBox6.DataBind();


                //set filName
                var array = ((string)Session["activeFilename"]).Split('\\');
                var filename = array[array.Count() - 1];



                Label1.Text = "NOW EDITING:";
                Label2.Text = filename.Split(' ').ElementAt(0) + " " + filename.Split(' ').ElementAt(1);
                Label3.Text = filename.Split('-').ElementAt(1);

                Session["WebGrid"] = GridWeb1;

                SheetName.Text = filename.Split(' ').ElementAt(0) + " " + filename.Split(' ').ElementAt(1);
            }
        }






        [System.Web.Services.WebMethod]
        public string OpenCashupFile(string filename)
        {
            var path = HttpRuntime.AppDomainAppPath + "//SavedFiles//" + filename;
            Session["activeFilename"] = path;
            /*Response.Redirect(Request.RawUrl);*/
            return path;
        }



        protected int SaveSheet()
        {

            var cont = new EmailController();
            cont.SaveSheet(GridWeb1);

            return 0;

        }

        
        [System.Web.Services.WebMethod]
        public static void SaveCurrentWorksheet(string sheetPath)
        {
            
        }

        protected void GridWeb1_SaveCommand(object sender, EventArgs e)
        {

            //SaveSheet();

            var path = (string)Session["activeFilename"];

            GridWeb1.WebWorksheets.SaveToExcelFile(path, FileFormatType.Excel2007);

            Response.Redirect("~/Pages/Cashup.aspx");
           

        }

        [WebMethod]
        public string RunAllFormulas()
        {

            GridWeb1.WebWorksheets.RunAllFormulas();

            return "ok";

        }

        protected int DownloadFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            long sz = fi.Length;

            Response.ClearContent();
            Response.ContentType = Path.GetExtension(filePath);
            Response.AddHeader("Content-Disposition",
                string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(filePath)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.TransmitFile(filePath);
            Response.End();

            return 0;
        }

        protected void EmailSheet(object sender, EventArgs e)
        {
            GridWeb1_SaveCommand(sender, e);

            
        }
    }
}
