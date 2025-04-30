using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cp.models;

namespace cp.services
{
    public static class AuthService
    {
        public static User CurrentUser { get; set; }
        public static bool IsAuthenticated => CurrentUser != null;

        public static void Logout()
        {
            CurrentUser = new User
            {
                Id = 0,
                Username = "guest",
                Password = string.Empty, 
                FullName = "Guest User",
                Role = "GUEST",
                Email = string.Empty 
            };
        }
    }

}
