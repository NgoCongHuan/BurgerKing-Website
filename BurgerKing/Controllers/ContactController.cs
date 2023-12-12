using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BurgerKing.Models;
using System.Configuration;
using System.Net.Mail;
using Common;
using System.Net;

namespace BurgerKing.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendContact(FormCollection field)
        {
            try
            {
                var contact = new BurgerKing.Models.Contact()
                {
                    name = field["name"],
                    email = field["email"],
                    subject = field["subject"],
                    message = field["message"]
                };

                // Gửi mail cho khách hàng
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Areas/client/template/newcontact.html"));

                content = content.Replace("{{Name}}", contact.name);
                content = content.Replace("{{Email}}", contact.email);
                content = content.Replace("{{Subject}}", contact.subject);
                content = content.Replace("{{Message}}", contact.message);
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                new MailHelper().SendMail(contact.email, "Phản hồi mới từ khách hàng của Burger King", content);
                new MailHelper().SendMail(toEmail, "Phản hồi mới từ khách hàng của Burger King", content);

                return View("Index");
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý lỗi tại đây
                // Ví dụ:
                Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
                // Hoặc ghi vào log
                // logger.Error("Lỗi khi gửi email", ex);

                // Có thể chuyển hướng hoặc hiển thị thông báo lỗi tùy thuộc vào yêu cầu của bạn
                return View("Error"); // Chuyển hướng đến trang lỗi
            }
        }
    }
}