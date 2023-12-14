using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Net;
using BurgerKing.Others;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Mail;
using Common;
using Newtonsoft.Json;

namespace BurgerKing.Controllers
{
    public class ShoppingCartController : Controller
    {
        private BurgerKingDBContext dbContext = new BurgerKingDBContext();
        private string strCart = "Cart";
        private int CalculateCartItemCount()
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            return ListCart != null ? ListCart.Sum(c => c.Quantity) : 0;
        }
        
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetCartItemCount()
        {
            int itemCount = CalculateCartItemCount();
            return Json(new { itemCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderNow(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Session[strCart] == null)
            {
                List<Cart> ListCart = new List<Cart>
                {
                    new Cart(dbContext.Products.Find(Id), 1)
                };
                Session[strCart] = ListCart;    
            }
            else
            {
                List<Cart> ListCart = (List<Cart>)Session[strCart];
                int check = IsExistingCheck(Id);
                if (check == -1)
                    ListCart.Add(new Cart(dbContext.Products.Find(Id), 1));
                else
                    ListCart[check].Quantity++;

                // Cập nhật số lượng sản phẩm trong giỏ hàng
                Session[strCart] = ListCart;
            }

            return RedirectToAction("Index");
        }

        public ActionResult OrderNowQuantity(int? Id, int Quantity)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Session[strCart] == null)
            {
                List<Cart> ListCart = new List<Cart>
                {
                    new Cart(dbContext.Products.Find(Id), Quantity)
                };
                Session[strCart] = ListCart;
            }
            else
            {
                List<Cart> ListCart = (List<Cart>) Session[strCart];
                int check = IsExistingCheck(Id);
                if (check == -1)
                    ListCart.Add(new Cart(dbContext.Products.Find(Id), Quantity));
                else
                    ListCart[check].Quantity += Quantity;

                // Cập nhật số lượng sản phẩm trong giỏ hàng
                Session[strCart] = ListCart;
            }

            return RedirectToAction("Index");
        }

        private int IsExistingCheck(int? Id)
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                if (ListCart[i].Product.ProId == Id)
                    return i;
            }
            return -1;
        }

        public ActionResult RemoveItem(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int check = IsExistingCheck(Id);
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            ListCart.RemoveAt(check);
            if (ListCart.Count == 0)
            {
                Session[strCart] = null;
            }
            else
            {
                Session[strCart] = ListCart;
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateCart(FormCollection field)
        {
            string[] quantities = field.GetValues("quantity");
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                ListCart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            Session[strCart] = ListCart;

            return RedirectToAction("Index");
        }
        public ActionResult ClearCart()
        {
            Session[strCart] = null;

            return RedirectToAction("Index");
        }
        public ActionResult CheckOut()
        {
            string accountInfo = Request.Cookies["AccountInfo"]?.Value;

            if (!string.IsNullOrEmpty(accountInfo))
            {
                Account acc = JsonConvert.DeserializeObject<Account>(accountInfo);

                if (acc != null)
                {
                    ViewBag.Name = acc.Name;
                    ViewBag.Email = acc.Email;
                    ViewBag.Phone = acc.Phone;
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult ProcessOrder(FormCollection field)
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];

            //1. Lưu Hóa đơn vào bảng Order
            var order = new BurgerKing.Models.Order()
            {
                CustomerName = field["cusName"],
                CustomerPhone = field["cusPhone"],
                CustomerEmail = field["cusEmail"],
                CustomerAddress = field["cusAddress"],
                OrderDate = DateTime.Now,
                OrderNote = field["ordNote"],
                PaymentType = "",
                Status = OrderStatus.Processing
            };
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();

            // Gán giá trị của OrderId
            var OrderId = order.OrderId;

            // ViewBag hiển thị OrderId
            ViewBag.OrderId = OrderId;

            // Tính tổng số tiền đơn hàng
            var TotalPrice = ListCart.Sum(n => n.Product.ProPrice * n.Quantity);

            // ViewBag hiển thị tổng số tiền của đơn hàng
            ViewBag.TotalPrice = TotalPrice;

            //2. Lưu Chi tiết hóa đơn vào bảng OrderDetail
            foreach (Cart cart in ListCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.OrderId,
                    ProductId = cart.Product.ProId,
                    Quantity = Convert.ToInt32(cart.Quantity),
                    Price = Convert.ToDouble(cart.Product.ProPrice)
                };
                dbContext.OrderDetails.Add(orderDetail);
                dbContext.SaveChanges();
            }

            return View("OrderSuccess");
        }
    }
}