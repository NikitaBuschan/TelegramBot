using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Telegram.Bot.Types;
using TelegramBot.Users;

namespace TelegramBot
{
    public static class DataControl
    {
        private static BinaryFormatter formatter = new BinaryFormatter();
        private static string adminsPath = "admin.dat";
        private static string usersPath = "user.dat";
        private static string allEmployeesPath = "employee.dat";

        public static void SetAdmins(List<Admin> admins)
        {
            using (var fileStream = new FileStream(adminsPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                formatter.Serialize(fileStream, admins);
        }

        public static void SetUsers(List<User> users)
        {
            using (var fileStream = new FileStream(usersPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                formatter.Serialize(fileStream, users);
        }

        public static void SetEmployees(List<Employee> employees)
        {
            using (var fileStream = new FileStream(allEmployeesPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                formatter.Serialize(fileStream, employees);
        }

        public static List<Admin> GetAdmins()
        {
            try
            {
                using (var fileStream = new FileStream(adminsPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    return (List<Admin>)formatter.Deserialize(fileStream);
            }
            catch (System.Exception)
            {
            }
            return new List<Admin>();
        }

        public static List<User> GetUsers()
        {
            try
            {
                using (var fileStream = new FileStream(usersPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    return (List<User>)formatter.Deserialize(fileStream);
            }
            catch (System.Exception)
            {
            }
            return new List<User>();
        }

        static public List<Employee> GetEmployees()
        {
            try
            {
                using (var fileStream = new FileStream(allEmployeesPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                return (List<Employee>)formatter.Deserialize(fileStream);
            }
            catch (System.Exception)
            {
            }
            return new List<Employee>();
        }
    }
}
