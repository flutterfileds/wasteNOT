using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string user_password { get; set; }
        public required string user_phone { get; set; }
        public required string user_address { get; set; }

        public required IEnumerable<Order> Orders { get; set; }
        public void register(string email, string password) { }

        public void updateProfile() { }
        public void login() { }
        public void logout() { }
    }
}
