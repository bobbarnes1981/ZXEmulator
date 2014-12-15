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

        public short Size
        {
            get { return (short)m_memory.Length; }
        }

        public Memory(short size)
        {
            m_memory = new byte[size];
        }

        public Memory(short size, byte[] data)
            : this(size)
        {
            if (data.Length > short.MaxValue)
            {
                throw new Exception("ROM is too big");
            }
            for (short i = 0; i < data.Length; i++)
            {
                m_memory[i] = data[i];
            }
        }

        public byte Read(short address)
        {
            return m_memory[address];
        }

        public void Write(short address, byte data)
        {
            m_memory[address] = data;
        }
    }
}
