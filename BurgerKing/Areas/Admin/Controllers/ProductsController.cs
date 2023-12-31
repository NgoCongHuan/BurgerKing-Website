using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.IO;

namespace BurgerKing.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private BurgerKingDBContext db = new BurgerKingDBContext();

        // GET: Admin/Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
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

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CatId = new SelectList(db.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, Product product)
        {

            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);

                // Đặt tên ảnh cho Product
                product.ProImage = _filename;

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (file.ContentLength <= 1000000)
                    {

                        TryUpdateModel(product); 
                        if (ModelState.IsValid)
                        {
                            db.Products.Add(product);
                            db.SaveChanges();
                            file.SaveAs(path);
                            ViewBag.msg = "Record Added";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Kích thước tệp không hợp lệ (phải nhỏ hơn hoặc bằng 1MB)";
                    }
                }
                else
                {
                    ViewBag.msg = "Định dạng tệp không hợp lệ (chỉ chấp nhận các định dạng JPG, JPEG và PNG)";
                }
            }
            else
            {
                ViewBag.msg = "Vui lòng chọn tệp";
            }

            // Hiển thị Drop Down List
            ViewBag.CatId = new SelectList(db.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }


        // GET: Admin/Products/Edit/5
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
            ViewBag.CatId = new SelectList(db.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);

                // Đặt tên ảnh cho Product
                product.ProImage = _filename;

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (file.ContentLength <= 1000000)
                    {
                        if (ModelState.IsValid)
                        {
                            db.Entry(product).State = EntityState.Modified;
                            db.SaveChanges();
                            file.SaveAs(path);
                            ViewBag.msg = "Record Added";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Kích thước tệp không hợp lệ (phải nhỏ hơn hoặc bằng 1MB)";
                    }
                }
                else
                {
                    ViewBag.msg = "Định dạng tệp không hợp lệ (chỉ chấp nhận các định dạng JPG, JPEG và PNG)";
                }
            }
            else
            {
                ViewBag.msg = "Vui lòng chọn tệp";
            }
            
            ViewBag.CatId = new SelectList(db.Categories, "CatId", "CatName", product.CatId);
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
    }
}
