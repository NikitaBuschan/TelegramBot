using System;
using System.Collections.Generic;
using TelegramBot.Users;
using Telegram.Bot.Types;

namespace TelegramBot
{
    [Serializable]
    public class Admin : User
    {
        public List<Employee> Employeers { get; set; } = new List<Employee>();
        public Employee EnableTimerToEmployee { get; set; }

        public bool inChooseEmployeeMenu = false;
        public bool inChooseEmployee = false;
        public bool inEnterMoney = false;
        public bool inDeleteEmployee = false;
        public bool showStatistic = false;

        public Admin(long id, string name, string surname) : base(id, name, surname)
        {
            this.Id = id;
            this.Name = name;
            this.Surname = surname;
        }

        public Admin(User user) : base(user)
        {
            Id = user.Id;
            Name = user.Name;
            Surname = user.Surname;
        }
    }
}

