using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First.Domain.Models
{
    public class Product
    {
        public int id {  get; set; }
        public required string name { get; set; }
        public int price { get; set; }
        public int stock { get; set; }
    }
}
