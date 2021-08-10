using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Users
{
    [Serializable]
    public class Employee : User
    {
        public List<WorkDay> workDays { get; set; } = new List<WorkDay>();
        public DateTime nextWorkDay;
        public int year;
        public int month;
        public int day;
        public int hours;
        public int minutes;
        public TimeSpan time;
        public DateTime StartTime;
        public double totalMoney;
        public bool isTimerOn;
        public bool inChooseWorkDay;
        public bool inChooseTimeWorkDay;
        public bool inChooseRealTimeWorkDay;

        public Employee(long id, string name, string surname) : base(id, name, surname)
        {
            this.Id = id;
            this.Name = name;
            this.Surname = surname;
        }

        public Employee(List<WorkDay> workDays, long id, string name, string surname) : base(id, name, surname)
        {
            this.workDays = workDays;
            this.Id = id;
            this.Name = name;
            this.Surname = surname;
        }
    }
}
