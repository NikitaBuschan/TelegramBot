using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Users;

namespace TelegramBot
{
    public partial class Program
    {
        private static ITelegramBotClient bot;

        private static List<Admin> admins = new List<Admin>();
        private static List<User> users = new List<User>();
        private static List<Employee> allEmployees = new List<Employee>();
        private static long? mainAdm = 190313221;
        private static long? mainAdmNik = 1137539286;

        public static void Main(string[] args)
        {
            admins = DataControl.GetAdmins();
            users = DataControl.GetUsers();
            allEmployees = DataControl.GetEmployees();

            foreach (var admin in admins)
                admin.messages = new List<Message>();
            foreach (var employee in allEmployees)
                employee.messages = new List<Message>();
            foreach (var user in users)
                user.messages = new List<Message>();

            foreach (var item in admins)
                Console.WriteLine($"{item.Id}\n{item.Name}{item.Surname}");

            bot = new TelegramBotClient("1134450894:AAHLeyl_CivOms0ttDWQDReJnZaJiUBus-w") { Timeout = TimeSpan.FromSeconds(10) };

            var me = bot.GetMeAsync().Result;
            Console.WriteLine($"Bot id: {me.Id}. Bot name: {me.FirstName}");

            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            bot.OnMessage += Bot_OnMessage;

            bot.StartReceiving();

            Console.ReadKey();
            bot.StopReceiving();
        }

        public static async void RememberEmployeeToChooseWorkDay(Employee employee)
        {
            int timer = 110;

            Thread.Sleep(timer * 1000 * 60);
            if (employee.nextWorkDay != null)
            {
                RememberEmployeeToChooseWorkDay(employee);
            }
            else if (DateTime.Now.Hour > 2 && DateTime.Now.Hour < 4)
            {
                employee.messages.Add(await bot.SendTextMessageAsync(employee.Id, $"Добрый день. Выберите рабочий день и время."));
                RememberEmployeeToChooseWorkDay(employee);
            }

        }

        public static async void RingToEmployee(Employee employee)
        {
            DateTime now = DateTime.Now;
            TimeSpan time = employee.nextWorkDay - now;
            int timer = 0;
            if (time.TotalMinutes > 100)
            {
                Thread.Sleep(30 * 1000 * 60);
                RingToEmployee(employee);
            }
            else if (time.TotalMinutes > 62)
            {
                timer = Convert.ToInt32(time.TotalMinutes) - 60;
                Thread.Sleep(timer * 1000 * 60);
                employee.messages.Add(await bot.SendTextMessageAsync(employee.Id, $"Начало работы через: 1 час"));
                RingToEmployee(employee);
            }
            else if (time.TotalMinutes > 30)
            {
                timer = Convert.ToInt32(time.TotalMinutes) - 30;
                Thread.Sleep(timer * 1000 * 60);
                employee.messages.Add(await bot.SendTextMessageAsync(employee.Id, $"Начало работы через: 30 минут"));
                RingToEmployee(employee);
            }
            else if (time.TotalMinutes > 10)
            {
                timer = Convert.ToInt32(time.TotalMinutes) - 10;
                Thread.Sleep(timer * 1000 * 60);
                employee.messages.Add(await bot.SendTextMessageAsync(employee.Id, $"Начало работы через: 10 минут"));
                RingToEmployee(employee);
            }
            else if (time.TotalMinutes < 10)
            {
                return;
            }
        }

