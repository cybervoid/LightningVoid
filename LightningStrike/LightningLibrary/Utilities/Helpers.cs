using System;
namespace LightningLibrary.Utilities
{
    public static class Helpers
    {
        public static void ChangeColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }


        public static void ChangeColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }
    }
}
