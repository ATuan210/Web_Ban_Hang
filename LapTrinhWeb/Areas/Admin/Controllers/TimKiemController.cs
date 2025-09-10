using LapTrinhWeb.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Areas.Admin.Controllers
{
    public class TimKiemController : Controller
    {
        private Product_DBContext db = new Product_DBContext();

        // POST: Admin/TimKiem/KetQuaTimKiem
        [HttpPost]
        public ActionResult KetQuaTimKiem(string txtTimKiem)
        {
            if (string.IsNullOrEmpty(txtTimKiem))
            {
                TempData["ThongBao"] = "Vui lòng nhập từ khóa!";
                return RedirectToAction("KetQuaTimKiem"); // redirect GET
            }

            // Redirect sang GET, truyền keyword qua query string
            return RedirectToAction("KetQuaTimKiem", new { txtTimKiem });
        }

        // GET: Admin/TimKiem/KetQuaTimKiem
        [HttpGet]
        public ActionResult KetQuaTimKiem(string txtTimKiem, int? page)
        {
            var kq = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(txtTimKiem))
            {
                kq = kq.Where(p => p.ProName.Contains(txtTimKiem));
            }
            else if (TempData["ThongBao"] != null)
            {
                ViewBag.ThongBao = TempData["ThongBao"];
            }

            int pageSize = 6;
            int pageNumber = page ?? 1;

            return View(kq.OrderBy(p => p.ProId).ToPagedList(pageNumber, pageSize));
        }
        // GET: Admin/Products/Details/5
        //[Authorize(Roles = "Admin, User")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Products
                            .Include(p => p.Category)   // load luôn Category
                            .FirstOrDefault(p => p.ProId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }



        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatId = new SelectList(db.Categories, "Id", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProId,ProName,ProImage,ProPrice,CatId,ProBreed,ProColor,ProAge,ProGender,ProVaccination,ProDeworming,ProHealth,ProShipping,Status,Origin,Quantity")] Product product, HttpPostedFileBase imgfile)
        {

            if (ModelState.IsValid)
            {
                var productInDb = db.Products.AsNoTracking().FirstOrDefault(p => p.ProId == product.ProId);

                if (productInDb == null)
                {
                    return HttpNotFound();
                }

                if (imgfile != null && imgfile.ContentLength > 0)
                {
                    // Nếu có ảnh mới thì upload và update
                    product.ProImage = FileUpload(imgfile);
                }
                else
                {
                    // Nếu không có ảnh mới thì giữ ảnh cũ
                    product.ProImage = productInDb.ProImage;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CatId = new SelectList(db.Categories, "Id", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        public string FileUpload(HttpPostedFileBase file)
        {
            if (file == null) return null;

            string folder = Server.MapPath("~/images/");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filename = Path.GetFileName(file.FileName);
            string fullPath = Path.Combine(folder, filename);

            file.SaveAs(fullPath);

            return "/images/" + filename;
        }
    }
}
