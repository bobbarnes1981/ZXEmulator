using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public class RegisterPair
    {
        private ushort m_register;

        public byte Hi
        {
            get { return (byte)(m_register >> 8); }
            set { m_register = (ushort) ((m_register & 0x00FF) | (value << 8)); }
        }

        public byte Lo
        {
            get { return (byte)(m_register & 0x00FF); }
            set { m_register = (ushort) ((m_register & 0xFF00) | value); }
        }

        public ushort Register
        {
            get { return m_register; }
            set { m_register = value; }
        }
    }
}
