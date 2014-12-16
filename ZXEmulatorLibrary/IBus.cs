namespace ZXEmulatorLibrary
{
    public interface IBus
    {
        byte Read(ushort address);
        byte Read(ushort address, bool memreq);
        void Write(ushort address, byte data);
        void Write(ushort address, byte data, bool memreq);
    }
}