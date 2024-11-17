using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wasteNOT
{
    
    public class UserSession
    {
        public static string UserEmail { get; set; }

        private static int? _currentUserId;

        public static int CurrentUserId
        {
            get
            {
                if (!_currentUserId.HasValue)
                    throw new InvalidOperationException("User is not logged in");
                return _currentUserId.Value;
            }
            set => _currentUserId = value;
        }

        public static bool IsLoggedIn => _currentUserId.HasValue;

        public static void Login(int userId)
        {
            _currentUserId = userId;
        }

        public static void Logout()
        {
            _currentUserId = null;
        }
    }
}
