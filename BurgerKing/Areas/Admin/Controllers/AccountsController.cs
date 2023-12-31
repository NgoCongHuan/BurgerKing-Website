using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;

namespace BurgerKing.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private BurgerKingDBContext db = new BurgerKingDBContext();

        // GET: Admin/Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Role);
            return View(accounts.ToList());
        }

        // GET: Admin/Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account account, HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);

                // Tên ảnh cho Image Account
                account.Image = _filename;

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (file.ContentLength <= 1000000)
                    {
                        
                        TryUpdateModel(account); 
                        if (ModelState.IsValid)
                        {
                            db.Accounts.Add(account);
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

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account account, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);

                // Tên ảnh cho Image Account
                account.Image = _filename;

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (file.ContentLength <= 1000000)
                    {

                        TryUpdateModel(account);
                        if (ModelState.IsValid)
                        {
                            db.Entry(account).State = EntityState.Modified;
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

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
