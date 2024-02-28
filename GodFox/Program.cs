using System;
using MafiaRules;

namespace GodFox
{
    public static class Program
    {
        
        private static Random randomizer;

        public static void Main(string[] args)
        {
            VkLongpollServer server;
            Console.WriteLine("Приложение запущено! ");
            Console.WriteLine("Инициализация сервера.. ");
            randomizer = new Random();
            Settings.RandomInteger += Rand;
            server = new VkLongpollServer();
            Console.WriteLine("Запуск сервера... ");
            server.Start();
        }

        public static int Rand() => randomizer.Next();

    }
}
