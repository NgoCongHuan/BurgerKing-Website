using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text;
using BurgerKing.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace BurgerKing.Controllers
{
    public class AccountController : Controller
    {

        // Đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            using (BurgerKingDBContext dbContext = new BurgerKingDBContext())
            {
                string emailOrPhoneNumber = form["EmailOrPhone"];
                string password = form["Password"];

                if (ModelState.IsValid)
                {
                    if (IsEmail(emailOrPhoneNumber))
                    {
                        password = GetSHA256(password);
                        bool isValidAccount = dbContext.Accounts.Any(acc => acc.Email == emailOrPhoneNumber && acc.Password.ToLower() == password.ToLower());
                        if (isValidAccount)
                        {
                            FormsAuthentication.SetAuthCookie(emailOrPhoneNumber, false);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else if (IsPhoneNumber(emailOrPhoneNumber))
                    {
                        password = GetSHA256(password);
                        bool isValidAccount = dbContext.Accounts.Any(acc => acc.Phone == emailOrPhoneNumber && acc.Password.ToLower() == password.ToLower());
                        if (isValidAccount)
                        {
                            FormsAuthentication.SetAuthCookie(emailOrPhoneNumber, false);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
                    return View();
                }
            }

            return View();
        }


        // Đăng kí
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            using (BurgerKingDBContext dbContext = new BurgerKingDBContext())
            {
                string name = form["Name"];
                string email = form["Email"];
                string phone = form["Phone"];
                string password = form["Password"];
                string confirmPassword = form["ConfirmPassword"]; // Thêm dòng này để lấy mật khẩu nhập lại

                // Kiểm tra xem email đã tồn tại hay chưa
                bool isEmailExist = dbContext.Accounts.Any(acc => acc.Email == email);

                // Kiểm tra xem số điện thoại đã tồn tại hay chưa
                bool isPhoneExist = dbContext.Accounts.Any(acc => acc.Phone == phone);

                // Nếu email hoặc số điện thoại đã tồn tại, trả về thông báo lỗi
                if (isEmailExist || isPhoneExist)
                {
                    ModelState.AddModelError("", "Email hoặc số điện thoại đã tồn tại. Vui lòng nhập lại");
                    return View();
                }

                // Kiểm tra mật khẩu có độ dài trên 6 kí tự và chứa cả chữ và số
                if (password.Length < 6 || !password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                {
                    ModelState.AddModelError("", "Mật khẩu phải có ít nhất 6 kí tự và bao gồm cả chữ và số.");
                    return View();
                }

                // Kiểm tra xem mật khẩu và mật khẩu nhập lại có khớp nhau hay không
                if (password != confirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu và mật khẩu nhập lại không khớp.");
                    return View();
                }

                // Thêm tài khoản vào Cơ sở dữ liệu
                var newAccount = new Account
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Password = GetSHA256(password),
                    // Thiết lập quyền mặc định là khách hàng
                    RoleId = dbContext.Roles.FirstOrDefault(r => r.RoleName == "Customer").RoleId
                };

                dbContext.Accounts.Add(newAccount);
                dbContext.SaveChanges();

                return RedirectToAction("Login", "Account");
            }
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // Kiểm tra số điện thoại hợp lệ hay không
        private bool IsPhoneNumber(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Length >= 10 && input.All(char.IsDigit);
        }

        // Kiểm tra email hợp lệ hay không
        private bool IsEmail(string input)
        {
            return new EmailAddressAttribute().IsValid(input);
        }

        // Hàm băm SHA-256 để mã hóa Passwword
        public static string GetSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }


    }
}