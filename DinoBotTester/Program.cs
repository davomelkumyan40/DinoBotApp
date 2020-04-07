using System;

namespace DinoBotTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bot Started...");
            DinoBot bot = new DinoBot();
            bot.Start();
            Console.ReadKey();
        }
    }
}
