using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Users
{
    public static class Checks
    {
        public static bool IsAdmin(long id, List<Admin> admins)
        {
            if (admins.Count == 0)
                return false;
            foreach (var item in admins)
                if (item.Id == id)
                    return true;
            return false;
        }

        public static bool IsEmploye(long id, List<Employee> employees)
        {
            if (employees.Count == 0)
                return false;
            foreach (var item in employees)
                if (item.Id == id)
                    return true;
            return false;
        }
    }
}
