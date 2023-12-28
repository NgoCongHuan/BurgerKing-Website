using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Data.Entity;

namespace BurgerKing.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(int categoryId = 1)
        {
            BurgerKingDBContext dBContext = new BurgerKingDBContext();
            List<Product> Listproducts = dBContext.Products.Include(cat => cat.Category).Where(pro => pro.CatId == categoryId).ToList();
            return View(Listproducts);
        }

        public ActionResult Detail(int id)
        {
            BurgerKingDBContext dBContext = new BurgerKingDBContext();
            Product_Category product = dBContext.Product_Category.FirstOrDefault(x => x.ProId == id);
            
            // Truyền thông tin các đánh giá của sản phẩm cho ViewBag
            var reviews = dBContext.ProductReviews.Where(r => r.ProId == id).Include(r => r.Account).ToList();
            ViewBag.Reviews = reviews;

            // Truyền đối tượng model cho view
            return View(product);
        }

        public ActionResult CreateReview(FormCollection reviewForm)
        {
            var review = new BurgerKing.Models.ProductReview()
            {
                ReviewDate = DateTime.Now
            };
            return View();
        }
    }
}