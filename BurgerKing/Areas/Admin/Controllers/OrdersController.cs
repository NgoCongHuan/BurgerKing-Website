using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using PagedList;

namespace BurgerKing.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private BurgerKingDBContext db = new BurgerKingDBContext();

        /// GET: Admin/Orders
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var orders = db.Orders.OrderBy(p => p.OrderId).ToPagedList(pageNumber, pageSize);
            return View(orders); // Return the 'orders' variable instead of 'db.Orders.ToList()'
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy cả thông tin Order, OrderDetails và Product
            Order order = db.Orders.Include(o => o.OrderDetails.Select(od => od.Product)).SingleOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            // Tạo danh sách SelectListItem từ hằng số trong lớp OrderStatus
            var orderStatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = OrderStatus.Canceled, Text = "Đã hủy đơn hàng" },
                new SelectListItem { Value = OrderStatus.Failed, Text = "Đã xảy ra lỗi trong quá trình thanh toán" },
                new SelectListItem { Value = OrderStatus.Processing, Text = "Đang chờ thanh toán" },
                new SelectListItem { Value = OrderStatus.Completed, Text = "Đã thanh toán" }
            };

            // Gán danh sách vào ViewBag.Status
            ViewBag.Status = new SelectList(orderStatusList, "Value", "Text", order.Status);

            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,OrderName,OrderDate,PaymentType,Status,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
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
