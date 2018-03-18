using System;
using System.Collections.Generic;
using System.Threading;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace ConsoleApplication2
{
    class Program
    {
        private static VkApi api;
        private static VkCollection<User> FriendListUsers()
        {
            return api.Friends.Get(new FriendsGetParams()
            {
                UserId = api.UserId,
                Fields = ProfileFields.FirstName | ProfileFields.LastName
            });
        }

        private static MessagesGetObject historyMessage(long id)
        {
            return api.Messages.GetHistory(new MessagesGetHistoryParams()
            {
                Count = 1,
                UserId = id
            });
        }

        private static void Sinhronize()
        {
            var friends = FriendListUsers();

            List<long> Key = new List<long>();
            List<long> Value = new List<long>();
            List<User> nameFirends = new List<User>();
            int count = 0;

            foreach (var VARIABLE in friends)
            {
                var messages = historyMessage(VARIABLE.Id);

                if (messages.Messages.Count != 0)
                {
                    Key.Add(VARIABLE.Id);
                    nameFirends.Add(VARIABLE);
                    Value.Add((long)messages.Messages[0].Id);
                }
                Console.Clear();

                Console.WriteLine("Синхронизировано диалогов " + (count++) + " из " + friends.Count);

            }

            MessageVisitor(Key, Value, nameFirends);

        }

        private static void MessageVisitor(List<long> Key, List<long> Value, List<User> friends)
        {
            int messages = 0;
            List<string> name = new List<string>();
            Console.WriteLine("Установите таймер автоматической отправки сообщения мин.");
            int time = (Convert.ToInt32(Console.ReadLine()) * 60000);

            while (true)
            {

                for (int a = 0; a < Key.Count; a++)
                {
                    var history = historyMessage(Key[a]);

                    if (Value[a] != history.Messages[0].Id && Key[a] == history.Messages[0].UserId)
                    {
                        Thread.Sleep(time);
                        history = historyMessage(Key[a]);
                        if (history.Messages[0].UserId != api.UserId)
                        {
                            messages++;
                            name.Add(friends[a].FirstName + " " + friends[a].LastName);
                            api.Messages.Send(new MessagesSendParams()
                            {
                                UserId = Key[a],
                                Message = "Привет " + name[name.Count - 1] +
                                          ",Если ты читаешь это сообщение,значит,я не могу ответить на него лично,но как только появится такая возможность я обязательно отвечу)"
                            });

                            Thread.Sleep(20000);
                            Value[a] = (long)historyMessage(Key[a]).Messages[0].Id;
                        }

                    }

                    Console.Clear();
                    Console.WriteLine("Новых сообщений  " + messages);
                    foreach (var VARIABLE in name)
                    {
                        Console.WriteLine(VARIABLE);
                    }
                }

                Thread.Sleep(10000);

            }

        }

        private static void autorize()
        {
            Console.WriteLine("Введите login");
            string login = Console.ReadLine();
            Console.WriteLine("Введите password");
            string password = Console.ReadLine();

            try
            {
                api.Authorize(new ApiAuthParams()
                {
                    Login = login,
                    Password = password,
                    ApplicationId = 6390479,
                    Settings = Settings.All
                });
            }
            catch (VkNet.Exception.VkApiException)
            {
                Console.WriteLine("Неверный логин или пароль.Повторите авторизацию!");
                Console.ReadKey();
                Console.Clear();
                autorize();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("VkBot 0.0.1" + "@Tema Kyspits");

            api = new VkApi();
            autorize();
            Console.WriteLine("Авторизация");
            Console.WriteLine("Бот авторизирован");
            Console.WriteLine("Для продолжения нажмите любую клавишу");
            Console.ReadKey();
            Console.Clear();
            Sinhronize();
            Console.ReadKey();
        }
    }
}

