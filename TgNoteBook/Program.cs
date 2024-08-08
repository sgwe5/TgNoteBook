using System;

namespace TgNoteBook;

class Program
{
    static void Main(string[] args)
    {
        Bot bot = new Bot();
        bot.Start();
        Console.ReadKey();  
    }
}