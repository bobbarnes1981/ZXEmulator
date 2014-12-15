using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public enum FlagMask
    {
        NS  = ~0x0080,
        S   =  0x0080,  // S
        NZ  = ~0x0040,
        Z   =  0x0040,  // Z
        // undocumented flag 5
        NH  = ~0x0010,
        H   =  0x0010,  // H
        // undocumented flag 3
        PO  = ~0x0004,
        PE  =  0x0004,  // PV
        NN  = ~0x0002,
        N   =  0x0002,  // N
        NC  = ~0x0001,
        C   =  0x0001,  // C
    }
}
