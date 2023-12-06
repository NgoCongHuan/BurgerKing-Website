using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BurgerKing.Models
{
    public class OrderStatus
    {
        public const string Canceled = "Đã hủy đơn hàng";
        public const string Failed = "Đã xảy ra lỗi trong quá trình thanh toán";
        public const string Processing = "Đang chờ thanh toán";
        public const string Completed = "Đã thanh toán";
    }
}