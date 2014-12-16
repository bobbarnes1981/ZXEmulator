using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ZXEmulatorLibrary.ZX80
{
    public class Hardware : IHardware
    {
        private uint m_videoShiftPeriod = 4;
        private uint m_videoShiftCounter;

        private bool m_running = true;

        private int m_videoWidth = 256;
        private int m_videoHeight = 192;

        //  ____0___1___2___3___4___5___6___7___8___9___A___B___C___D___E___F____
        //  00 SPC  "  [: ][..][' ][ '][. ][ .][.']{::}{..}{''}GBP  $   :   ?  0F
        //  10  (   )   -   +   *   /   =   >   <   ;   ,   .   0   1   2   3  1F
        //  20  4   5   6   7   8   9   A   B   C   D   E   F   G   H   I   J  2F
        //  30  K   L   M   N   O   P   Q   R   S   T   U   V   W   X   Y   Z  3F
        public static readonly char[] CharacterMap = new char[]
        {
            ' ', '"', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '£', '$', ':', '?',
            '(', ')', '-', '+', '*', '/', '=', '>', '<', ';', ',', '.', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        private Memory m_rom;
        private Memory m_ram;
        private Video m_video;
        private Bus m_bus;
        private Z80 m_cpu;

        public Hardware(string path)
        {
            m_rom = new Memory(0x4000, File.ReadAllBytes(path));
            m_ram = new Memory(0x4000);
            m_video = new Video();
            m_bus = new Bus(m_rom, m_ram, m_video);
            m_cpu = new Z80(m_bus);
        }

        public void Run()
        {
            uint cycles = 0;
            m_videoShiftCounter = 0;

            do
            {
                m_videoShiftCounter += cycles;
                if (m_videoShiftCounter > m_videoShiftPeriod)
                {
                    m_videoShiftCounter -= m_videoShiftPeriod;
                    m_video.Shift();
                }

                // TODO:  calculate elapsed time in milliseconds
                cycles = m_cpu.Step();
            } while (m_running);
        }
    }
}
