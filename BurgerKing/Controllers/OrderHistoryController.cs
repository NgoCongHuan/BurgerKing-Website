using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;

namespace BurgerKing.Controllers
{
    public class OrderHistoryController : Controller
    {
        BurgerKingDBContext dBContext = new BurgerKingDBContext();

        // GET: OrderHistory
        public ActionResult Index(string PhoneNumber)
        {

            return View();
        }
    }
}