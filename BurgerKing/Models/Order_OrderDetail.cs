using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BurgerKing.Models
{
    public class Order_OrderDetail
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}