using Aspose.Cells;
using Revel._808nd.com.CaternetData;
using Revel._808nd.com.CaternetData.Models;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class CSVImportController : Controller
    {
        static string path = HttpRuntime.AppDomainAppPath + @"CSV\Fourth\";
        static string filenameOutput = String.Format("CaternetXML_{0}.csv", DateTime.Now.ToShortDateString());
        static string tempFilenameOutput = "tempProcessFile.csv";
        static string fullPath = path + filenameOutput;
        static string oneOffPRocessPath = path + tempFilenameOutput;

        private static string fullXmlPath = path + "latestCaternetXML.xml";
        // GET: CSVImport
        [HttpGet]
        public ActionResult ConvertToFourthCSV()
        {

            return View();
        }

        public ActionResult UploadCSV()
        {

            using (var db = new GrindContext())
            {
                // var prods = db.Products.ToList();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var file = Request.Files[0];
                file.SaveAs(fullPath);
                //BinaryReader b = new Files(file.InputStream);
                //byte[] binData = b.ReadBytes(file.ContentLength);
                Workbook wb = new Workbook(fullPath);

                wb.Worksheets.Add("new Sheet");

                var numberofSheets = wb.Worksheets.Count;

                var revelSheet = wb.Worksheets[0];
                var fourthSheet = wb.Worksheets[1];


                //copy values between sheets and aumgente if necessary

                //PLU
                fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[4].Index,
                    fourthSheet.Cells.Columns[0].Index);
                //name
                fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[3].Index,
                    fourthSheet.Cells.Columns[1].Index);
                //gross sales
                fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[10].Index,
                    fourthSheet.Cells.Columns[4].Index);
                //sls count
                fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[8].Index,
                    fourthSheet.Cells.Columns[3].Index);

                //comps


                //voids 


                /*    //vat
                    fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[11].Index,
                        fourthSheet.Cells.Columns[4].Index);*/
                ////price
                //fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[10].Index, fourthSheet.Cells.Columns[5].Index);
                ////net sales
                //fourthSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[10].Index, fourthSheet.Cells.Columns[6].Index);
                ////rvc number

                //price
                //major group number

                var prods = db.Products.Select(product => new
                {
                    sku = product.sku,
                    price = product.price

                });

                //add blank price column
                /*fourthSheet.Cells.InsertColumn(6);*/
                fourthSheet.Cells["A1"].PutValue("PLU");
                fourthSheet.Cells["B1"].PutValue("NAME");
                fourthSheet.Cells["C1"].PutValue("PRICE");
                fourthSheet.Cells["D1"].PutValue("QTY");
                fourthSheet.Cells["E1"].PutValue("TOTAL SALES");
                //fourthSheet.Cells["F1"].PutValue("PRICE");
                //fourthSheet.Cells["G1"].PutValue("NET_SLS");
                //fourthSheet.Cells["H1"].PutValue("RVC_NUM");
                //fourthSheet.Cells["I1"].PutValue("MAJ_GRP_NUM");


                // add total sales column


                ////set column formats
                //Style style;
                //StyleFlag flag;
                //style = wb.Styles[wb.Styles.Add()];
                //style.Custom = "$#,##0.00;$-#,##0.00"; //Sets the currency format.
                //flag = new StyleFlag();
                //flag.NumberFormat = true;
                //fourthSheet.Cells.ApplyColumnStyle(3, style, flag);
                //fourthSheet.Cells.ApplyColumnStyle(4, style, flag);


                //wb.Save(@"C://test/tempTemp.xlsx", SaveFormat.Xlsx);
                //wb = new Workbook(@"C://test/tempTemp.xlsx");

                var numberOfRows = fourthSheet.Cells.MaxDataRow;

                //do prices
                for (int i = 2; i < numberOfRows; i++)
                {
                    try
                    {
                        var sku = fourthSheet.Cells["A" + i].Value;
                        if (sku != null)
                        {
                            var price = prods.FirstOrDefault(x => x.sku.ToLower().Trim() == sku.ToString().ToLower().Trim());

                            var currentIndex = String.Format("C{0}", i);

                            if (price != null)
                            {
                                try
                                {
                                    fourthSheet.Cells[currentIndex].PutValue(price.price);
                                }
                                catch (Exception ex)
                                {

                                    fourthSheet.Cells[currentIndex].PutValue(0.00);
                                }
                            }


                        }

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }

                //do net sales
                /* for (int i = 2; i < numberOfRows + 2; i++)
                 {
                     if (i == numberOfRows)
                     {

                     }
                     var currentRowSalesIndex = fourthSheet.Cells[String.Format("C{0}", i)].Value.ToString();
                     var currentRowTaxIndex = fourthSheet.Cells[String.Format("E{0}", i)].Value.ToString();

                     var netForRow = Convert.ToDecimal(currentRowSalesIndex) - Convert.ToDecimal(currentRowTaxIndex);

                     var currentIndex = String.Format("G{0}", i);
                     fourthSheet.Cells[currentIndex].PutValue(netForRow);
                 }*/

                //Save the workbook in xls format
                wb.Worksheets.RemoveAt(0);
                var fileName = (DateTime.Now.ToString("yyyyMMdd")) + ".csv";
                var returnPath = path + fileName;
                wb.Save(returnPath, SaveFormat.CSV);

                Response.StatusCode = 200;
                return Json(fileName);
            }
        }


        public ActionResult DownloadFile()
        {

            string filepath = @"C:/test/output.csv";
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "test.csv",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }



        //private async Task ConvertToFourth(Workbook wb)
        //{

        //    wb.Worksheets.Add();
        //    var DateTimeAppend = DateTime.Now;
        //    wb.Save(String.Format("C://test/fourth{0}.csv", DateTimeAppend) + FileFormatType.CSV);

        //    var numberofSheets = wb.Worksheets.Count;

        //    var revelSheet = wb.Worksheets[0];
        //    var fourthSheet = wb.Worksheets[numberofSheets - 1];


        //    //copy values between sheets and aumgente if necessary

        //    //PLU
        //    revelSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[4].Index, fourthSheet.Cells.Columns[0].Index);
        //    revelSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[3].Index, fourthSheet.Cells.Columns[1].Index);
        //    revelSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[10].Index, fourthSheet.Cells.Columns[2].Index);
        //    revelSheet.Cells.CopyColumn(revelSheet.Cells, revelSheet.Cells.Columns[10].Index, fourthSheet.Cells.Columns[3].Index);


        //    ////new Fourth CSV
        //    //var fourthWorkbook = new Workbook();
        //    //wb.Save("C://test/fourth.csv", FileFormatType.CSV);


        //}

        protected void DownloadFile(string filePath, string fileName = "latestCaternetXML.xml")
        {
            FileInfo fi = new FileInfo(filePath);
            long sz = fi.Length;

            Response.ClearContent();
            Response.ContentType = Path.GetExtension(filePath);
            Response.AddHeader("Content-Disposition",
                string.Format("attachment; filename = {0}", System.IO.Path.GetFileName(filePath)));
            Response.AddHeader("Content-Length", sz.ToString("F0"));
            Response.AddHeader("x-FileName", fileName);
            Response.TransmitFile(filePath);
            Response.End();

        }


        public ActionResult UploadCaternetXML()
        {
            return View();
        }

        public void ProcessCaternetXML()
        {

            //get the file and save it
            var file = Request.Files[0];
            file.SaveAs(oneOffPRocessPath);

            //convert using the saved file
            var pathOFCOnvertedXML = DemoCaternetXML(oneOffPRocessPath);
            DownloadFile(pathOFCOnvertedXML);
            //delete the saved file?

            //log 

            //tidy uy?
        }

        public string DemoCaternetXML(string passedPath)
        {
            Workbook wb = new Workbook(passedPath);
            var numberofSheets = wb.Worksheets.Count;

            var revelSheet = wb.Worksheets[0];
            var numberOfRows = revelSheet.Cells.MaxDataRow;


            //todo:some validation 



            IEnumerable<dynamic> prods;
            using (var db = new GrindContext())
            {
                prods = db.Products.Select(x => new
                {
                    x.sku,
                    x.price
                }).ToList();

            }//convert all rows from CSV to the proper objects
             //PLU

            var salesRow = new List<CaternetCsvRow>();
            var voidsRow = new List<CaternetCsvRow>();
            var compsRow = new List<CaternetCsvRow>();


            var allRows = new List<CaternetCsvRow>();

            for (int i = 2; i < numberOfRows + 2; i++)
            {
                try
                {
                    if (i == 4)
                    {
                        var stop = "";
                    }


                    var plu = revelSheet.Cells[String.Format("D{0}", i)].Value?.ToString() ?? "";
                    var name = revelSheet.Cells[String.Format("C{0}", i)].Value.ToString();

                    var grossSales = revelSheet.Cells[String.Format("M{0}", i)].Value.ToString();
                    var qty = revelSheet.Cells[String.Format("I{0}", i)].Value.ToString();
                    var voids = Convert.ToInt16(revelSheet.Cells[String.Format("J{0}", i)].Value.ToString());
                    var comps = Convert.ToInt16(revelSheet.Cells[String.Format("K{0}", i)].Value.ToString());
                    var VAT = revelSheet.Cells[String.Format("N{0}", i)].Value.ToString();

                    var currentProd = prods.FirstOrDefault(x => x.sku == plu);
                    var prodPrice = currentProd != null ? currentProd.price : 0.00M;

                    //add the normal row for the product
                    allRows.Add(new CaternetCsvRow
                    {
                        SKU = plu ?? "No Plu In System",
                        Name = name,
                        GrossSalesPrice = prodPrice,
                        GrossSales = Convert.ToDecimal(grossSales),
                        Quantity = Convert.ToInt32(qty),
                        NetSales = Convert.ToDecimal(grossSales) - Convert.ToDecimal(VAT),
                        VAT = Convert.ToDecimal(VAT),
                        SalesTypeRef = 0

                    });

                    ////add one to voids if there are any voids / wastage
                    //if (voids > 0)
                    //{
                    //    allRows.Add(new CaternetCsvRow
                    //    {
                    //        SKU = plu ?? "No Plu In System",
                    //        Name = name,
                    //        GrossSalesPrice = prodPrice,
                    //        GrossSales = (prodPrice * voids),
                    //        Quantity = Convert.ToInt32(voids),
                    //        NetSales = Convert.ToDecimal(prodPrice * voids) * 0.8M,
                    //        VAT = Convert.ToDecimal(VAT),
                    //        SalesTypeRef = 5
                    //    });
                    //}
                    //add one to comps if there are any comps

                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }


            var factory = new CaternetXMLFactory();
            factory.CreateXML(fullXmlPath, DateTime.Now, "9200", "9200", allRows);


            return fullXmlPath;


        }
    }
}