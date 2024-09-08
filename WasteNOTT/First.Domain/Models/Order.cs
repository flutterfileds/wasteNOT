using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
 
        public required string DateCreated { get; set; }
        public required string DateShipped { get; set; }
        public required string CustomerName { get; set; }
        public required string Status { get; set; }
        public required bool isPurchased { get; set; }
        public int ShippingId { get; set; }
        public required User User { get; set; }

        
        public void placeOrder() { }
        public void confirmOrder(Order order) { }
    }
}
