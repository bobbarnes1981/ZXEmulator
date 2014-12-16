using System;

namespace ZXEmulatorLibrary.ZX80
{
    public class Bus : IBus
    {
        private Memory m_rom;
        private Memory m_ram;
        private Video m_video;

        private int m_topOfRom;
        private int m_topOfRam;

        public Bus(Memory rom, Memory ram, Video video)
        {
            m_rom = rom;
            m_ram = ram;
            m_video = video;

            m_topOfRom = m_rom.Size;
            m_topOfRam = m_rom.Size + m_ram.Size;
        }

        public byte Read(ushort address)
        {
            return Read(address, true);
        }

        public byte Read(ushort address, bool memreq)
        {
            if (memreq)
            {
                return ReadMemory(address);
            }
            else
            {
                return ReadIO(address);
            }
        }

        private byte ReadIO(ushort address)
        {
            throw new NotImplementedException();
        }

        private byte ReadMemory(ushort address)
        {
            if (address < m_topOfRom)
            {
                return m_rom.Read(address);
            }

            if (address < m_topOfRam)
            {
                return m_ram.Read((ushort)(address - m_topOfRom));
            }

            // ZX80 hardware for video

            // bit 15 of address is set >0x8000 so mirror ram
            byte data = ReadMemory((ushort) (address - m_ram.Size));
            // if bit 6 is 1 return opcode
            if ((0x40 & data) == 0x40)
            {
                // video displays white
                return data;
            }
            bool invertVideo = false;
            // if bit 7 is 1 invert video
            if ((0x80 & data) == 0x80)
            {
                invertVideo = true;
            }
            // use data as address in rom
            ushort romAddress = (ushort)((0x68 & data) + 0x0E00); // + line counter (currently we always get top line of char)
            // send rom data to video output (bit 7 is invert attribute)
            byte videoData = ReadMemory(romAddress);
            // send video data to video hardware
            m_video.Load(videoData, invertVideo);
            // send nop to cpu if bit 6 is zero
            return 0x00;
        }

        public void Write(ushort address, byte data)
        {
            Write(address, data, true);
        }

        public void Write(ushort address, byte data, bool memreq)
        {
            if (memreq)
            {
                WriteMemory(address, data);
            }
            else
            {
                WriteIO(address, data);
            }
        }

        private void WriteIO(ushort address, byte data)
        {
            throw new NotImplementedException();
        }

        private void WriteMemory(ushort address, byte data)
        {
            if (address < m_topOfRom)
            {
                throw new Exception(string.Format("Attempt to write to ROM address {0:x4}", address));
            }
            
            if (address < m_topOfRam)
            {
                m_ram.Write((ushort)(address - m_topOfRom), data);
            }
            else
            {
                //throw new Exception(string.Format("Invalid memory address {0:x4}", address));
                return;
            }
        }
    }
}
