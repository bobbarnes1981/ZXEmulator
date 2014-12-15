using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXEmulatorLibrary;

namespace ZXEmulatorCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                HardwareType hardwareType;
                if (Enum.TryParse<HardwareType>(args[0], out hardwareType))
                {
                    new Emulator(hardwareType, args[1]).Run();
                }
                else
                {
                    usage();
                }
            }
            else
            {
                usage();
            }
        }

        static void usage()
        {
            Console.WriteLine("Usage: ZXEmulatorCLI <ZX80|ZX81> <filename>");
        }
    }
}
