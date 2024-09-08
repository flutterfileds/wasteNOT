using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First.Domain.Models
{
    public class ShippingInfo
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string DateShipped { get; set; }
        public required string ShippingCost { get; set; }
        public int ShippingRegionId { get; set; }

        public void updateShippingInfo() { }
    }
}
