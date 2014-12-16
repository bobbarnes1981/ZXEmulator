using System;
using System.Collections.Generic;

namespace ZXEmulatorLibrary.ZX80
{
    public class Video
    {
        private Queue<byte> m_shiftRegister;

        public Video()
        {
            m_shiftRegister = new Queue<byte>();
        }

        public void Load(byte data, bool invert)
        {
            m_shiftRegister.Enqueue(data);
        }

        public void Shift()
        {
            byte data = 0x00;
            if (m_shiftRegister.Count > 0)
            {
                data = m_shiftRegister.Dequeue();
            }

            //display data
            //Console.Write("{0:x2}", data);
        }
    }
}
