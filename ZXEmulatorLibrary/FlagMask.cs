namespace ZXEmulatorLibrary
{
    public enum FlagMask
    {
        NS  = 0x7F,
        S   = 0x80,  // S
        NZ  = 0xBF,
        Z   = 0x40,  // Z
        // undocumented flag 5
        NH  = 0xEF,
        H   = 0x10,  // H
        // undocumented flag 3
        PO  = 0xFB,
        PE  = 0x04,  // PV
        NN  = 0xFD,
        N   = 0x02,  // N
        NC  = 0xFE,
        C   = 0x01,  // C
    }
}
