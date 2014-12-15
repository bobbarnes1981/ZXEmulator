using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public enum Condition
    {
        NS  = ~0x0080,
        S   =  0x0080,  // S
        NZ  = ~0x0040,
        Z   =  0x0040,  // Z
        PO  = ~0x0004,
        PE  =  0x0004,  // PV
        NC  = ~0x0001,
        C   =  0x0001,  // C
    }
}
