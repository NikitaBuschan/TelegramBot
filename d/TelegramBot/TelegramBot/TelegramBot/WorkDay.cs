using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Users
{
    [Serializable]
    public class WorkDay
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public double Money { get; set; }

        public WorkDay(DateTime date, TimeSpan time, double money)
        {
            Date = date;
            Time = time;
            Money = money;
        }
    }
}
