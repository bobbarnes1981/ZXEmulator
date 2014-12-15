using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public class Memory
    {
        private byte[] m_memory;

        public ushort Size
        {
            get { return (ushort)m_memory.Length; }
        }

        public Memory(ushort size)
        {
            m_memory = new byte[size];
        }

        public Memory(ushort size, byte[] data)
            : this(size)
        {
            if (data.Length > ushort.MaxValue)
            {
                throw new Exception("ROM is too big");
            }
            for (ushort i = 0; i < data.Length; i++)
            {
                m_memory[i] = data[i];
            }
        }

        public byte Read(ushort address)
        {
            return m_memory[address];
        }

        public void Write(ushort address, byte data)
        {
            m_memory[address] = data;
        }
    }
}
