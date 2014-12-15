using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public class Bus
    {
        private Memory m_rom;
        private Memory m_ram;

        private int m_topOfRom;
        private int m_topOfRam;

        public Bus(Memory rom, Memory ram)
        {
            m_rom = rom;
            m_ram = ram;
            m_topOfRom = m_rom.Size;
            m_topOfRam = m_rom.Size + m_ram.Size;
        }

        public byte Read(short address)
        {
            if (address < m_topOfRom)
            {
                return m_rom.Read(address);
            }

            if (address < m_topOfRam)
            {
                return m_ram.Read((short)(address - m_topOfRom));
            }

            throw new Exception(string.Format("Invalid memory address {0:x4}", address));
        }

        public void Write(short address, byte data)
        {
            if (address < m_topOfRom)
            {
                throw new Exception(string.Format("Attempt to write to ROM address {0:x4}", address));
            }
            
            if (address < m_topOfRam)
            {
                m_ram.Write((short)(address - m_topOfRom), data);
            }
            else
            {
                throw new Exception(string.Format("Invalid memory address {0:x4}", address));
            }
        }
    }
}
