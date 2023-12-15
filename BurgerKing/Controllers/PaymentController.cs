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

namespace BurgerKing.Controllers
{
    public class PaymentController : Controller
    {
        private string strCart = "Cart";

        // GET: Payment
        public ActionResult Payment(string id, string total)
        {

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "test";
            string returnUrl = "http://localhost:50111/Payment/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Payment/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = total;
            string orderid = id; //mã đơn hàng;
            string requestId = id;
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i

        public ActionResult ConfirmPaymentClient(Result result)
        {
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công

            return RedirectToAction("SavePayment", new { id = rOrderId, error = rErrorCode});
        }

        public ActionResult SavePayment(string id, string error)
        {
            // Khởi tạo đối tượng Database
            BurgerKingDBContext dBContext = new BurgerKingDBContext();
            var query = from ord in dBContext.Orders where ord.OrderId == id select ord;

            // Tính tổng số tiền thanh toán
            int TotalPrice = (int)dBContext.OrderDetails.Where(o => o.OrderId == id).Sum(o => o.Price * o.Quantity);

            foreach (Order ord in query)
            {
                if (error == "0")
                {
                    // Cập nhật trạng thái và phương thức thanh toán của đơn hàng
                    ord.Status = OrderStatus.Completed;
                    ord.PaymentType = PaymentType.Bank;

                    // Gửi mail cho khách hàng
                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Areas/client/template/neworder.html"));

                    content = content.Replace("{{OrderId}}", ord.OrderId);
                    content = content.Replace("{{CustomerName}}", ord.CustomerName);
                    content = content.Replace("{{CustomerPhone}}", ord.CustomerPhone);
                    content = content.Replace("{{CustomerEmail}}", ord.CustomerEmail);
                    content = content.Replace("{{CustomerAddress}}", ord.CustomerAddress);
                    content = content.Replace("{{Status}}", ord.Status);
                    content = content.Replace("{{PaymentType}}", ord.PaymentType);
                    content = content.Replace("{{Total}}", TotalPrice.ToString());
                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                    new MailHelper().SendMail(ord.CustomerEmail, "Đơn hàng mới từ Burger King", content);
                    new MailHelper().SendMail(toEmail, "Đơn hàng mới từ Burger King", content);
                }
                else
                {
                    ord.Status = OrderStatus.Failed;
                }
            }

            try
            {
                dBContext.SaveChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            if (Session[strCart] != null)
            {
                Session.Remove(strCart);
            }

            return View();
        }

        public ActionResult PaymentCash(string id)
        {
            // Khởi tạo đối tượng Database
            BurgerKingDBContext dBContext = new BurgerKingDBContext();
            var query = from ord in dBContext.Orders where ord.OrderId == id select ord;

            // Tính tổng số tiền thanh toán
            int TotalPrice = (int)dBContext.OrderDetails.Where(o => o.OrderId == id).Sum(o => o.Price * o.Quantity);

            foreach (Order ord in query)
            {
                // Cập nhật trạng thái và phương thức thanh toán của đơn hàng
                ord.Status = OrderStatus.Processing;
                ord.PaymentType = PaymentType.Cash;

                // Gửi mail cho khách hàng
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Areas/client/template/neworder.html"));

                content = content.Replace("{{OrderId}}", ord.OrderId);
                content = content.Replace("{{CustomerName}}", ord.CustomerName);
                content = content.Replace("{{CustomerPhone}}", ord.CustomerPhone);
                content = content.Replace("{{CustomerEmail}}", ord.CustomerEmail);
                content = content.Replace("{{CustomerAddress}}", ord.CustomerAddress);
                content = content.Replace("{{Status}}", ord.Status);
                content = content.Replace("{{PaymentType}}", ord.PaymentType);
                content = content.Replace("{{Total}}", TotalPrice.ToString());
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                new MailHelper().SendMail(ord.CustomerEmail, "Đơn hàng mới từ Burger King", content);
                new MailHelper().SendMail(toEmail, "Đơn hàng mới từ Burger King", content);
            }

            try
            {
                dBContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (Session[strCart] != null)
            {
                Session.Remove(strCart);
            }

            return View();
        }
    }
}