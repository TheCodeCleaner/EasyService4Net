using System;
using System.IO;

namespace EasyService4Net.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var easyService = new EasyService();

            easyService.OnServiceStarted += () =>
            {
                Console.WriteLine("Hello World!");
                File.WriteAllText("C:\\exampleFile.txt", "Hello World!");
            };

            easyService.Run(args);
        }
    }
}
