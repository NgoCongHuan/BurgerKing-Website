using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using BurgerKing.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace BurgerKing.Models
{
    public class LoginView
    {
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc số điện thoại")]
        [Display(Name = "Email hoặc số điện thoại")]
        public string EmailOrPhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}