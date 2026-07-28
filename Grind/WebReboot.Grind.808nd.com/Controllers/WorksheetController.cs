using System.Web.Helpers;
using Aspose.Cells.GridWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aspose.Cells.GridWeb.Data;

namespace Web.Grind._808nd.com.Controllers
{
    public class WorksheetController : Controller
    {
        public WorksheetController()
        {
                
        }
        //
        // GET: /Worksheet/
        [HttpGet]
        public void SaveCurrentWorksheet(Aspose.Cells.GridWeb.GridWeb GridWeb1)
        {

          
            string path = (string)Session["activeFilename"];

            var grid = (GridWeb)Session["WebGrid"];

            grid.WebWorksheets.SaveToExcelFile("H:\\test\\TestDog.xlsx", FileFormatType.Excel2007);
           
            Response.Redirect("~/Pages/Cashup.aspx");
         

        }
	}
}