        //
        // ---------- Обработка нажатий клавиш меню ----------
        //
        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            try
            {
                foreach (var item in admins)
                {
                    if (item.Id == e.CallbackQuery.From.Id)
                    {
                        Console.WriteLine($"ID: {item.Id}  Name: {item.Name}");
                        Console.WriteLine($"InLoginMenu: {item.inLonigMenu} InEnterMoney: {item.inEnterMoney} inDeleteEmployee: {item.inDeleteEmployee}\ninChooseEmployeeMenu: {item.inChooseEmployeeMenu} inChooseEmployee: {item.inChooseEmployee}");
                    }
                }

                string message = e.CallbackQuery.Data;

                // Очистка сообщений
                // --------------------------------

                foreach (var admin in admins)
                {
                    if (admin.Id == e.CallbackQuery.From.Id)
                    {
                        if (admin.messages == null)
                            break;
                        for (int i = 0; i < admin.messages.Count; i++)
                        {
                            await bot.DeleteMessageAsync(admin.messages[i].Chat.Id, admin.messages[i].MessageId);
                        }
                        admin.messages.Clear();
                        DataControl.SetAdmins(admins);
                        break;
                    }
                }
                foreach (var employee in allEmployees)
                {
                    if (employee.Id == e.CallbackQuery.From.Id)
                    {
                        if (employee.messages == null)
                            break;
                        for (int i = 0; i < employee.messages.Count; i++)
                        {
                            await bot.DeleteMessageAsync(employee.messages[i].Chat.Id, employee.messages[i].MessageId);
                        }
                        employee.messages.Clear();
                        DataControl.SetEmployees(allEmployees);
                        break;
                    }
                }
                foreach (var user in users)
                {
                    if (user.Id == e.CallbackQuery.From.Id)
                    {
                        if (user.messages == null)
                            break;
                        for (int i = 0; i < user.messages.Count; i++)
                        {
                            await bot.DeleteMessageAsync(user.messages[i].Chat.Id, user.messages[i].MessageId);
                        }
                        user.messages.Clear();
                        DataControl.SetUsers(users);
                        break;
                    }
                }


                // --------------------------------

                // Добавление рабоника
                // Выбор работника для управления им
                foreach (var admin in admins)
                {
                    if (admin.Id == e.CallbackQuery.From.Id)  // поиск администратора который нажал кнопку
                    {
                        if (admin.showStatistic == true)
                        {
                            foreach (var employee in allEmployees)
                            {
                                if ($"{employee.Name} {employee.Surname} | id:{employee.Id.ToString().Remove(0, employee.Id.ToString().Length - 2)}" == message)
                                {
                                    if (employee.workDays.Count == 0)
                                    {
                                        Message mes = new Message();
                                        mes = await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"У девушки еще не было рабочих дней");
                                        Thread.Sleep(2000);
                                        await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Выберите девушку",
                                                                 replyMarkup: Buttons.MenuChooseEmployeesForMainAdmin(allEmployees)));
                                        return;
                                    }
                                    int iter = 0;
                                    foreach (var day in employee.workDays)
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Дата: {employee.workDays[iter].Date.Day}/{employee.workDays[iter].Date.Month}/{employee.workDays[iter].Date.Year}\n" +
                                                             $"Время работы: {employee.workDays[iter].Time.Minutes}\n" +
                                                             $"Заработанно: {employee.workDays[iter].Money}"));
                                        iter++;
                                    }
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выбор девушки",
                                                         replyMarkup: Buttons.BackToEmployeeChooseMenu()));
                                    return;
                                }
                            }
                        }

                        foreach (var employee in admin.Employeers)  // поиск работника у администратора
                        {
                            if ($"{employee.Name} {employee.Surname} | id:{employee.Id.ToString().Remove(0, employee.Id.ToString().Length - 2)}" == message)
                            {
                                //Выбор работника для управления им
                                if (admin.inDeleteEmployee == true)
                                {
                                    admin.inDeleteEmployee = false;
                                    Employee temp = employee;
                                    admin.Employeers.Remove(employee);
                                    DataControl.SetAdmins(admins);

                                    Message mes = new Message();
                                    mes = await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Девушка {temp.Name} {temp.Surname} была удалена");
                                    Thread.Sleep(2000);
                                    await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                    if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForMainAdmin()));
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForAdmin()));
                                    }
                                    return;
                                }
                                else if (admin.inChooseEmployeeMenu == true)
                                {
                                    admin.inChooseEmployeeMenu = false;
                                    admin.inChooseEmployee = true;
                                    if (employee.nextWorkDay != null)
                                    {
                                        if (employee.nextWorkDay.Minute == 0)
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Выбран рабочий день:\n" +
                                                         $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                         $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                        }
                                        else
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Выбран рабочий день:\n" +
                                                         $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                         $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                        }
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Девушка не выбрала рабочий день."));
                                    }
                                    if (employee.isTimerOn == true)
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Девушка: {employee.Name} {employee.Surname}",
                                                         replyMarkup: Buttons.OffTimerOfEmployee()));
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Девушка: {employee.Name} {employee.Surname}",
                                                             replyMarkup: Buttons.OnTimerOfEmployee()));
                                    }
                                    admin.EnableTimerToEmployee = employee;
                                    return;
                                }
                            }
                        }
                        foreach (var employee in allEmployees)
                        {
                            if ($"{employee.Name} {employee.Surname} | id:{employee.Id.ToString().Remove(0, employee.Id.ToString().Length - 2)}" == message)
                            {
                                // Добавление работника
                                admin.Employeers.Add(employee);
                                DataControl.SetAdmins(admins);
                                Message mes = new Message();
                                mes = await bot.SendTextMessageAsync(
                                                     e.CallbackQuery.From.Id,
                                                     $"Девушка {employee.Name} {employee.Surname} | id:{employee.Id.ToString().Remove(0, employee.Id.ToString().Length - 2)} добавлена");
                                Thread.Sleep(2000);
                                await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                {
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Меню",
                                                         replyMarkup: Buttons.MainMenuForMainAdmin()));
                                }
                                else
                                {
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Меню",
                                                         replyMarkup: Buttons.MainMenuForAdmin()));
                                }
                                return;
                            }
                        }
                    }
                }

                switch (message)
                {
                    //
                    // Обработка нажатий клавишь Переводчиком
                    //

                    case "Переводчик":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForMainAdmin()));
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForAdmin()));
                                    }
                                    return;
                                }
                            }
                            foreach (var user in users)
                            {
                                if (user.Id == e.CallbackQuery.From.Id)
                                {
                                    user.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Введите пароль для регистрации: ",
                                                         replyMarkup: Buttons.BackMainMenuButton()));
                                    return;
                                }
                            }
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Введите пароль для регистрации: ",
                                                         replyMarkup: Buttons.BackMainMenuButton()));
                                    return;
                                }
                            }

                            break;
                        }
                    case "<< Венуться в главное меню":
                        {
                            foreach (var user in users)
                            {
                                if (user.Id == e.CallbackQuery.From.Id)
                                {
                                    user.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите пункт меню",
                                                         replyMarkup: Buttons.StartMesage()));
                                    return;
                                }
                            }
                            break;
                        }
                    case "<< Вернуться в меню":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    admin.inDeleteEmployee = false;
                                    admin.inChooseEmployee = false;
                                    admin.inChooseEmployeeMenu = false;
                                    if (admin.Id == mainAdm || admin.Id == mainAdmNik)
                                    {
                                        admin.showStatistic = false;
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForMainAdmin()));
                                        return;
                                    }
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                     e.CallbackQuery.From.Id,
                                                     "Меню",
                                                     replyMarkup: Buttons.MainMenuForAdmin()));
                                    return;
                                }
                            }
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    if (employee.nextWorkDay != null)
                                    {
                                        if (employee.nextWorkDay.Minute == 0)
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Выбран рабочий день:\n" +
                                                         $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                         $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                        }
                                        else
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Выбран рабочий день:\n" +
                                                         $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                         $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                        }
                                    }
                                    else
                                    {
                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Вы не выбрали рабочий день."));
                                    }
                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                     e.CallbackQuery.From.Id,
                                                     "Меню",
                                                     replyMarkup: Buttons.MainMenuOfEmployee()));
                                    return;
                                }
                            }
                            break;
                        }
                    case "<< Выбор девушки":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    admin.inChooseEmployeeMenu = true;
                                    admin.inChooseEmployee = false;
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите девушку",
                                                         replyMarkup: Buttons.MenuChooseEmployees(admin.Employeers)));
                                    admin.EnableTimerToEmployee = null;
                                }
                            }
                            break;
                        }
                    case "Включить таймер":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    foreach (var employee in admin.Employeers)
                                    {
                                        if (employee.Id == admin.EnableTimerToEmployee.Id)
                                        {
                                            employee.isTimerOn = true;
                                            employee.StartTime = DateTime.Now;
                                            DataControl.SetAdmins(admins);
                                            if (employee.nextWorkDay != null)
                                            {
                                                if (employee.nextWorkDay.Minute == 0)
                                                {
                                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 $"Выбран рабочий день:\n" +
                                                                 $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                                 $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                                }
                                                else
                                                {
                                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 $"Выбран рабочий день:\n" +
                                                                 $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                                 $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                                }
                                            }
                                            else
                                            {
                                                admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 $"Девушка не выбрала рабочий день."));
                                            }
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите пункт меню",
                                                         replyMarkup: Buttons.OffTimerOfEmployee()));
                                            return;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Выключить таймер":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    foreach (var employee in admin.Employeers)
                                    {
                                        if (employee.Id == admin.EnableTimerToEmployee.Id)
                                        {
                                            employee.isTimerOn = false;
                                            admin.inEnterMoney = true;

                                            employee.time = DateTime.Now - employee.StartTime;
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Время работы: {employee.time.Minutes} минут.\n Укажите заработанную сумму в $"));
                                            return;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Выбрать девушку":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    if (admin.Employeers.Count == 0)
                                    {
                                        Message mes = new Message();
                                        mes = await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "В вашем списке нет девушек. Добавьте девушку.");
                                        Thread.Sleep(2000);
                                        await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                        if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Меню",
                                                                 replyMarkup: Buttons.MainMenuForMainAdmin()));
                                        }
                                        else
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Меню",
                                                                 replyMarkup: Buttons.MainMenuForAdmin()));
                                        }
                                    }
                                    else
                                    {
                                        admin.inChooseEmployeeMenu = true;
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Выберите девушку",
                                                             replyMarkup: Buttons.MenuChooseEmployees(admin.Employeers)));
                                    }
                                    return;
                                }
                            }
                            break;
                        }
                    case "Добавить новую девушку":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    if (allEmployees.Count == 0)
                                    {
                                        Message mes = new Message();
                                        mes = await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "В списке нет девушек");
                                        Thread.Sleep(2000);
                                        await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                        if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Меню",
                                                                 replyMarkup: Buttons.MainMenuForMainAdmin()));
                                        }
                                        else
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Меню",
                                                                 replyMarkup: Buttons.MainMenuForAdmin()));
                                        }
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Добавить девушку",
                                                             replyMarkup: Buttons.MenuOfAllEmployees(allEmployees, admin.Employeers)));
                                    }
                                    return;
                                }
                            }
                            break;
                        }
                    case "Удалить девушку из списка":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    if (admin.Employeers.Count == 0)
                                    {
                                        Message mes = new Message();
                                        mes = await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "В вашем списке нет девушек. Добавьте девушку.");
                                        Thread.Sleep(2000);
                                        await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                        if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Меню",
                                                                 replyMarkup: Buttons.MainMenuForMainAdmin()));
                                        }
                                        else
                                        {
                                            admin.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 "Меню",
                                                                 replyMarkup: Buttons.MainMenuForAdmin()));
                                        }
                                    }
                                    else
                                    {
                                        admin.inDeleteEmployee = true;
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Выберите девушку",
                                                             replyMarkup: Buttons.MenuChooseEmployees(admin.Employeers)));
                                    }
                                }
                            }
                            break;
                        }
                    case "Оповестить всех 'Я в сети'":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    foreach (var employee in admin.Employeers)
                                    {
                                        foreach (var item in allEmployees)
                                        {
                                            if (employee.Id == item.Id)
                                            {
                                                employee.messages.Add(await bot.SendTextMessageAsync(
                                                                              employee.Id,
                                                                              $"{admin.Name} {admin.Surname} сейчас онлайн!"));
                                                break;
                                            }
                                        }
                                    }
                                    Message mes = new Message();
                                    if (admin.Employeers.Count == 0)
                                    {
                                        mes = await bot.SendTextMessageAsync(
                                                      e.CallbackQuery.From.Id,
                                                      "В вашем списке нет девушек");
                                    }
                                    else
                                    {
                                        mes = await bot.SendTextMessageAsync(
                                                      e.CallbackQuery.From.Id,
                                                      "Все девушки были оповещены");
                                    }
                                    Thread.Sleep(2000);
                                    await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                    if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForMainAdmin()));
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForAdmin()));
                                    }
                                    return;
                                }
                            }
                            break;
                        }
                    case "<< Венуться к выбору девушки":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    if (e.CallbackQuery.From.Id == mainAdm || e.CallbackQuery.From.Id == mainAdmNik)
                                    {
                                        admin.showStatistic = true;
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MenuChooseEmployeesForMainAdmin(allEmployees)));
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Выберите девушку",
                                                             replyMarkup: Buttons.MenuChooseEmployees(admin.Employeers)));
                                    }
                                }
                            }
                            break;
                        }

                    //
                    // Обработка нажатий клавишь Работником
                    //

                    case "Девушка":
                        {
                            if (!Checks.IsEmploye(e.CallbackQuery.From.Id, allEmployees) &&
                                !Checks.IsAdmin(e.CallbackQuery.From.Id, admins))
                            {
                                Employee temp = new Employee(e.CallbackQuery.From.Id, e.CallbackQuery.From.FirstName, e.CallbackQuery.From.LastName);
                                allEmployees.Add(temp);
                                DataControl.SetEmployees(allEmployees);
                                foreach (var user in users)
                                {
                                    if (user.Id == temp.Id)
                                    {
                                        users.Remove(user);
                                        DataControl.SetUsers(users);
                                        // console
                                        Console.WriteLine($"Добавлена новая девушка: {temp.Id}\n{temp.Name}\n{temp.Surname}");
                                        break;
                                    }
                                }
                            }
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id)
                                {
                                    Message mes = new Message();
                                    mes = await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Вы являетесь администратором");

                                    Thread.Sleep(2000);
                                    await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите пункт меню",
                                                         replyMarkup: Buttons.StartMesage()));
                                    return;
                                }
                            }
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    if (employee.nextWorkDay != null)
                                    {
                                        if (employee.nextWorkDay.Minute == 0)
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Выбран рабочий день:\n" +
                                                         $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                         $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                        }
                                        else
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Выбран рабочий день:\n" +
                                                         $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                         $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                        }
                                    }
                                    else
                                    {
                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         $"Вы не выбрали рабочий день."));
                                    }
                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Меню",
                                                         replyMarkup: Buttons.MainMenuOfEmployee()));
                                    RememberEmployeeToChooseWorkDay(employee);
                                }
                            }
                            break;
                        }
                    case "Выбрать рабочий день":
                        {
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите рабочий день",
                                                         replyMarkup: Buttons.EnterWorkDay()));
                                    employee.inChooseWorkDay = true;
                                    return;
                                }
                            }
                            break;
                        }
                    case "<< Выбрать рабочий день":
                        {
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    employee.inChooseTimeWorkDay = false;
                                    employee.inChooseWorkDay = true;
                                    employee.day = 0;
                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите рабочий день",
                                                         replyMarkup: Buttons.EnterWorkDay()));
                                    return;
                                }
                            }
                            break;
                        }
                    case "<< Выбрать время":
                        {
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    employee.inChooseRealTimeWorkDay = false;
                                    employee.inChooseTimeWorkDay = true;

                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите время",
                                                         replyMarkup: Buttons.EnterWorkTime(employee.day)));
                                    return;
                                }
                            }
                            break;
                        }
                    case "Показать статистику":
                        {
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.CallbackQuery.From.Id && e.CallbackQuery.From.Id == mainAdm || admin.Id == e.CallbackQuery.From.Id && e.CallbackQuery.From.Id == mainAdmNik)
                                {
                                    admin.showStatistic = true;
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.CallbackQuery.From.Id,
                                                         "Выберите девушку",
                                                         replyMarkup: Buttons.MenuChooseEmployeesForMainAdmin(allEmployees)));
                                    return;
                                }
                            }

                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    if (employee.workDays.Count == 0)
                                    {
                                        Message mes = new Message();
                                        mes = await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "У вас не было рабочих дней");
                                        Thread.Sleep(2000);
                                        await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                        if (employee.nextWorkDay != null)
                                        {
                                            if (employee.nextWorkDay.Minute == 0)
                                            {
                                                employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Выбран рабочий день:\n" +
                                                             $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                             $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                            }
                                            else
                                            {
                                                employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Выбран рабочий день:\n" +
                                                             $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                             $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                            }
                                        }
                                        else
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Вы не выбрали рабочий день."));
                                        }
                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuOfEmployee()));
                                        return;
                                    }
                                    else
                                    {
                                        int iter = 0;
                                        foreach (var day in employee.workDays)
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                                 e.CallbackQuery.From.Id,
                                                                 $"Дата: {employee.workDays[iter].Date.Day}/{employee.workDays[iter].Date.Month}/{employee.workDays[iter].Date.Year}\n" +
                                                                 $"Время работы: {employee.workDays[iter].Time.Minutes} минут\n" +
                                                                 $"Заработанно: {employee.workDays[iter].Money} $"));
                                            iter++;
                                        }
                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Выход в меню",
                                                             replyMarkup: Buttons.BackToMenu()));
                                        return;
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        {
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.CallbackQuery.From.Id)
                                {
                                    // выбор рабочего дня
                                    if (employee.inChooseWorkDay == true)
                                    {
                                        employee.inChooseWorkDay = false;
                                        employee.inChooseTimeWorkDay = true;

                                        if (DateTime.Now.Month == 12 && DateTime.Now.Day < Convert.ToInt32(message))
                                        {
                                            employee.year = DateTime.Now.Year + 1;
                                            employee.month = 01;
                                            employee.day = Convert.ToInt32(message);
                                        }
                                        else if (DateTime.Now.Month < 12 && DateTime.Now.Day < Convert.ToInt32(message))
                                        {
                                            employee.year = DateTime.Now.Year;
                                            employee.month = DateTime.Now.Month + 1;
                                            employee.day = Convert.ToInt32(message);
                                        }
                                        else
                                        {
                                            employee.year = DateTime.Now.Year;
                                            employee.month = DateTime.Now.Month;
                                            employee.day = Convert.ToInt32(message);
                                        }

                                        Console.WriteLine($"Work day: {message}");


                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Выберите время для {employee.day}/{employee.month}/{employee.year}",
                                                             replyMarkup: Buttons.EnterWorkTime(employee.day)));
                                        return;
                                    }
                                    // Выбор рабочего времени
                                    else if (employee.inChooseTimeWorkDay == true)
                                    {
                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Выберите время",
                                                             replyMarkup: Buttons.EnterRealWorkTime(message, employee.day)));
                                        employee.inChooseTimeWorkDay = false;
                                        employee.inChooseRealTimeWorkDay = true;
                                        return;
                                    }
                                    // Выбор конкретного рабочего времени
                                    else if (employee.inChooseRealTimeWorkDay == true)
                                    {
                                        if (message.Length == 4)
                                        {
                                            message = $"0{message}";
                                        }

                                        employee.inChooseRealTimeWorkDay = false;
                                        employee.nextWorkDay = new DateTime(
                                            employee.year,
                                            employee.month,
                                            employee.day,
                                            Convert.ToInt32(message.Remove(2, 3)),
                                            Convert.ToInt32(message.Remove(0, 3)),
                                            0);

                                        // console
                                        Console.WriteLine(employee.nextWorkDay);

                                        // вывод на экран рабочее время и возврат в главное меню
                                        Message mes = new Message();
                                        mes = await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Вы выбрали время: {employee.nextWorkDay}");

                                        foreach (var admin in admins)
                                        {
                                            foreach (var item in admin.Employeers)
                                            {
                                                if (item.Id == e.CallbackQuery.From.Id)
                                                {
                                                    if (employee.nextWorkDay.Minute == 0)
                                                    {
                                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                                                     admin.Id,
                                                                                     $"{item.Name} {item.Surname} выбрала рабочее время.\n" +
                                                                                     $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                                                     $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                                    }
                                                    else
                                                    {
                                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                                                   admin.Id,
                                                                                   $"{item.Name} {item.Surname} выбрала рабочее время.\n" +
                                                                                   $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                                                   $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                        Thread.Sleep(2000);
                                        await bot.DeleteMessageAsync(e.CallbackQuery.From.Id, mes.MessageId);

                                        if (employee.nextWorkDay != null)
                                        {
                                            if (employee.nextWorkDay.Minute == 0)
                                            {
                                                employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Выбран рабочий день:\n" +
                                                             $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                             $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}0"));
                                            }
                                            else
                                            {
                                                employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Выбран рабочий день:\n" +
                                                             $"Дата: {employee.nextWorkDay.Day}/{employee.nextWorkDay.Month}/{employee.nextWorkDay.Year}\n" +
                                                             $"Время: {employee.nextWorkDay.Hour}:{employee.nextWorkDay.Minute}"));
                                            }
                                        }
                                        else
                                        {
                                            employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             $"Вы не выбрали рабочий день."));
                                        }
                                        employee.messages.Add(await bot.SendTextMessageAsync(
                                                             e.CallbackQuery.From.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuOfEmployee()));
                                        RingToEmployee(employee);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}


// после показа статистики, вернуться назад  и зависает меню
// 
