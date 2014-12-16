using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary.ZX81
{
    class Bus : IBus
    {
        public Bus(Memory rom, Memory ram, Video video)
        {
            throw new NotImplementedException();
        }

        public byte Read(ushort address)
        {
            throw new NotImplementedException();
        }

        public byte Read(ushort address, bool memreq)
        {
            throw new NotImplementedException();
        }

        public void Write(ushort address, byte data)
        {
            throw new NotImplementedException();
        }

        public void Write(ushort address, byte data, bool memreq)
        {
            throw new NotImplementedException();
        }
    }
}
