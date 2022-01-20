using System;
using System.Collections.Generic;
using System.Text;

namespace DemoMYSQL
{
    public static class LOG
    {

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            //Console.BackgroundColor = color;
            Console.ForegroundColor = color;
            Console.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), message));

            Console.ResetColor();
        }

        public static void Write(string message, ConsoleColor color = ConsoleColor.White, bool center = false)
        {
            //Console.BackgroundColor = color;
            Console.ForegroundColor = color;

            if (center)
            {
                Console.Write(String.Format("{0," + ((Console.WindowWidth / 2) + (message.Length / 2)) + "}", message));
            }
            else
            {
                Console.Write($"[{DateTime.Now.ToString("HH:mm:ss")}] " + message);
            }
            Console.ResetColor();
        }

    }
}
