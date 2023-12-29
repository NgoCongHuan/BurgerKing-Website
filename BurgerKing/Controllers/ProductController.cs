using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Data.Entity;
using Newtonsoft.Json;

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

            // Lấy mã sản phẩm ra ViewBag để làm tham số truyền vào action
            ViewBag.ProId = product.ProId;

            // Lấy tổng số lượng đánh giá để hiển thị ra ViewBag
            ViewBag.TotalComment = dBContext.ProductReviews.Where(p => p.ProId == id).Count();

            // Tính trung bình số sao của tất cả lượt đánh giá và in ra ViewBag
            var averageRating = dBContext.ProductReviews.Where(p => p.ProId == id).Select(p => p.Rating);
            // Kiểm tra sự tồn tại
            if (averageRating.Any())
            {
                ViewBag.AverageRating = averageRating.Average();
            }
            else
            {
                ViewBag.AverageRating = 0;
            }

            // Truyền thông tin các đánh giá của sản phẩm cho ViewBag
            var reviews = dBContext.ProductReviews.Where(r => r.ProId == id).Include(r => r.Account).ToList();
            ViewBag.Reviews = reviews;

            // Truyền đối tượng model cho view
            return View(product);
        }

        public ActionResult CreateReview(FormCollection reviewForm, int ProId)
        {
            string accountInfo = Request.Cookies["AccountInfo"]?.Value;

            if (!string.IsNullOrEmpty(accountInfo))
            {
                Account acc = JsonConvert.DeserializeObject<Account>(accountInfo);

                if (acc != null)
                {
                    int AccId = acc.Id;

                    var review = new BurgerKing.Models.ProductReview()
                    {
                        ProId = ProId,
                        AccId = AccId,
                        Rating = Convert.ToInt32(reviewForm["Rating"]),
                        Comment = reviewForm["Comment"],
                        ReviewDate = DateTime.Now
                    };

                    // Lưu đánh giá vào cơ sở dữ liệu
                    BurgerKingDBContext dBContext = new BurgerKingDBContext();
                    dBContext.ProductReviews.Add(review);
                    dBContext.SaveChanges();

                    return RedirectToAction("Detail", new { id = ProId });
                }
            }

            return RedirectToAction("Detail", new { id = ProId });
        }
    }
}