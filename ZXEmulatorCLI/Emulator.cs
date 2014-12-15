using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXEmulatorLibrary;

namespace ZXEmulatorCLI
{
    public class Emulator
    {
        private IHardware m_hardware;

        public Emulator(HardwareType hardware, string path)
        {
            switch(hardware)
            {
                case HardwareType.ZX80:
                    m_hardware = new ZX80(path);
                    break;
                case HardwareType.ZX81:
                    m_hardware = new ZX81(path);
                    break;
                default:
                    throw new NotImplementedException(hardware.ToString());
            }
        }

        public void Run()
        {
            m_hardware.Run();
        }
    }
}
