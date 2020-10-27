using System;

namespace DinoBotTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bot Started..."); // Объявление о запуске бота
            DinoBot bot = new DinoBot(); // создание экземпляра класса DinoBot
            bot.Start(); // запуск Бота
            Console.ReadKey();
        }
    }
}
