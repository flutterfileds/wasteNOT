using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First.Domain.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public required Order Order { get; set; }
        
        public required string ProductName { get; set; }
        public required string Quantity { get; set; }
        public required float UnitCost { get; set; }
        public required float Subtotal { get; set; }
    }
}
