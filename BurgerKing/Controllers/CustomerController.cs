using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Net;
using System.Data.Entity;
using System.IO;

namespace BurgerKing.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        BurgerKingDBContext dbContext = new BurgerKingDBContext();

        // GET: Customer/Edit/
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = dbContext.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Customer/Edit/
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,Name,Email,Phone,Password,RoleId,Address")] Account account, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Xử lý hình ảnh
                    string filename = Path.GetFileName(ImageFile.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);

                    account.Image = _filename;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (ImageFile.ContentLength <= 10000000)
                        {
                            // Tiếp tục với việc cập nhật thông tin tài khoản
                            account.RoleId = dbContext.Roles.FirstOrDefault(r => r.RoleName == "Customer").RoleId;
                            dbContext.Entry(account).State = EntityState.Modified;
                            dbContext.SaveChanges();

                            // Lưu hình ảnh sau khi đã lưu thay đổi vào cơ sở dữ liệu
                            ImageFile.SaveAs(path);

                            ViewBag.msg = "Record Added";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.msg = "File size is not valid (should be less than or equal to 1MB)";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "File format is not valid (only JPG, JPEG, and PNG are allowed)";
                    }
                }
                else
                {
                    ViewBag.msg = "Please select a file";
                }
            }

            // In ra thông tin lỗi cụ thể
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            // Nếu ModelState không hợp lệ, quay lại view với model và hiển thị lỗi
            return View(account);
        }


        public ActionResult OrderHistory(int? id)
        {
            // Lấy ra email của người dùng ứng với id truyền vào
            var email = dbContext.Accounts.Where(acc => acc.Id == id).Select(acc => acc.Email).FirstOrDefault();

            // Lấy ra những đơn hàng của người dùng trùng với email
            List<Order> orders = dbContext.Orders.Where(o => o.CustomerEmail == email).ToList();

            // Tính tổng số tiền của từng đơn hàng và gán cho ViewBag
            Dictionary<string, double?> orderTotalPrices = new Dictionary<string, double?>();

            foreach (var order in orders)
            {
                var orderDetails = dbContext.OrderDetails.Where(ord => ord.OrderId == order.OrderId).ToList();

                // Tính tổng số tiền của mỗi đơn hàng
                double? orderTotalPrice = orderDetails.Sum(detail => detail.Price * detail.Quantity);

                // Gán tổng số tiền của đơn hàng vào Dictionary
                orderTotalPrices.Add(order.OrderId, orderTotalPrice);
            }

            ViewBag.OrderTotalPrices = orderTotalPrices;

            return View(orders);
        }
    }
}