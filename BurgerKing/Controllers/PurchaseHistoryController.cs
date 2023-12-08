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

        public ActionResult ViewOrder (int? id)
        {
            Order order = db.Orders.Find(id);

            if (order == null)
            {
                // Return a partial view with a message indicating no order was found
                return PartialView("_OrderNotFound");
            }

            // Return the partial view with the order data
            return PartialView("_ViewOrder", order);
        }
    }
}