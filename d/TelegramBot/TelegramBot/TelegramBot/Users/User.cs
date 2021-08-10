using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot
{
    [Serializable]
    public class User
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        [NonSerialized]
        public List<Message> messages = new List<Message>();
        public string Surname { get; set; }
        public bool inLonigMenu = false;


        public User(long id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }

        public User(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Surname = user.Surname;
        }
    }
}
