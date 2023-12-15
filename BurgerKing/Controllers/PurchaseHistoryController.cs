using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Net;

namespace BurgerKing.Controllers
{
    public class PurchaseHistoryController : Controller
    {
        BurgerKingDBContext db = new BurgerKingDBContext();

        // GET: OrderHistory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewOrder(string id)
        {
            // Khởi tạo 2 đối tượng Order và OrderDetail tương ứng
            Order order = db.Orders.Find(id);
            List<OrderDetail> orderDetails = db.OrderDetails.Where(o => o.OrderId == id).ToList();

            // Kiểm tra nếu là null thì trả về lỗi
            if (order == null)
            {
                // Return a partial view with a message indicating no order was found
                return PartialView("_OrderNotFound");
            }

            // Tính tổng số tiền thanh toán
            int? totalPrice = (int?)db.OrderDetails.Where(o => o.OrderId == id).Sum(o => o.Price * o.Quantity);
            ViewBag.TotalPrice = totalPrice;

            // Khởi tạo đối tượng Order_OrderDetail
            Order_OrderDetail ord_ordDetails = new Order_OrderDetail()
            {
                Order = order,
                OrderDetails = orderDetails
            };

            // Trả về Partial View nếu có đơn hàng
            return PartialView("_ViewOrder", ord_ordDetails);
        }
    }
}