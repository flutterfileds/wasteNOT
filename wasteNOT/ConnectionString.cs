using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wasteNOT
{
    internal class ConnectionString
    {
        public static string GetConnectionString() { return "Host=localhost;Port=5432;Username=postgres;Password=stanloona;Database=postgres"; }
    }
}
