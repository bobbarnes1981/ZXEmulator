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
        /// <summary>
        /// The interrupt triggers the start of the display signal and the CPU interrupt
        /// this is supposed to happen every 64us (0.000064s), the cpu clock is 3.21Mhz
        /// (3210000hz), this gives 205.44 cycles per interrupt.
        /// </summary>
        private const uint INTERRUPT_PERIOD = 205;
        private uint m_interruptCounter;

        /// <summary>
        /// The video hardware shifts 8 bits to the display about every 4 cycles
        /// </summary>
        private const uint VIDEO_PEROID = 4;
        private uint m_videoCounter;

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
            m_interruptCounter = 0;
            m_videoCounter = 0;
            
            do
            {
                bool interrupt = false;

                m_interruptCounter += cycles;
                while (m_interruptCounter > INTERRUPT_PERIOD)
                {
                    m_interruptCounter -= INTERRUPT_PERIOD;
                    interrupt = true;
                }

                m_videoCounter += cycles;
                while (m_videoCounter > VIDEO_PEROID)
                {
                    m_videoCounter -= VIDEO_PEROID;
                    m_video.Shift();
                }

                // TODO:  calculate elapsed time in milliseconds
                cycles = m_cpu.Step(interrupt);
            } while (m_running);
        }
    }
}
