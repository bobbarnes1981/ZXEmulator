using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXEmulatorCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                new Emulator(args[0]).Run();
            }
            else
            {
                Console.WriteLine("Usage: ZXEmulatorCLI <filename>");
            }
        }
    }
}
