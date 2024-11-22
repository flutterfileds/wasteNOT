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

        private static string _currentUserName;

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

        public static string CurrentUserName
        {   get
            {
                if(_currentUserName=="")
                    throw new InvalidOperationException("User is not logged in");
                return _currentUserName;
            }
            set => _currentUserName = value;
        }
        

        public static bool IsLoggedIn => _currentUserId.HasValue;


        public static void Login(int userId, string userName)
        {
            _currentUserId = userId;
            _currentUserName = userName;
        }

        public static void Logout()
        {
            _currentUserId = null;
            _currentUserName = null;
        }
    }


}
