using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public enum Condition
    {
        NS  = 0x7F,
        S   = 0x80,  // S
        NZ  = 0xBF,
        Z   = 0x40,  // Z
        PO  = 0xFB,
        PE  = 0x04,  // PV
        NC  = 0xFE,
        C   = 0x01,  // C
    }
}
