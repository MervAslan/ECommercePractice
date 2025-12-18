using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models.ViewModels
{
    public class OrderHeaderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
