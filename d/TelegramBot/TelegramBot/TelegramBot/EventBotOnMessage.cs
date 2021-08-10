using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Telegram.Bot.Types;
using TelegramBot.Users;

namespace TelegramBot
{
    public partial class Program
    {
        //
        // ---------- Обработка сообщений ----------
        //
        public static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                foreach (var item in admins)
                {
                    if (item.Id == e.Message.Chat.Id)
                    {
                        Console.WriteLine($"ID: {item.Id}  Name: {item.Name}");
                        Console.WriteLine($"InLoginMenu: {item.inLonigMenu} InEnterMoney: {item.inEnterMoney} inDeleteEmployee: {item.inDeleteEmployee}\ninChooseEmployeeMenu: {item.inChooseEmployeeMenu} inChooseEmployee: {item.inChooseEmployee}");
                    }
                }

                string message = e.Message.Text;

                await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);

                // Очистка сообщений
                // --------------------------------

                foreach (var admin in admins)
                {
                    if (admin.Id == e.Message.Chat.Id)
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
                    if (employee.Id == e.Message.Chat.Id)
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
                    if (user.Id == e.Message.Chat.Id)
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

                // Ввод заработанных денег и сохраннеие файлов
                foreach (var admin in admins)
                {
                    if (admin.Id == e.Message.Chat.Id && admin.inEnterMoney == true)
                    {
                        admin.inEnterMoney = false;
                        foreach (var employee in allEmployees)
                        {
                            if (employee.Id == admin.EnableTimerToEmployee.Id)
                            {
                                WorkDay temp = new WorkDay(DateTime.Now, DateTime.Now - admin.EnableTimerToEmployee.StartTime, Convert.ToDouble(e.Message.Text));
                                employee.workDays.Add(temp);
                                TimeSpan tempTime = DateTime.Now - admin.EnableTimerToEmployee.StartTime;
                                // сообщение работнику о рабочем времени
                                employee.messages.Add(await bot.SendTextMessageAsync(employee.Id, 
                                                              $"Вермя работы: {Convert.ToInt32(tempTime.TotalMinutes)} минут\n" +
                                                              $"Заработок: {Convert.ToDouble(e.Message.Text)} $"));

                                if (employee.workDays.Count > 30)
                                {
                                    employee.workDays.RemoveAt(0);
                                }
                                admin.EnableTimerToEmployee = null;
                                DataControl.SetAdmins(admins);
                                DataControl.SetEmployees(allEmployees);

                                // console
                                Console.WriteLine($"Date: {temp.Date}\nTime: {temp.Time.Minutes}\nSum: {temp.Money}");
                                if (e.Message.Chat.Id == mainAdm || e.Message.Chat.Id == mainAdmNik)
                                {
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.Message.Chat.Id,
                                                         "Меню",
                                                         replyMarkup: Buttons.MainMenuForMainAdmin()));
                                }
                                else
                                {
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                         e.Message.Chat.Id,
                                                         "Меню",
                                                         replyMarkup: Buttons.MainMenuForAdmin()));
                                }
                                return;
                            }
                        }
                    }
                }

                switch (e.Message.Text)
                {
                    case "/start":
                        {
                            if (!Checks.IsEmploye(e.Message.Chat.Id, allEmployees) &&
                                !Checks.IsAdmin(e.Message.Chat.Id, admins))
                            {
                                users.Add(new User(e.Message.Chat.Id, e.Message.Chat.FirstName, e.Message.Chat.LastName));
                                DataControl.SetUsers(users);
                                // console
                                Console.WriteLine($"Add new user:\nID: {e.Message.Chat.Id}\nName: {e.Message.Chat.FirstName}\nLast name: {e.Message.Chat.LastName}\n\n");
                                foreach (var user in users)
                                {
                                    if (user.Id == e.Message.Chat.Id)
                                    {
                                        user.messages.Add(await bot.SendTextMessageAsync(
                                                             e.Message.Chat.Id,
                                                             "Выберите пункт меню",
                                                             replyMarkup: Buttons.StartMesage()));
                                        return;
                                    }
                                }                                
                            }
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.Message.Chat.Id)
                                {
                                    employee.messages.Add(await bot.SendTextMessageAsync(
                                                         e.Message.Chat.Id,
                                                         "Выберите пункт меню",
                                                         replyMarkup: Buttons.StartMesage()));
                                    return;
                                }
                            }
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.Message.Chat.Id)
                                {
                                    admin.messages.Add(await bot.SendTextMessageAsync(
                                                               e.Message.Chat.Id,
                                                               "Выберите пункт меню",
                                                               replyMarkup: Buttons.StartMesage()));
                                    return;
                                }
                            }
                            break;
                        }
                    case "l36hoi9f":
                        {
                            foreach (var item in admins)
                            {
                                if (item.Id == e.Message.Chat.Id)
                                {
                                    item.inLonigMenu = false;
                                    break;
                                }
                            }
                            foreach (var user in users)
                            {
                                if (user.Id == e.Message.Chat.Id)
                                {
                                    admins.Add(new Admin(user));
                                    DataControl.SetAdmins(admins);
                                    // console
                                    Console.WriteLine($"Add new admin:\nID: {e.Message.Chat.Id}\nName: {e.Message.Chat.FirstName}\nLast name: {e.Message.Chat.LastName}\n\n");
                                    users.Remove(user);
                                    DataControl.SetUsers(users);
                                    break;
                                }
                            }
                            foreach (var employee in allEmployees)
                            {
                                if (employee.Id == e.Message.Chat.Id)
                                {
                                    admins.Add(new Admin(employee));
                                    DataControl.SetAdmins(admins);
                                    // console
                                    Console.WriteLine($"Add new admin:\nID: {e.Message.Chat.Id}\nName: {e.Message.Chat.FirstName}\nLast name: {e.Message.Chat.LastName}\n\n");
                                    allEmployees.Remove(employee);
                                    DataControl.SetEmployees(allEmployees);
                                    break;
                                }
                            }
                            // console
                            Console.WriteLine($"Незарегистрированных пользователей: {users.Count}");
                            foreach (var admin in admins)
                            {
                                if (admin.Id == e.Message.Chat.Id)
                                {
                                    if (e.Message.Chat.Id == mainAdm || e.Message.Chat.Id == mainAdmNik)
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.Message.Chat.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForMainAdmin()));
                                    }
                                    else
                                    {
                                        admin.messages.Add(await bot.SendTextMessageAsync(
                                                             e.Message.Chat.Id,
                                                             "Меню",
                                                             replyMarkup: Buttons.MainMenuForAdmin()));
                                    }
                                    return;
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
