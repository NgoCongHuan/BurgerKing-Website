using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Net;
using System.Data.Entity;
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
        public ActionResult Index([Bind(Include = "Id,Name,Email,Phone,Password,RoleId")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.RoleId = dbContext.Roles.FirstOrDefault(r => r.RoleName == "Customer").RoleId;
                dbContext.Entry(account).State = EntityState.Modified;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
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