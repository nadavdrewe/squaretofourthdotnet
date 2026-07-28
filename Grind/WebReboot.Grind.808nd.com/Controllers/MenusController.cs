using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aspose.Email.Exchange.Schema;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;
using Revel._808nd.com.Models.ViewModels;
using WebReboot.Grind._808nd.com.Models.ViewModels;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class MenusController : Controller
    {
        private GrindContext db = new GrindContext();

        public ActionResult Upload()
        {

            for (int i = 0; i < Request.Files.Count; i++)
            {

                var existingBlobs = (List<MenuFile>)Session["uploads"];
                var file = Request.Files[i];

                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] binData = b.ReadBytes(file.ContentLength);

                existingBlobs.Add(new MenuFile
                {
                    filename = file.FileName.Split('.')[0],
                    extension = "." + file.FileName.Split('.')[1],
                    bytes = binData

                });

                Session["uploads"] = existingBlobs;

            }

            return Json(new { Message = "A new file was uploaded" });

        }

        /// <summary>
        /// Takes a menu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="extension"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Show(int id, string name = "", string type = "show")
        {
            var allFiles = db.MenuFiles.Include(x => x.Menu)
                .Where(x => x.Menu.id == id).ToList();

            var imageData = db.MenuFiles.Find(id);


            if (imageData != null)
            {
                var extension = imageData.extension;
                var fullName = (imageData.filename + imageData.extension);
                if (type == "show")
                {
                    Response.ClearHeaders();
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + fullName);
                }
                else
                {
                    Response.ClearHeaders();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + fullName);
                }

                //strategy
                if (extension == ".jpg")
                {
                    return File(imageData.bytes, "image/jpeg");
                }
                else if (extension == ".gif")
                {
                    return File(imageData.bytes, "image/gif");
                }
                else if (extension == ".png")
                {
                    return File(imageData.bytes, "image/png");
                }
                else
                {
                    return File(imageData.bytes, MimeMapping.GetMimeMapping(fullName));
                }
            }

            else return HttpNotFound("Could not find the file");
        }


        public ActionResult ShowMenu(string grind, string name)
        {
            try
            {
                var establishment = db.Establishments.FirstOrDefault(x => x.name.ToLower().Trim() == grind.Trim().ToLower());

                if (establishment != null)
                {
                    var type = db.MenuTypes.Where(x => x.name.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();

                    if (type != null)
                    {

                        //check there are permissions for the type and establishment, else redirect to error
                        if (db.MenuPermissions.Where(
                            x => x.Establishment.DBKEY_establishment_id == establishment.DBKEY_establishment_id
                                 && x.MenuType.id == type.id
                            ).ToList().Count > 0)
                        {

                            var menus =
                                db.Menus.Where(
                                    x =>
                                        x.MenuType.id == type.id &&
                                        x.Establishment.DBKEY_establishment_id == establishment.DBKEY_establishment_id)
                                    .ToList();

                            if (menus != null)
                            {
                                var latestMenu = menus.OrderByDescending(x => x.WhenCreated).First();

                                var imageData = db.MenuFiles
                                    .Where(x => x.extension.Equals(".pdf") || x.extension.Equals(".jpg"))
                                    .Where(x => x.Menu.id == latestMenu.id).FirstOrDefault();


                                if (imageData != null)
                                {
                                    var extension = imageData.extension;
                                    var fullName = (imageData.filename + imageData.extension);

                                    Response.ClearHeaders();
                                    Response.AppendHeader("Content-Disposition", "inline; filename=" + fullName);

                                    return File(imageData.bytes, MimeMapping.GetMimeMapping(fullName));
                                }
                            }
                        }
                        else
                        {
                            return RedirectToAction("NotFound");
                        }
                    }

                }
                return HttpNotFound("Could not find the file");
            }
            catch (Exception ex)
            {
                //return a not found page
                throw;
            }
        }




        // GET: Menus
        [HttpGet]
        public ActionResult Index()
        {
            var filesNoBytes = db.MenuFiles.Include(x => x.Menu.Establishment).Select(x => new MenuFileDTO
            {
                bytes = null,
                extension = x.extension,
                id = x.id,
                menuid = x.Menu.id,
                filename = x.filename,
                url = x.url
            }).ToList();

            var menus = db.Menus.OrderByDescending(x => x.WhenCreated).ToList();

            ViewBag.Files = filesNoBytes;

            PopulateViewBag();

            return View(menus);
        }


        [HttpPost]
        public ActionResult Index(Establishment Establishment, MenuType MenuType)
        {
            var filesNoBytes = db.MenuFiles.Include(x => x.Menu.Establishment)
                .Select(x => new MenuFileDTO
                {
                    bytes = null,
                    extension = x.extension,
                    id = x.id,
                    menuid = x.Menu.id,
                    filename = x.filename,
                    url = x.url
                }).ToList();

            var menus = db.Menus
                .Where(x => x.Establishment.establishment_id == Establishment.establishment_id)
                .Where(x => x.MenuType.id == MenuType.id)
                .OrderByDescending(x => x.WhenCreated).ToList();

            ViewBag.Files = filesNoBytes;
            PopulateViewBag();
            return View(menus);
        }



        // GET: Menus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            PopulateViewBag();

            return View();
        }

        private void PopulateViewBag()
        {
            var permissionsforTypes = db.MenuPermissions.Include(x=>x.Establishment).Include(x=>x.MenuType).ToList();
            var allTypes = db.MenuTypes.ToList();
            var allowedTypes = new List<EstablishmentMenuPermissionViewModel>();

            foreach (var perm in permissionsforTypes)
            {
                allowedTypes.Add(new EstablishmentMenuPermissionViewModel
                {
                    Establishment_Id = perm.Establishment.establishment_id,
                    MenuTypeName = perm.MenuType.name,
                    MenuTypeId = perm.MenuType.id
                });
            }


            ViewBag.EstablishmentMenuPermissionsViewModel = allowedTypes;

            ViewBag.MenuTypes = db.MenuTypes.ToList().Select(menu => new SelectListItem
            {
                Text = menu.name,
                Value = Convert.ToString(menu.id)

            }).ToList();

            ViewBag.Establishments = db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem
            {
                Text = x.name,
                Value = Convert.ToString(x.establishment_id)

            }).ToList(); ;

            ViewBag.Estabs = db.Establishments.Where(x => x.establishment_id != 2).ToList();
            ViewBag.MenuPermissions = db.MenuPermissions.ToList();

            ViewBag.StandardMenus = db.MenuTypes.Where(x => x.name == "Bar" || x.name == "Breakfast").ToList().Select(menu => new SelectListItem
            {
                Text = menu.name,
                Value = Convert.ToString(menu.id)

            }).ToList();

            ViewBag.LondonMenu = db.MenuTypes.Where(x => x.name == "Bar"
            || x.name == "Breakfast"
            || x.name == "Brunch"
            || x.name == "AllDay"
            || x.name == "Dessert"

            ).ToList().Select(menu => new SelectListItem
            {
                Text = menu.name,
                Value = Convert.ToString(menu.id)

            }).ToList();
        }


        // POST: Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EstablishmentMenuPermissionViewModel vm)
        {

            var menu = new Menu();

            menu.WhenCreated = DateTime.Now;
            menu.WhoUploaded = User.Identity.Name;
            menu.Establishment = db.Establishments.First(x => x.establishment_id == vm.Establishment_Id);
            menu.MenuType = db.MenuTypes.First(x => x.id.Equals(vm.MenuTypeId));

            menu.MenuFiles = Session["uploads"] as List<MenuFile>;

            if (ModelState.IsValid)
            {
                db.Menus.Add(menu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menu);
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,WhenCreated,WhoUploaded")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menu);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Menu menu = db.Menus.Find(id);
            var files = db.MenuFiles.Include(x => x.Menu).Where(x => x.Menu.id == id).ToList();

            db.MenuFiles.RemoveRange(files);
            db.Menus.Remove(menu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}
