using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;

namespace BurgerKing.Controllers
{
    public class HomeController : Controller
    {
        private BurgerKingDBContext dbContext = new BurgerKingDBContext();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.OrderQuantity = dbContext.Orders.Count();
            return View();
        }
    }
}