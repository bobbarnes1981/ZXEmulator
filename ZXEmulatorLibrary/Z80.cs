using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public class Z80
    {
        private int m_interruptPeriod = 28;
        private int m_interruptCycles;

        private bool m_IFF1 = false;
        private bool m_IFF2 = false;

        private short m_programCounter = 0x0000;
        private byte m_instructionRegister = 0x00;
        
        private bool m_halted = false;
        
        private InterruptMode m_interruptMode = InterruptMode.None;
        
        private short m_AF;
        private short m_BC;
        private short m_DE;
        private short m_HL;
        private short m_SP;

        private short m_IX;
        private short m_IY;

        private byte m_I = 0x00;
        private byte m_R = 0x00;

        private Bus m_bus;

        public Z80(Bus bus)
        {
            m_bus = bus;
        }

        public void Step()
        {
            // TODO:  calculate elapsed time in milliseconds

            int requiredCycles = 1;
            m_interruptCycles += requiredCycles;

            m_R = (byte)((m_R + 1) & 0x7F);

            // should interrupts  be in this loop?
            while (requiredCycles > 0)
            {
                if (!m_halted)
                {
                    requiredCycles -= executeNextOpcode();
                }
                else
                {
                    requiredCycles -= nop();
                }
            }

            switch(m_interruptMode)
            {
                case InterruptMode.Mode_0: throw new NotImplementedException("Interrupt mode 0 not implemented"); break;
                case InterruptMode.Mode_1: if (m_interruptCycles > m_interruptPeriod) { m_interruptCycles = 0; m_programCounter = 0x0038; m_halted = false; } break;
                case InterruptMode.Mode_2: throw new NotImplementedException("Interrupt Mode 2 not implemented"); break;
            }
        }

        private int executeNextOpcode()
        {
            m_instructionRegister = m_bus.Read(m_programCounter);
            m_programCounter++;
            return executeOpcode();
        }

        private int executeOpcode()
        {
            int cycles = 0;
            switch(m_instructionRegister)
            {
                case 0x00: cycles = nop(); break;
                case 0x01: cycles = ld_dd_nn(RegisterPairSP.BC); break;

                case 0x03: cycles = inc_ss(RegisterPairSP.BC); break;

                case 0x05: cycles = dec_m(RegisterExt.B); break;
                case 0x06: cycles = ld_r_n(Register.B); break;

                case 0x0B: cycles = dec_ss(RegisterPairSP.BC); break;

                case 0x0D: cycles = dec_m(RegisterExt.C); break;
                case 0x0E: cycles = ld_r_n(Register.C); break;

                case 0x11: cycles = ld_dd_nn(RegisterPairSP.DE); break;

                case 0x13: cycles = inc_ss(RegisterPairSP.DE); break;

                case 0x15: cycles = dec_m(RegisterExt.D); break;
                case 0x16: cycles = ld_r_n(Register.D); break;

                case 0x18: cycles = jr_e(); break;

                case 0x1B: cycles = dec_ss(RegisterPairSP.DE); break;

                case 0x1D: cycles = dec_m(RegisterExt.E); break;
                case 0x1E: cycles = ld_r_n(Register.E); break;

                case 0x20: cycles = jr_nz_e(); break;
                case 0x21: cycles = ld_dd_nn(RegisterPairSP.HL); break;
                case 0x22: cycles = ld__nn__hl(); break;

                case 0x23: cycles = inc_ss(RegisterPairSP.HL); break;

                case 0x25: cycles = dec_m(RegisterExt.H); break;
                case 0x26: cycles = ld_r_n(Register.H); break;

                case 0x2A: cycles = ld_hl__nn__(); break;
                case 0x2B: cycles = dec_ss(RegisterPairSP.HL); break;

                case 0x2D: cycles = dec_m(RegisterExt.L); break;
                case 0x2E: cycles = ld_r_n(Register.L); break;

                case 0x28: cycles = jr_z_e(); break;

                case 0x31: cycles = ld_dd_nn(RegisterPairSP.SP); break;

                case 0x33: cycles = inc_ss(RegisterPairSP.SP); break;

                case 0x35: cycles = dec_m(RegisterExt.HL); break;
                case 0x36: cycles = ld__hl__n(); break;

                case 0x3B: cycles = dec_ss(RegisterPairSP.SP); break;

                case 0x3D: cycles = dec_m(RegisterExt.A); break;
                case 0x3E: cycles = ld_r_n(Register.A); break;

                case 0x60: cycles = ld_r_r(Register.H, Register.B); break;

                case 0x69: cycles = ld_r_r(Register.L, Register.C); break;

                case 0x76: cycles = halt(); break;

                case 0xB8: cycles = cp_s(RegisterExtN.B); break;
                case 0xB9: cycles = cp_s(RegisterExtN.C); break;
                case 0xBA: cycles = cp_s(RegisterExtN.D); break;
                case 0xBB: cycles = cp_s(RegisterExtN.E); break;
                case 0xBC: cycles = cp_s(RegisterExtN.H); break;
                case 0xBD: cycles = cp_s(RegisterExtN.L); break;
                case 0xBE: cycles = cp_s(RegisterExtN.HL); break;
                case 0xBF: cycles = cp_s(RegisterExtN.A); break;

                case 0xC0: cycles = ret_cc(Condition.NZ); break;
                case 0xC1: cycles = pop_qq(RegisterPairAF.BC); break;
                case 0xC2: cycles = jp_cc_nn(Condition.NZ); break;
                case 0xC3: cycles = jp_nn(); break;

                case 0xC5: cycles = push_qq(RegisterPairAF.BC); break;

                case 0xC8: cycles = ret_cc(Condition.Z); break;
                case 0xC9: cycles = ret(); break;
                case 0xCA: cycles = jp_cc_nn(Condition.Z); break;

                case 0xCD: cycles = call_nn(); break;

                case 0xD0: cycles = ret_cc(Condition.C); break;
                case 0xD1: cycles = pop_qq(RegisterPairAF.DE); break;
                case 0xD2: cycles = jp_cc_nn(Condition.C); break;
                case 0xD3: cycles = out__n__a();break;

                case 0xD5: cycles = push_qq(RegisterPairAF.DE); break;

                case 0xD8: cycles = ret_cc(Condition.C); break;

                case 0xDA: cycles = jp_cc_nn(Condition.C); break;
                case 0xDD: cycles = executeOpcodeDD(); break;

                case 0xE0: cycles = ret_cc(Condition.PO); break;
                case 0xE1: cycles = pop_qq(RegisterPairAF.HL); break;
                case 0xE2: cycles = jp_cc_nn(Condition.PO); break;
                case 0xE5: cycles = push_qq(RegisterPairAF.HL); break;

                case 0xE8: cycles = ret_cc(Condition.PE); break;
                case 0xE9: cycles = jp__hl__(); break;
                case 0xEA: cycles = jp_cc_nn(Condition.PE); break;

                case 0xED: cycles = executeOpcodeED(); break;

                case 0xF0: cycles = ret_cc(Condition.NS); break;
                case 0xF1: cycles = pop_qq(RegisterPairAF.AF); break;
                case 0xF2: cycles = jp_cc_nn(Condition.NS); break;

                case 0xF5: cycles = push_qq(RegisterPairAF.AF); break;

                case 0xF8: cycles = ret_cc(Condition.S); break;
                case 0xF9: cycles = ld_sp_hl(); break;
                case 0xFA: cycles = jp_cc_nn(Condition.S); break;
                case 0xFB: cycles = ei(); break;

                case 0xFD: cycles = executeOpcodeFD(); break;
                case 0xFE: cycles = cp_s(RegisterExtN.N); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: {0x4} @ {1x6}\r\n", m_instructionRegister, m_programCounter));
            }
            return cycles;
        }

        private int executeOpcodeDD()
        {
            int cycles = 0;
            byte opcode = m_bus.Read(m_programCounter);
            m_programCounter++;
            switch (opcode)
            {
                case 0x35: cycles = dec_m(RegisterExt.IXd); break;

                case 0xBE: cycles = cp_s(RegisterExtN.IXd); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xDD {0x4} @ {0x6}\r\n", opcode, m_programCounter - 1));
            }

            return cycles;
        }

        private int executeOpcodeED()
        {
            int cycles = 0;
            byte opcode = m_bus.Read(m_programCounter);
            m_programCounter++;
            switch (opcode)
            {
                case 0x43: cycles = ld__nn__dd(RegisterPairSP.BC); break;

                case 0x46: cycles = im_0(); break;
                case 0x47: cycles = ld_i_a(); break;

                case 0x4F: cycles = ld_r_a(); break;

                case 0x53: cycles = ld__nn__dd(RegisterPairSP.DE); break;

                case 0x56: cycles = im_1(); break;

                case 0x5E: cycles = im_2(); break;

                case 0x63: cycles = ld__nn__dd(RegisterPairSP.HL); break;

                case 0x73: cycles = ld__nn__dd(RegisterPairSP.SP); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xED {0x4} @ {0x6}\r\n", opcode, m_programCounter - 1));
            }
            return cycles;
        }

        private int executeOpcodeFD()
        {
            int cycles = 0;
            byte opcode = m_bus.Read(m_programCounter);
            m_programCounter++;
            switch (opcode)
            {
                case 0x21: cycles = ld_iy_nn(); break;

                case 0x35: cycles = dec_m(RegisterExt.IYd); break;
                case 0x36: cycles = ld_iyd_n(); break;

                case 0xBE: cycles = cp_s(RegisterExtN.IYd); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xFD %#04x @ %#06x\r\n", opcode, m_programCounter - 1));
            }
            return cycles;
        }

        private byte getN()
        {
            byte n = 0x00;
            n = m_bus.Read(m_programCounter);
            m_programCounter++;
            return n;
        }

        private short getNN()
        {
            short nn = 0x0000;
            nn |= (short)m_bus.Read(m_programCounter);
            m_programCounter++;
            nn |= (short)(m_bus.Read(m_programCounter) << 8);
            m_programCounter++;
            return nn;
        }

        private int nop()
        {
            //Description: The CPU performs no operation during this machine cycle.
            //	M Cycles	T States	4 MHz E.T.
            //	1			4			1.00
            //Condition Bits Affected: None
            return 4;
        }

        private int out__n__a()
        {
            //Description: The operand n is placed on the bottom half (A0 through A7) of the address
            //bus to select the I/O device at one of 256 possible ports. The contents of the
            //Accumulator (register A) also appear on the top half (A8 through A15) of
            //the address bus at this time. Then the byte contained in the Accumulator is
            //placed on the data bus and written to the selected peripheral device.
            //	M Cycles	T States		4 MHz E.T.
            //	3			11 (4, 3, 4)	2.75
            //Condition Bits Affected: None

            //TODO: implement this
            byte n = getN();

            throw new NotImplementedException();

            return 11;
        }

        private int ld_dd_nn(RegisterPairSP reg)
        {
            //The 2-byte integer nn is loaded to the dd register pair, where dd defines the
            //BC, DE, HL, or SP register pairs, assembled as follows in the object code:
            //Pair dd
            //	BC 00
            //	DE 01
            //	HL 10
            //	SP 11
            //The first n operand after the Op Code is the low order byte.
            //	M Cycles	T States		4 MHz E.T.
            //	2			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            short nn = getNN();
            switch(reg)
            {
                case RegisterPairSP.BC: m_BC = nn; break;
                case RegisterPairSP.DE: m_DE = nn; break;
                case RegisterPairSP.HL: m_HL = nn; break;
                case RegisterPairSP.SP: m_SP = nn; break;
            }
            return 10;
        }

        private int ld_r_r(Register reg1, Register reg2)
        {
            //Description: The contents of any register r' are loaded to any other register r. r, r'
            //identifies any of the registers A, B, C, D, E, H, or L, assembled as follows
            //in the object code:
            //Register r, C
            //	A 111
            //	B 000
            //	C 001
            //	D 010
            //	E 011
            //	H 100
            //	L 101
            //	M Cycles	T States	MHz E.T.
            //	1			4			1.0
            //Condition Bits Affected: None
            byte data = 0x00;
            switch (reg1)
            {
                case Register.B: data = (byte)(m_BC >> 8); break;
                case Register.C: data = (byte)(m_BC & 0x00FF); break;
                case Register.D: data = (byte)(m_DE >> 8); break;
                case Register.E: data = (byte)(m_DE & 0x00FF); break;
                case Register.H: data = (byte)(m_HL >> 8); break;
                case Register.L: data = (byte)(m_HL & 0x00FF); break;
                case Register.A: data = (byte)(m_AF >> 8); break;
            }
            switch (reg2)
            {
                case Register.B: m_BC &= (byte)((data << 8) | 0x00FF); break;
                case Register.C: m_BC &= (byte)(data | 0xFF00); break;
                case Register.D: m_DE &= (byte)((data << 8) | 0x00FF); break;
                case Register.E: m_DE &= (byte)(data | 0xFF00); break;
                case Register.H: m_HL &= (byte)((data << 8) | 0x00FF); break;
                case Register.L: m_HL &= (byte)(data | 0xFF00); break;
                case Register.A: m_AF &= (byte)((data << 8) | 0x00FF); break;
            }
            return 4;
        }

        private int ld_r_a()
        {
            //Description: The contents of the Accumulator are loaded to the Memory Refresh
            //register R.
            //	M Cycles	T States	MHz E.T.
            //	2			9 (4, 5)	2.25
            //Condition Bits Affected: None
            m_R = (byte)(m_AF >> 8);
            return 9;
        }

        private int jr_e()
        {
            //Description: This instruction provides for unconditional branching to other segments of
            //a program. The value of the displacement e is added to the Program
            //Counter (PC) and the next instruction is fetched from the location
            //designated by the new contents of the PC. This jump is measured from the
            //address of the instruction Op Code and has a range of-126 to +129 bytes.
            //The assembler automatically adjusts for the twice incremented PC.
            //	M Cycles	T States		4 MHz E.T.
            //	3			12 (4, 3, 5)	3.00
            //Condition Bits Affected: None
            byte e = getN();
            m_programCounter = (short)(m_programCounter + e);
            return 12;
        }

        private int jr_nz_e()
        {
            //Description: This instruction provides for conditional branching to other segments of a
            //program depending on the results of a test on the Zero Flag. If the flag is
            //equal to a 0, the value of the displacement e is added to the Program
            //Counter (PC) and the next instruction is fetched from the location
            //designated by the new contents of the PC. The jump is measured from the
            //address of the instruction Op Code and has a range of -126 to +129 bytes.
            //The assembler automatically adjusts for the twice incremented PC.
            //If the Zero Flag is equal to a 1, the next instruction executed is taken from
            //the location following this instruction.
            //If the condition is met:
            //	M Cycles	T States		4 MHz E.T.
            //	3			12 (4, 3, 5)	3.00
            //If the condition is not met:
            //	M Cycles	T States		4 MHz E.T.
            //	2			7 (4, 3)		1.75
            //Condition Bits Affected: None
            int cycles = 7;
            byte e = getN();
            if (check_condition(Condition.NZ))
            {
                m_programCounter = (short)(m_programCounter + e);
                cycles = 12;
            }
            return cycles;
        }

        private int jr_z_e()
        {
            //Description: This instruction provides for conditional branching to other segments of a
            //program depending on the results of a test on the Zero Flag. If the flag is
            //equal to a 1, the value of the displacement e is added to the Program
            //Counter (PC) and the next instruction is fetched from the location
            //designated by the new contents of the PC. The jump is measured from the
            //address of the instruction Op Code and has a range of -126 to +129 bytes.
            //The assembler automatically adjusts for the twice incremented PC.
            //If the Zero Flag is equal to a 0, the next instruction executed is taken from
            //the location following this instruction. If the condition is met:
            //	M Cycles	T States		4 MHz E.T.
            //	3			12 (4, 3, 5)	3.00
            //If the condition is not met;
            //	M Cycles	T States		4 MHz E.T.
            //	2			7 (4, 3)		1.75
            //Condition Bits Affected: None
            int cycles = 7;
            byte e = getN();
            if (check_condition(Condition.Z))
            {
                m_programCounter = (short)(m_programCounter + e);
                cycles = 12;
            }
            return cycles;
        }

        private int jp_cc_nn(Condition flg)
        {
            //Description: If condition cc is true, the instruction loads operand nn to register pair PC
            //(Program Counter), and the program continues with the instruction
            //beginning at address nn. If condition cc is false, the Program Counter is
            //incremented as usual, and the program continues with the next sequential
            //instruction. Condition cc is programmed as one of eight status that
            //corresponds to condition bits in the Flag Register (register F). These eight
            //status are defined in the table below that also specifies the corresponding
            //cc bit fields in the assembled object code.
            //Relevant
            //	cc Condition Flag
            //	000 NZ non zero Z
            //	001 Z zero Z
            //	010 NC no carry C
            //	011 C carry C
            //	100 PO parity odd P/V
            //	101 PE parity even P/V
            //	110 P sign positive S
            //	111 M sign negative S
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            short nn = getNN();
            if (check_condition(flg))
            {
                m_programCounter = nn;
            }
            return 10;
        }

        private int dec_m(RegisterExt reg)
        {
            //Register r
            //	B 000
            //	C 001
            //	D 010
            //	E 011
            //	H 100
            //	L 101
            //	A 111
            //Description: The byte specified by the m operand is decremented.
            //Instruction	M Cycles	T States				4 MHz E.T.
            //	DEC r		1 			4						1.00
            //	DEC (HL)	3 			11 (4, 4, 3				2.75
            //	DEC (IX+d)	6 			23 (4, 4, 3, 5, 4, 3)	5.75
            //	DEC (lY+d)	6			23 (4, 4, 3, 5, 4, 3)	5.75
            //Condition Bits Affected:
            //	S is set if result is negative; reset otherwise
            //	Z is set if result is zero; reset otherwise
            //	H is set if borrow from bit 4, reset otherwise
            //	P/V is set if m was 80H before operation; reset otherwise
            //	N is set
            //	C is not affected
            int cycles = 4;
            short mem;
            switch(reg)
            {
                case RegisterExt.A: m_AF &= (short)(dec((byte)(m_AF & 0x00FF)) | 0xFF00); break;
                case RegisterExt.B: m_BC &= (short)((dec((byte)((m_BC & 0xFF00) >> 8)) << 8) | 0x00FF); break;
                case RegisterExt.C: m_BC &= (short)(dec((byte)(m_BC & 0x00FF)) | 0xFF00); break;
                case RegisterExt.D: m_DE &= (short)((dec((byte)((m_DE & 0xFF00) >> 8)) << 8) | 0x00FF); break;
                case RegisterExt.E: m_DE &= (short)(dec((byte)(m_DE & 0x00FF)) | 0xFF00); break;
                case RegisterExt.H: m_HL &= (short)((dec((byte)((m_HL & 0xFF00) >> 8)) << 8) | 0x00FF); break;
                case RegisterExt.L: m_HL &= (short)(dec((byte)(m_HL & 0x00FF)) | 0xFF00); break;

                case RegisterExt.HL: m_bus.Write(m_HL, dec(m_bus.Read(m_HL))); cycles = 11; break;

                case RegisterExt.IXd: mem = (byte)(m_bus.Read(m_programCounter) + m_IX); m_programCounter++; m_bus.Write(mem, dec(m_bus.Read(mem))); cycles = 23; break;
                case RegisterExt.IYd: mem = (byte)(m_bus.Read(m_programCounter) + m_IY); m_programCounter++; m_bus.Write(mem, dec(m_bus.Read(mem))); cycles = 23; break;
            }
            return cycles;
        }

        private byte dec(byte input)
        {
            //Condition Bits Affected:
            //	S is set if result is negative; reset otherwise
            //	Z is set if result is zero; reset otherwise
            //	H is set if borrow from bit 4, reset otherwise
            //	P/V is set if m was 80H before operation; reset otherwise
            //	N is set
            //	C is not affected
            input--;
            if (input < 0)
            {
                m_AF |= (short)Condition.S;
            }
            else
            {
                m_AF &= (short)Condition.NS;
            }
            if (input == 0)
            {
                m_AF |= (short)Condition.Z;
            }
            else
            {
                m_AF &= (short)Condition.NZ;
            }
            //TODO: H FLAG
            if (input == 0x7F)
            {
                m_AF |= (short)Condition.PE;
            }
            else
            {
                m_AF &= (short)Condition.PO;
            }
            m_AF |= (short)FlagMask.N;
            return input;
        }

        private int dec_ss(RegisterPairSP reg)
        {
            //Description: The contents of register pair ss (any of the register pairs BC, DE, HL, or
            //SP) are decremented. Operand ss is specified as follows in the assembled
            //object code.
            //Register
            //Pair ss
            //	BC 00
            //	DE 01
            //	HL 10
            //	SP 11
            //	M Cycles	T States	4 MHz E.T.
            //	1			6			1.50
            //Condition Bits Affected: None
            switch(reg)
            {
                case RegisterPairSP.BC: m_BC--; break;
                case RegisterPairSP.DE: m_DE--; break;
                case RegisterPairSP.HL: m_HL--; break;
                case RegisterPairSP.SP: m_SP--; break;
            }
            return 6;
        }

        private int ld_r_n(Register reg)
        {
            //Description: The 8-bit integer n is loaded to any register r, where r identifies register A,
            //B, C, D, E, H, or L, assembled as follows in the object code:
            //Register r
            //	A 111
            //	B 000
            //	C 001
            //	D 010
            //	E 011
            //	H 100
            //	L 101
            //	M Cycles	T States	4 MHz E.T.
            //	2			7 (4, 3)	1.75
            //Condition Bits Affected: None
            byte n = getN();
            switch(reg)
            {
                case Register.A: m_AF = (short)((m_AF & 0x00FF) | (n << 8)); break;
                case Register.B: m_BC = (short)((m_BC & 0xFF00) | (n)); break;
                case Register.C: m_BC = (short)((m_BC & 0x00FF) | (n << 8)); break;
                case Register.D: m_DE = (short)((m_DE & 0xFF00) | (n)); break;
                case Register.E: m_DE = (short)((m_DE & 0x00FF) | (n << 8)); break;
                case Register.H: m_HL = (short)((m_HL & 0xFF00) | (n)); break;
                case Register.L: m_HL = (short)((m_HL & 0x00FF) | (n << 8)); break;
            }
            return 7;
        }

        private int inc_ss(RegisterPairSP reg)
        {
            //Description: The contents of register pair ss (any of register pairs BC, DE, HL, or SP) 
            //are incremented. Operand ss is specified as follows in the assembled 
            //object code.
            //Register
            //Pair ss
            //	BC 00
            //	DE 01
            //	HL 10
            //	SP 11
            //	M Cycles	T States	4 MHz E.T.
            //	1			6			1.50
            //Condition Bits Affected: None
            switch(reg)
            {
                case RegisterPairSP.BC: m_BC++; break;
                case RegisterPairSP.DE: m_DE++; break;
                case RegisterPairSP.HL: m_HL++; break;
                case RegisterPairSP.SP: m_SP++; break;
            }
            return 6;
        }

        private int cp_s(RegisterExtN reg)
        {
            //Register r
            //	B 000
            //	C 001
            //	D 010
            //	E 011
            //	H 100
            //	L 101
            //	A 111
            //Description: The contents of the s operand are compared with the contents of the
            //Accumulator. If there is a true compare, the Z flag is set. The execution of
            //this instruction does not affect the contents of the Accumulator.
            //Instruction	M Cycles	T States			4 MHz E.T.
            //	CP r		1			4					1.00
            //	CP n		2			7(4, 3)				1.75
            //	CP (HL)		2			7 (4, 3)			1.75
            //	CP (IX+d)	5			19 (4, 4, 3, 5, 3)	4.75
            //	CP (lY+d)	5			19 (4, 4, 3, 5, 3)	4.75
            //Condition Bits Affected:
            //	S is set if result is negative; reset otherwise
            //	Z is set if result is zero; reset otherwise
            //	H is set if borrow from bit 4; reset otherwise
            //	P/V is set if overflow; reset otherwise
            //	N is set
            //	C is set if borrow; reset otherwise
            int cycles = 4;
            switch(reg)
            {
                case RegisterExtN.B: cp((byte)(m_AF >> 8), (byte)(m_BC >> 8)); break;
                case RegisterExtN.C: cp((byte)(m_AF >> 8), (byte)(m_BC & 0x0F)); break;
                case RegisterExtN.D: cp((byte)(m_AF >> 8), (byte)(m_DE >> 8)); break;
                case RegisterExtN.E: cp((byte)(m_AF >> 8), (byte)(m_DE & 0x0F)); break;
                case RegisterExtN.H: cp((byte)(m_AF >> 8), (byte)(m_HL >> 8)); break;
                case RegisterExtN.L: cp((byte)(m_AF >> 8), (byte)(m_HL & 0x0F)); break;
                case RegisterExtN.A: cp((byte)(m_AF >> 8), (byte)(m_AF >> 8)); break;

                case RegisterExtN.N: cp((byte)(m_AF >> 8), m_bus.Read(m_programCounter)); m_programCounter++; cycles = 7; break;

                case RegisterExtN.HL: cp((byte)(m_AF >> 8), m_bus.Read(m_HL)); cycles = 7; break;

                case RegisterExtN.IXd: cp((byte)(m_AF >> 8), (byte)(m_bus.Read((byte)(m_bus.Read(m_programCounter) + m_IX)))); m_programCounter++; cycles = 19; break;
                case RegisterExtN.IYd: cp((byte)(m_AF >> 8), (byte)(m_bus.Read((byte)(m_bus.Read(m_programCounter) + m_IY)))); m_programCounter++; cycles = 19; break;
            }
            return cycles;
        }

        private void cp(byte x, byte y)
        {
            if (x > y)
            {
                m_AF |= (short)Condition.S;
            }
            else
            {
                m_AF &= (short)Condition.NS;
            }
            if (x == y)
            {
                m_AF |= (short)Condition.Z;
            }
            else
            {
                m_AF &= (short)Condition.NZ;
            }
            // TODO: H FLAG
            // TODO: PV FLAG
            if (x < y)
            {
                m_AF |= (short)Condition.C;
            }
            else
            {
                m_AF &= (short)Condition.NC;
            }
        }

        private int jp__hl__()
        {
            //Description: The Program Counter (register pair PC) is loaded with the contents of the
            //HL register pair. The next instruction is fetched from the location
            //designated by the new contents of the PC.
            //	M Cycles	T States	4 MHz E.T.
            //	1			4			1.00
            //Condition Bits Affected: None
            m_programCounter = m_HL;
            return 4;
        }

        private int jp_nn()
        {
            //Note: The first operand in this assembled object code is the low order
            //byte of a two-byte address.
            //Description: Operand nn is loaded to register pair PC (Program Counter). The next
            //instruction is fetched from the location designated by the new contents of
            //the PC.
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            m_programCounter = getNN();
            return 10;
        }

        private int ld__hl__n()
        {
            //Description: Integer n is loaded to the memory address specified by the contents of the
            //HL register pair.
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            m_bus.Write(m_HL, getN());
            return 10;
        }

        private int ld_sp_hl()
        {
            //Description: The contents of the register pair HL are loaded to the Stack Pointer (SP).
            //	M Cycles	T States	4 MHz E.T.
            //	1			6			1.5
            //Condition Bits Affected: None
            m_SP = m_HL;
            return 6;
        }

        private int push_qq(RegisterPairAF reg)
        {
            //Description: The contents of the register pair qq are pushed to the external memory
            //LIFO (last-in, first-out) Stack. The Stack Pointer (SP) register pair holds the
            //16-bit address of the current top of the Stack. This instruction first
            //decrements SP and loads the high order byte of register pair qq to the
            //memory address specified by the SP. The SP is decremented again and
            //loads the low order byte of qq to the memory location corresponding to this
            //new address in the SP. The operand qq identifies register pair BC, DE, HL,
            //or AF, assembled as follows in the object code:
            //Pair qq
            //	BC 00
            //	DE 01
            //	HL 10
            //	AF 11
            //	M Cycles	T States		4 MHz E.T.
            //	3			11 (5, 3, 3)	2.75
            //Condition Bits Affected: None
            short data = 0x0000;
            switch(reg)
            {
                case RegisterPairAF.BC: data = m_BC; break;
                case RegisterPairAF.DE: data = m_DE; break;
                case RegisterPairAF.HL: data = m_HL; break;
                case RegisterPairAF.AF: data = m_AF; break;
            }
            m_SP--;
            m_bus.Write(m_SP, (byte)(data >> 8));
            m_SP--;
            m_bus.Write(m_SP, (byte)(data & 0x00FF));
            return 11;
        }

        private int pop_qq(RegisterPairAF reg)
        {
            //Description: The top two bytes of the external memory LIFO (last-in, first-out) Stack
            //are popped to register pair qq. The Stack Pointer (SP) register pair holds
            //the 16-bit address of the current top of the Stack. This instruction first
            //loads to the low order portion of qq, the byte at memory location
            //corresponding to the contents of SP; then SP is incriminated and the
            //contents of the corresponding adjacent memory location are loaded to the
            //high order portion of qq and the SP is now incriminated again. The
            //operand qq identifies register pair BC, DE, HL, or AF, assembled as
            //follows in the object code:
            //Pair r
            //	BC 00
            //	DE 01
            //	HL 10
            //	AF 11
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            short data = 0x0000;
            data |= (short)(m_bus.Read(m_SP) & 0x00FF);
            m_SP++;
            data |= (short)(m_bus.Read(m_SP) << 8);
            m_SP++;
            switch(reg)
            {
                case RegisterPairAF.BC: m_BC = data; break;
                case RegisterPairAF.DE: m_DE = data; break;
                case RegisterPairAF.HL: m_HL = data; break;
                case RegisterPairAF.AF: m_AF = data; break;
            }
            return 10;
        }

        private int ld_i_a()
        {
            //Description: The contents of the Accumulator are loaded to the Interrupt Control Vector
            //Register, I.
            //	M Cycles	T States	MHz E.T.
            //	2			9 (4, 5)	2.25
            //Condition Bits Affected: None
            m_I = (byte)(m_AF >> 8);
            return 9;
        }

        private int im_0()
        {
            //Description: The IM 0 instruction sets interrupt mode 0. In this mode, the interrupting
            //device can insert any instruction on the data bus for execution by the
            //CPU. The first byte of a multi-byte instruction is read during the interrupt
            //acknowledge cycle. Subsequent bytes are read in by a normal memory
            //read sequence.
            //	M Cycles	T States	4 MHz E.T.
            //	2			8 (4, 4)	2.00
            //Condition Bits Affected: None
            m_interruptMode = InterruptMode.Mode_0;
            return 8;
        }

        private int im_1()
        {
            //Description: The IM 1 instruction sets interrupt mode 1. In this mode, the processor
            //responds to an interrupt by executing a restart to location 0038H.
            //	M Cycles	T States	4 MHz E.T.
            //	2			8 (4, 4)	2.00
            //Condition Bits Affected: None
            m_interruptMode = InterruptMode.Mode_1;
            return 8;
        }

        private int im_2()
        {
            //Description: The IM 2 instruction sets the vectored interrupt mode 2. This mode allows
            //an indirect call to any memory location by an 8-bit vector supplied from the
            //peripheral device. This vector then becomes the least-significant eight bits
            //of the indirect pointer, while the I register in the CPU provides the mostsignificant
            //eight bits. This address points to an address in a vector table that
            //is the starting address for the interrupt service routine.
            //	M Cycles	T States	4 MHz E.T.
            //	2			8 (4, 4)	2.00
            //Condition Bits Affected: None
            m_interruptMode = InterruptMode.Mode_2;
            return 8;
        }

        private int ld_iy_nn()
        {
            //Description: Integer nn is loaded to the Index Register IY. The first n operand after the
            //Op Code is the low order byte.
            //	M Cycles	T States		4 MHz E.T.
            //	4			14 (4, 4, 3, 3)	3.50
            //Condition Bits Affected: None
            m_IY = 0x0000;
            m_IY |= (short)(m_bus.Read(m_programCounter) & 0x00FF);
            m_programCounter++;
            m_IY |= (short)(m_bus.Read(m_programCounter) << 8);
            m_programCounter++;
            return 14;
        }

        private int ld__nn__hl()
        {
            //Description: The contents of the low order portion of register pair HL (register L) are
            //loaded to memory address (nn), and the contents of the high order portion
            //of HL (register H) are loaded to the next highest memory address (nn+1).
            //The first n operand after the Op Code is the low order byte of nn.
            //	M Cycles	T States			4 MHz E.T.
            //	5			16 (4, 3, 3, 3, 3)	4.00
            //Condition Bits Affected: None
            short nn = getNN();
            m_bus.Write(nn, (byte)(m_HL & 0x00FF));
            m_bus.Write((short)(nn+1), (byte)(m_HL >> 8));
            return 16;
        }

        private int ld_hl__nn__()
        {
            //Description: The contents of memory address (nn) are loaded to the low order portion of
            //register pair HL (register L), and the contents of the next highest memory
            //address (nn+1) are loaded to the high order portion of HL (register H). The
            //first n operand after the Op Code is the low order byte of nn.
            //	M Cycles	T States			4 MHz E.T.
            //	5			16 (4, 3, 3, 3, 3)	4.00
            //Condition Bits Affected: None
            short nn = getNN();
            m_HL = 0x0000;
            m_IY |= (short)(m_bus.Read(nn) & 0x00FF);
            m_IY |= (short)(m_bus.Read((short)(nn+1)) << 8);
            return 16;
        }

        private int ld_iyd_n()
        {
            //Description: Integer n is loaded to the memory location specified by the contents of the
            //Index Register summed with the two’s complement displacement integer d.
            //	M Cycles	T States			4 MHz E.T.
            //	5			19 (4, 4, 3, 5, 3)	2.50
            //Condition Bits Affected: None
            byte d = getN();
            byte n = getN();
            m_bus.Write((short)(m_IY + d), n);
            return 19;
        }

        private int ld_r_iyd(Register reg)
        {
            //Description: The operand (lY+d) (the contents of the Index Register IY summed with a
            //two’s complement displacement integer (d) is loaded to register r, where r
            //identifies register A, B, C, D, E, H, or L, assembled as follows in the
            //object code:
            //Register r
            //	A 111
            //	B 000
            //	C 001
            //	D 010
            //	E 011
            //	H 100
            //	L 101
            //	M Cycles	T States			4 MHz E.T.
            //	5			19 (4, 4, 3, 5, 3)	4.75
            //Condition Bits Affected: None
            byte d = getN();
            byte data = m_bus.Read((short)(m_IY + d));
            switch(reg)
            {
                case Register.B: m_BC &= (short)(data << 8 | 0x00FF); break;
                case Register.C: m_BC &= (short)(data | 0xFF00); break;
                case Register.D: m_DE &= (short)(data << 8 | 0x00FF); break;
                case Register.E: m_DE &= (short)(data | 0xFF00); break;
                case Register.H: m_HL &= (short)(data << 8 | 0x00FF); break;
                case Register.L: m_HL &= (short)(data | 0xFF00); break;
                case Register.A: m_AF &= (short)(data << 8 | 0x00FF); break;
            }
            return 19;
        }

        private int call_nn()
        {
            //Description: The current contents of the Program Counter (PC) are pushed onto the top
            //of the external memory stack. The operands nn are then loaded to the PC to
            //point to the address in memory where the first Op Code of a subroutine is to
            //be fetched. At the end of the subroutine, a RETurn instruction can be used
            //to return to the original program flow by popping the top of the stack back
            //to the PC. The push is accomplished by first decrementing the current
            //contents of the Stack Pointer (register pair SP), loading the high-order byte
            //of the PC contents to the memory address now pointed to by the SP; then
            //decrementing SP again, and loading the low order byte of the PC contents
            //to the top of stack.
            //Because this is a 3-byte instruction, the Program Counter was incremented
            //by three before the push is executed.
            //	M Cycles	T States			4 MHz E.T.
            //	5			17 (4, 3, 4, 3, 3)	4.25
            //Condition Bits Affected: None
            short nn = getNN();
            m_SP--;
            m_bus.Write(m_SP, (byte)(m_programCounter >> 8));
            m_SP--;
            m_bus.Write(m_SP, (byte)(m_programCounter & 0x00FF));
            m_programCounter = nn;
            return 17;
        }

        private int ld__nn__dd(RegisterPairSP reg)
        {
            //Description: The low order byte of register pair dd is loaded to memory address (nn); the
            //upper byte is loaded to memory address (nn+1). Register pair dd defines
            //either BC, DE, HL, or SP, assembled as follows in the object code:
            //Pair dd
            //	BC 00
            //	DE 01
            //	HL 10
            //	SP 11
            //The first n operand after the Op Code is the low order byte of a two byte
            //memory address.
            //	M Cycles	T States				4 MHz E.T.
            //	6			20 (4, 4, 3, 3, 3, 3)	5.00
            //Condition Bits Affected: None
            short nn = getNN();
            short data = 0x0000;
            switch (reg)
            {
                case RegisterPairSP.BC: data = m_BC; break;
                case RegisterPairSP.DE: data = m_DE; break;
                case RegisterPairSP.HL: data = m_HL; break;
                case RegisterPairSP.SP: data = m_SP; break;
            }
            m_bus.Write(nn, (byte)(data & 0x00FF));
            m_bus.Write((short)(nn+1), (byte)(data >> 8));
            return 20;
        }

        private int ret()
        {
            //Description: The byte at the memory location specified by the contents of the Stack
            //Pointer (SP) register pair is moved to the low order eight bits of the
            //Program Counter (PC). The SP is now incremented and the byte at the
            //memory location specified by the new contents of this instruction is fetched
            //from the memory location specified by the PC. This instruction is normally
            //used to return to the main line program at the completion of a routine
            //entered by a CALL instruction.
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None	
            m_programCounter = 0x0000;
            m_programCounter |= (short)(m_SP & 0x00FF);
            m_SP--;
            m_programCounter |= (short)(m_SP << 8);
            m_SP--;
            return 10;
        }

        private int ret_cc(Condition flg)
        {
            //Description: If condition cc is true, the byte at the memory location specified by the
            //contents of the Stack Pointer (SP) register pair is moved to the low order
            //eight bits of the Program Counter (PC). The SP is incremented and the byte
            //at the memory location specified by the new contents of the SP are moved
            //to the high order eight bits of the PC. The SP is incremented again. The
            //next Op Code following this instruction is fetched from the memory
            //location specified by the PC. This instruction is normally used to return to
            //the main line program at the completion of a routine entered by a CALL
            //instruction. If condition cc is false, the PC is simply incremented as usual,
            //and the program continues with the next sequential instruction. Condition
            //cc is programmed as one of eight status that correspond to condition bits in
            //the Flag Register (register F). These eight status are defined in the table
            //below, which also specifies the corresponding cc bit fields in the assembled
            //object code.
            //Relevant
            //	cc Condition Flag
            //	000 NZ non zero Z
            //	001 Z zero Z
            //	010 NC non carry C
            //	011 C carry C
            //	100 PO parity odd P/V
            //	101 PE parity even P/V
            //	110 P sign positive S
            //	111 M sign negative S
            //If cc is true:
            //	M Cycles	T States		4 MHz E.T.
            //	3			11 (5, 3, 3)	2.75
            //If cc is false:
            //	M Cycles	T States		4 MHz E.T.
            //	1			5				1.25
            //Condition Bits Affected: None
            int cycles = 5;
            if (check_condition(flg))
            {
                m_programCounter = 0x0000;
                m_programCounter |= (short)(m_SP & 0x00FF);
                m_SP--;
                m_programCounter |= (short)(m_SP << 8);
                m_SP--;
                cycles = 11;
            }
            return cycles;
        }

        private bool check_condition(Condition flg)
        {
            bool ret = false;
            switch (flg)
            {
                case Condition.Z: ret = (short)(m_AF & (short)Condition.Z) == (short)Condition.Z; break;
                case Condition.NZ: ret = (short)(m_AF | (short)Condition.NZ) == (short)Condition.NZ; break;
                case Condition.C: ret = (short)(m_AF & (short)Condition.C) == (short)Condition.C; break;
                case Condition.NC: ret = (short)(m_AF | (short)Condition.NC) == (short)Condition.NC; break;
                case Condition.PE: ret = (short)(m_AF & (short)Condition.PE) == (short)Condition.PE; break;
                case Condition.PO: ret = (short)(m_AF | (short)Condition.PO) == (short)Condition.PO; break;
                case Condition.S: ret = (short)(m_AF & (short)Condition.S) == (short)Condition.S; break;
                case Condition.NS: ret = (short)(m_AF | (short)Condition.NS) == (short)Condition.NS; break;
            }
            return ret;
        }

        private int halt()
        {
            //Description: The HALT instruction suspends CPU operation until a subsequent interrupt
            //or reset is received. While in the HALT state, the processor executes NOPs
            //to maintain memory refresh logic.
            //	M Cycles	T States	4 MHz E.T.
            //	1			4			1.00
            //Condition Bits Affected: None
            m_halted = true;
            return 4;
        }

        private int ei()
        {
            //Description: The enable interrupt instruction sets both interrupt enable flip flops (IFFI
            //and IFF2) to a logic 1, allowing recognition of any maskable interrupt. Note
            //that during the execution of this instruction and the following instruction,
            //maskable interrupts are disabled.
            //	M Cycles	T States	4 MHz E.T.
            //	1			4			1.00
            //Condition Bits Affected: None
            m_IFF1 = true;
            m_IFF2 = true;
            return 4;
        }
    }
}
