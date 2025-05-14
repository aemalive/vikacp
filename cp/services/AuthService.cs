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
        public static readonly User GuestUser = new User
        {
            Id = 0,
            Username = "guest",
            Password = string.Empty,
            FullName = "Guest User",
            Role = "GUEST",
            Email = string.Empty
        };

        public static User CurrentUser { get; set; } = GuestUser;

        public static bool IsAuthenticated => CurrentUser != null && CurrentUser != GuestUser;

        public static void Logout()
        {
            CurrentUser = GuestUser;
        }
    }

}
