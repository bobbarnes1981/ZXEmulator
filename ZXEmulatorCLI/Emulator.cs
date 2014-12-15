using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXEmulatorLibrary;

namespace ZXEmulatorCLI
{
    public class Emulator
    {
        private ZX80 m_zx80;

        public Emulator(string path)
        {
            m_zx80 = new ZX80(path);
        }

        public void Run()
        {
            m_zx80.Run();
        }
    }
}
