using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Users;

namespace TelegramBot
{
    public class Buttons
    {
        // Главное меню 
        public static InlineKeyboardMarkup StartMesage() => new InlineKeyboardMarkup(
             new[]
             {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Переводчик"),
                        InlineKeyboardButton.WithCallbackData("Девушка"),
                    }
             });


        //                                                                               //
        // ----------------------- Набор меню для Администратора ----------------------- //
        //                                                                               //

        // Меню для Администратора
        public static InlineKeyboardMarkup MainMenuForAdmin() => new InlineKeyboardMarkup(
             new[]
             {
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData("Выбрать девушку"),
                     },
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData("Добавить новую девушку"),
                     },
                     new []
                     {
                        InlineKeyboardButton.WithCallbackData("Удалить девушку из списка"),
                     },
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData("Оповестить всех 'Я в сети'")
                     }
             });

        public static InlineKeyboardMarkup MainMenuForMainAdmin() => new InlineKeyboardMarkup(
             new[]
             {
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData("Выбрать девушку"),
                     },
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData("Добавить новую девушку"),
                     },
                     new []
                     {
                        InlineKeyboardButton.WithCallbackData("Удалить девушку из списка"),
                     },
                     new []
                     {
                        InlineKeyboardButton.WithCallbackData("Показать статистику"),
                     },
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData("Оповестить всех 'Я в сети'")
                     }
            });

        // Кнопка возврата к выбору сотрудника
        public static InlineKeyboardMarkup BackToEmployeeChooseMenu() => new InlineKeyboardMarkup(
            new[]
            {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("<< Венуться к выбору девушки"),
                    }
            });

        // Меню ввода пароля
        public static InlineKeyboardMarkup BackMainMenuButton() => new InlineKeyboardMarkup(
             new[]
             {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("<< Венуться в главное меню"),
                    }
             });

        // Мееню выбора всех работников, для главного админа
        public static InlineKeyboardMarkup MenuChooseEmployeesForMainAdmin(List<Employee> employees)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

            foreach (var item in employees)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();
                button.Add($"{item.Name} {item.Surname} | id:{item.Id.ToString().Remove(0, item.Id.ToString().Length - 2)}");
                buttons.Add(button);
            }
            List<InlineKeyboardButton> buttonBackToMain = new List<InlineKeyboardButton>();
            buttonBackToMain.Add("<< Вернуться в меню");
            buttons.Add(buttonBackToMain);

            return new InlineKeyboardMarkup(buttons);
        }



        // Меню выбора работника
        public static InlineKeyboardMarkup MenuChooseEmployees(List<Employee> employees)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

            foreach (var item in employees)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();
                button.Add($"{item.Name} {item.Surname} | id:{item.Id.ToString().Remove(0, item.Id.ToString().Length - 2)}");
                buttons.Add(button);
            }
            List<InlineKeyboardButton> buttonBackToMain = new List<InlineKeyboardButton>();
            buttonBackToMain.Add("<< Вернуться в меню");
            buttons.Add(buttonBackToMain);

            return new InlineKeyboardMarkup(buttons);
        }

        // Меню добавления нового работника
        public static InlineKeyboardMarkup MenuOfAllEmployees(List<Employee> allEmployees, List<Employee> administratorWorkers)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            List<Employee> employeesButtonLis = getEmployessToAdd(allEmployees, administratorWorkers);

            foreach (var item in employeesButtonLis)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();
                button.Add($"{item.Name} {item.Surname} | id:{item.Id.ToString().Remove(0, item.Id.ToString().Length - 2)}");
                buttons.Add(button);
            }
            List<InlineKeyboardButton> buttonBackToMain = new List<InlineKeyboardButton>();
            buttonBackToMain.Add("<< Вернуться в меню");
            buttons.Add(buttonBackToMain);

            return new InlineKeyboardMarkup(buttons);
        }

        // Вспомогательные функции для добавления нового работника
        // ---------------------------------------------
        private static List<Employee> getEmployessToAdd(List<Employee> allEmployees, List<Employee> administratorWorkers)
        {
            if (administratorWorkers.Count == 0)
                return allEmployees;

            List<Employee> employeesButtonList = new List<Employee>();

            foreach (var employee in allEmployees)
            {
                if (inList(employee, administratorWorkers) == true)
                    continue;
                employeesButtonList.Add(employee);
            }

            return employeesButtonList;
        }

        private static bool inList(Employee employee, List<Employee> administratorWorkers)
        {
            foreach (var item in administratorWorkers)
            {
                if (item.Id == employee.Id)
                    return true;
            }
            return false;
        }

        // ---------------------------------------------

        // Включение таймера работнику
        public static InlineKeyboardMarkup OnTimerOfEmployee() => new InlineKeyboardMarkup(
             new[]
             {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Включить таймер")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("<< Выбор девушки")
                    }
             });

        // Выключение таймера работнику
        public static InlineKeyboardMarkup OffTimerOfEmployee() => new InlineKeyboardMarkup(
             new[]
             {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Выключить таймер")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("<< Выбор девушки")
                    }
             });


        //
        // ----------------------- Набор меню для Работника -----------------------
        //

        // Главное меню рабочего
        public static InlineKeyboardMarkup MainMenuOfEmployee() => new InlineKeyboardMarkup(
            new[]
            {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Выбрать рабочий день")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Показать статистику")
                    }
            });

        // Выбор рабочего дня
        public static InlineKeyboardMarkup EnterWorkDay()
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            List<int> days = GetMonth();
            foreach (var day in days)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();
                button.Add(day.ToString());
                buttons.Add(button);
            }
            List<InlineKeyboardButton> back = new List<InlineKeyboardButton>();
            back.Add("<< Вернуться в меню");
            buttons.Add(back);

            return new InlineKeyboardMarkup(buttons);
        }

        // Вспомогательная функция выбора рабочего дня
        public static List<int> GetMonth()
        {
            List<int> list = new List<int>();
            int countDaysOfThisMonth = getDays(DateTime.Now.Month);
            int countDaysOfNextMonth;
            if (getDays(DateTime.Now.Month) == 12)
                countDaysOfNextMonth = 1;
            countDaysOfNextMonth = getDays(DateTime.Now.Month + 1);

            for (int i = DateTime.Now.Day, j = 1; j < 8; i++, j++)
            {
                list.Add(i);
                if (i == countDaysOfThisMonth)
                    i = 0;
            }
            return list;
        }

        public static int getDays(int month)
        {
            int count = 0;
            List<int> Days31 = new List<int>() { 1, 3, 5, 7, 8, 10, 12 };
            List<int> Days30;
            int Days28 = 0;
            int Days29 = 0;

            if (DateTime.Now.Year == 2021)
            {
                Days30 = new List<int>() { 4, 6, 9, 11 };
                Days28 = 2;
            }
            else
            {
                Days30 = new List<int>() { 4, 6, 9, 11 };
                Days29 = 2;
            }

            if (month == Days28)
            {
                count = 28;
            }
            else if (month == Days29)
            {
                count = 29;
            }

            foreach (var day in Days31)
            {
                if (month == day)
                {
                    count = 31;
                    break;
                }
            }

            foreach (var day in Days30)
            {
                if (month == day)
                {
                    count = 30;
                    break;
                }
            }
            return count;
        }

        // ----------------------


        // Выбор рабочего времени
        public static InlineKeyboardMarkup EnterWorkTime(int day)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

            List<string> times = GetTimes(day == DateTime.Now.Day ? true : false);
            foreach (var time in times)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();
                button.Add(time);
                buttons.Add(button);
            }
            List<InlineKeyboardButton> back = new List<InlineKeyboardButton>();
            back.Add("<< Выбрать рабочий день");
            buttons.Add(back);

            return new InlineKeyboardMarkup(buttons);
        }

        public static List<string> GetTimes(bool time)
        {
            List<string> times = new List<string>();
            int from = 0;
            int till = 4;

            for (int i = 0; i < 6; i++)
            {
                if (time == true && DateTime.Now.Hour >= till)
                {
                    from += 4;
                    till += 4;
                    continue;
                }
                times.Add($"{from}:00 - {till}:00");
                from += 4;
                till += 4;
            }

            return times;
        }

        public static List<string> GetAllTimes()
        {
            List<string> times = new List<string>();
            int from = 0;
            int till = 4;

            for (int i = 0; i < 6; i++)
            {
                times.Add($"{from}:00 - {till}:00");
                from += 4;
                till += 4;
            }

            return times;
        }

        // ----------------------


        // Выбор конкретного рабочего времени
        public static InlineKeyboardMarkup EnterRealWorkTime(string t, int day)
        {
            List<string> times = new List<string>();
            int iter = 0;
            List<string> str = new List<string>();
            str = GetAllTimes();

            foreach (var item in str)
            {
                if (item == t)
                {
                    times = GetRealTimes(iter, day == DateTime.Now.Day ? true : false);
                    break;
                }
                iter += 4;
            }

            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            foreach (var time in times)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>();
                button.Add(time);
                buttons.Add(button);
            }
            List<InlineKeyboardButton> back = new List<InlineKeyboardButton>();
            back.Add("<< Выбрать время");
            buttons.Add(back);

            return new InlineKeyboardMarkup(buttons);
        }

        public static List<string> GetRealTimes(int t, bool thisDay)
        {
            List<string> times = new List<string>();
            int from = t;
            int till = from + 4;

            for (int i = from; i < till; i++)
            {
                if (thisDay == true && DateTime.Now.Hour > i)
                    continue;
                for (int j = 0, k = 0; j < 4; j++, k += 15)
                {
                    if (thisDay == true && DateTime.Now.Minute > k && DateTime.Now.Hour == i)
                        continue;
                    if (thisDay == true)
                    {
                        if (k == 0)
                        {
                            times.Add($"{i}:{k}0");
                        }
                        else
                        {
                            times.Add($"{i}:{k}");
                        }
                    }
                    else
                    {
                        if (k == 0)
                        {
                            times.Add($"{i}:{k}0");
                        }
                        else
                        {
                            times.Add($"{i}:{k}");
                        }
                    }
                }
            }
            if (till == 24)
            {
                times.Add($"{till - 1}:59");
            }
            else
            {
                times.Add($"{till}:00");
            }

            return times;
        }

        // ----------------------
        public static InlineKeyboardMarkup BackToMenu() => new InlineKeyboardMarkup(
            new[]
            {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("<< Вернуться в меню")
                    }
             });
    }
}
