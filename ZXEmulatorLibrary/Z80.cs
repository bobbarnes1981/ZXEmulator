using System;
using System.Collections.Generic;
using System.Globalization;

namespace ZXEmulatorLibrary
{
    public class Z80
    {
        private const string UNKNOWN_OPCODE = "Unknown Opcode";

        private Dictionary<byte, string> m_opCodes = new Dictionary<byte, string>
        {
            {0x00, "nop"},
            {0x01, "ld_dd_nn BC"},

            {0x03, "inc_ss BC"},

            {0x05, "dec_r B"},
            {0x06, "ld_r_n B"},
            {0x07, "rlca"},

            {0x0B, "dec_ss BC"},

            {0x0D, "dec_r C"},
            {0x0E, "ld_r_n C"},
            {0x0F, "rrca"},

            {0x11, "ld_dd_nn DE"},

            {0x13, "inc_ss DE"},

            {0x15, "dec_r D"},
            {0x16, "ld_r_n D"},

            {0x18, "jr_e"},

            {0x1B, "dec_ss DE"},

            {0x1D, "dec_r E"},
            {0x1E, "ld_r_n E"},

            {0x19, "add_hl_ss DE"},

            {0x20, "jr_nz_e"},
            {0x21, "ld_dd_nn HL"},
            {0x22, "ld__nn__hl"},

            {0x23, "inc_ss HL"},

            {0x25, "dec_r H"},
            {0x26, "ld_r_n H"},

            {0x28, "jr_z_e"},

            {0x2A, "ld_hl__nn__"},
            {0x2B, "dec_ss HL"},

            {0x2D, "dec_r L"},
            {0x2E, "ld_r_n L"},

            {0x30, "jr_nc_e"},
            {0x31, "ld_dd_nn SP"},

            {0x33, "inc_ss SP"},

            {0x35, "dec_m HL"},
            {0x36, "ld__hl__n"},
            {0x37, "scf"},

            {0x38, "jr_c_e"},

            {0x3B, "dec_ss SP"},
            {0x3C, "inc_r A"},

            {0x3D, "dec_r A"},
            {0x3E, "ld_r_n A"},

            {0x44, "ld_r_r B H"},

            {0x47, "ld_r_r B A"},

            {0x4D, "ld_r_r C L"},

            {0x54, "ld_r_r D H"},

            {0x56, "ld_r_hl D"},

            {0x5D, "ld_r_r E L"},
            {0x5E, "ld_r_hl E"},

            {0x60, "ld_r_r H B"},

            {0x69, "ld_r_r L C"},

            {0x76, "halt"},

            {0x78, "ld_r_r A B"},

            {0x7B, "ld_r_r A E"},

            {0x7E, "ld_r_hl A"},

            {0x87, "add_a_r A"},

            {0xA7, "and_r A"},

            {0xAF, "xor_r A"},

            {0xB8, "cp_s B"},
            {0xB9, "cp_s C"},
            {0xBA, "cp_s D"},
            {0xBB, "cp_s E"},
            {0xBC, "cp_s H"},
            {0xBD, "cp_s L"},
            {0xBE, "cp_hl"},
            {0xBF, "cp_s A"},

            {0xC0, "ret_cc NZ"},
            {0xC1, "pop_qq BC"},
            {0xC2, "jp_cc_nn NZ"},
            {0xC3, "jp_nn"},

            {0xC5, "push_qq BC"},

            {0xC8, "ret_cc Z"},
            {0xC9, "ret"},
            {0xCA, "jp_cc_nn Z"},
            {0xCB, "CB"},

            {0xCD, "call_nn"},

            {0xD0, "ret_cc NC"},
            {0xD1, "pop_qq DE"},
            {0xD2, "jp_cc_nn C"},
            {0xD3, "out__n__a"},

            {0xD5, "push_qq DE"},

            {0xD8, "ret_cc C"},
            {0xD9, "exx"},

            {0xDA, "jp_cc_nn C"},

            {0xDC, "call_cc_nn C"},
            {0xDD, "DD"},

            {0xE0, "ret_cc PO"},
            {0xE1, "pop_qq HL"},
            {0xE2, "jp_cc_nn PO"},
            {0xE5, "push_qq HL"},

            {0xE8, "ret_cc PE"},
            {0xE9, "jp__hl__"},
            {0xEA, "jp_cc_nn PE"},
            {0xEB, "ex_de_hl"},

            {0xED, "ED"},

            {0xF0, "ret_cc NS"},
            {0xF1, "pop_qq AF"},
            {0xF2, "jp_cc_nn NS"},

            {0xF5, "push_qq AF"},

            {0xF8, "ret_cc S"},
            {0xF9, "ld_sp_hl"},
            {0xFA, "jp_cc_nn S"},
            {0xFB, "ei"},

            {0xFD, "FD"},
            {0xFE, "cp_n"},
        };

        private Dictionary<byte, string> m_opCodesCB = new Dictionary<byte, string>
        {
            {0x13, "rl_r E"},
        };

        private Dictionary<byte, string> m_opCodesDD = new Dictionary<byte, string>
        {
            {0xBE, "cp_ixd"},
        }; 

        private Dictionary<byte, string> m_opCodesED = new Dictionary<byte, string>
        {
            {0x42, "sbc_hl_ss BC"},
            {0x43, "ld__nn__dd BC"},

            {0x46, "im_0"},
            {0x47, "ld_i_a"},

            {0x4B, "ld_dd__nn__ BC"},

            {0x4F, "ld_r_a"},

            {0x52, "sbc_hl_ss DE"},
            {0x53, "ld__nn__dd DE"},

            {0x56, "im_1"},

            {0x5B, "ld_dd__nn__ DE"},

            {0x5E, "im_2"},

            {0x63, "ld__nn__dd HL"},

            {0x73, "ld__nn__dd SP"},

            {0xB1, "cpir"},
        };

        private Dictionary<byte, string> m_opCodesFD = new Dictionary<byte, string>
        {
            {0x21, "ld_iy_nn"},

            {0x36, "ld_iyd_n"},

            {0x96, "sub_iyd"},

            {0xBE, "cp_iyd"},
        }; 

        private bool m_IFF1 = false;
        private bool m_IFF2 = false;

        private RegisterPair m_PC = new RegisterPair();
        private byte m_instructionRegister = 0x00;
        
        private bool m_halted = false;
        
        private InterruptMode m_interruptMode = InterruptMode.None;

        private RegisterPair m_AF = new RegisterPair();
        private RegisterPair m_BC = new RegisterPair();
        private RegisterPair m__BC = new RegisterPair();
        private RegisterPair m_DE = new RegisterPair();
        private RegisterPair m__DE = new RegisterPair();
        private RegisterPair m_HL = new RegisterPair();
        private RegisterPair m__HL = new RegisterPair();
        private RegisterPair m_SP = new RegisterPair();

        private short m_IX;
        private short m_IY;

        private byte m_I; // TODO: make external accessible for use during interrupt for addressing
        private byte m_R;

        private IBus m_bus;


        public Z80(IBus bus)
        {
            m_bus = bus;
        }


        public uint Step(bool interrupt, bool nmi)
        {
            // when is IFF2 reset?
            if (nmi)
            {
                m_IFF1 = false;
                // jump to 0x0066
                throw new NotImplementedException();
            }
            else if(interrupt && m_IFF1)
            {
                switch (m_interruptMode)
                {
                    case InterruptMode.Mode_0: throw new NotImplementedException("Interrupt mode 0 not implemented"); break;
                    case InterruptMode.Mode_1: m_IFF1 = false; rst_p(ResetLocation.Addr38h); m_halted = false; break;
                    case InterruptMode.Mode_2: throw new NotImplementedException("Interrupt Mode 2 not implemented"); break;
                }
            }

            if (m_halted)
            {
                return nop();
            }

            return executeNextOpcode();
        }

        public uint Reset()
        {
            m_IFF1 = false;
            m_IFF2 = false;
            m_interruptMode = InterruptMode.Mode_0;
            m_PC.Register = 0x0000;
            m_I = 0x00;
            m_R = 0x00;
            m_SP.Register = 0xFFFF;
            m_AF.Register = 0xFFFF;
            return 3;
        }

        private uint executeNextOpcode()
        {
            m_instructionRegister = m_bus.Read(m_PC.Register);
            Console.WriteLine("0x{0:x4} 0x{1:x2} {2}", m_PC.Register, m_instructionRegister, m_opCodes.ContainsKey(m_instructionRegister) ? m_opCodes[m_instructionRegister] : UNKNOWN_OPCODE);
            m_PC.Register++;
            return executeOpcode();
        }


        private uint executeOpcode()
        {
            incrementR();
            uint cycles = 0;
            switch(m_instructionRegister)
            {
                case 0x00: cycles = nop(); break;
                case 0x01: cycles = ld_dd_nn(RegisterPairSP.BC); break;

                case 0x03: cycles = inc_ss(RegisterPairSP.BC); break;

                case 0x05: cycles = dec_r(Register.B); break;
                case 0x06: cycles = ld_r_n(Register.B); break;
                case 0x07: cycles = rlca(); break;

                case 0x09: cycles = add_hl_ss(RegisterPairSP.BC); break;

                case 0x0B: cycles = dec_ss(RegisterPairSP.BC); break;

                case 0x0D: cycles = dec_r(Register.C); break;
                case 0x0E: cycles = ld_r_n(Register.C); break;
                case 0x0F: cycles = rrca(); break;

                case 0x11: cycles = ld_dd_nn(RegisterPairSP.DE); break;

                case 0x13: cycles = inc_ss(RegisterPairSP.DE); break;

                case 0x15: cycles = dec_r(Register.D); break;
                case 0x16: cycles = ld_r_n(Register.D); break;

                case 0x18: cycles = jr_e(); break;

                case 0x1B: cycles = dec_ss(RegisterPairSP.DE); break;

                case 0x1D: cycles = dec_r(Register.E); break;
                case 0x1E: cycles = ld_r_n(Register.E); break;

                case 0x19: cycles = add_hl_ss(RegisterPairSP.DE); break;

                case 0x20: cycles = jr_nz_e(); break;
                case 0x21: cycles = ld_dd_nn(RegisterPairSP.HL); break;
                case 0x22: cycles = ld__nn__hl(); break;

                case 0x23: cycles = inc_ss(RegisterPairSP.HL); break;

                case 0x25: cycles = dec_r(Register.H); break;
                case 0x26: cycles = ld_r_n(Register.H); break;

                case 0x28: cycles = jr_z_e(); break;

                case 0x2A: cycles = ld_hl__nn__(); break;
                case 0x2B: cycles = dec_ss(RegisterPairSP.HL); break;

                case 0x2D: cycles = dec_r(Register.L); break;
                case 0x2E: cycles = ld_r_n(Register.L); break;

                case 0x30: cycles = jr_nc_e(); break;
                case 0x31: cycles = ld_dd_nn(RegisterPairSP.SP); break;

                case 0x33: cycles = inc_ss(RegisterPairSP.SP); break;

                case 0x35: cycles = dec_hl(); break;
                case 0x36: cycles = ld__hl__n(); break;
                case 0x37: cycles = scf(); break;

                case 0x38: cycles = jr_c_e(); break;

                case 0x3B: cycles = dec_ss(RegisterPairSP.SP); break;
                case 0x3C: cycles = inc_r(Register.A); break;

                case 0x3D: cycles = dec_r(Register.A); break;
                case 0x3E: cycles = ld_r_n(Register.A); break;

                case 0x44: cycles = ld_r_r(Register.B, Register.H); break;

                case 0x47: cycles = ld_r_r(Register.B, Register.A); break;

                case 0x4D: cycles = ld_r_r(Register.C, Register.L); break;

                case 0x54: cycles = ld_r_r(Register.D, Register.H); break;

                case 0x56: cycles = ld_r_hl(Register.D); break;

                case 0x5D: cycles = ld_r_r(Register.E, Register.L); break;
                case 0x5E: cycles = ld_r_hl(Register.E); break;

                case 0x60: cycles = ld_r_r(Register.H, Register.B); break;

                case 0x69: cycles = ld_r_r(Register.L, Register.C); break;

                case 0x76: cycles = halt(); break;

                case 0x78: cycles = ld_r_r(Register.A, Register.B); break;

                case 0x7B: cycles = ld_r_r(Register.A, Register.E); break;

                case 0x7E: cycles = ld_r_hl(Register.A); break;

                case 0x87: cycles = add_a_r(Register.A); break;

                case 0xA7: cycles = and_r(Register.A); break;

                case 0xAF: cycles = xor_r(Register.A); break;

                case 0xB8: cycles = cp_r(Register.B); break;
                case 0xB9: cycles = cp_r(Register.C); break;
                case 0xBA: cycles = cp_r(Register.D); break;
                case 0xBB: cycles = cp_r(Register.E); break;
                case 0xBC: cycles = cp_r(Register.H); break;
                case 0xBD: cycles = cp_r(Register.L); break;
                case 0xBE: cycles = cp_hl(); break;
                case 0xBF: cycles = cp_r(Register.A); break;

                case 0xC0: cycles = ret_cc(Condition.NZ); break;
                case 0xC1: cycles = pop_qq(RegisterPairAF.BC); break;
                case 0xC2: cycles = jp_cc_nn(Condition.NZ); break;
                case 0xC3: cycles = jp_nn(); break;

                case 0xC5: cycles = push_qq(RegisterPairAF.BC); break;

                case 0xC8: cycles = ret_cc(Condition.Z); break;
                case 0xC9: cycles = ret(); break;
                case 0xCA: cycles = jp_cc_nn(Condition.Z); break;
                case 0xCB: cycles = executeOpcodeCB(); break;

                case 0xCD: cycles = call_nn(); break;

                case 0xD0: cycles = ret_cc(Condition.NC); break;
                case 0xD1: cycles = pop_qq(RegisterPairAF.DE); break;
                case 0xD2: cycles = jp_cc_nn(Condition.C); break;
                case 0xD3: cycles = out__n__a();break;

                case 0xD5: cycles = push_qq(RegisterPairAF.DE); break;

                case 0xD8: cycles = ret_cc(Condition.C); break;
                case 0xD9: cycles = exx(); break;

                case 0xDA: cycles = jp_cc_nn(Condition.C); break;

                case 0xDC: cycles = call_cc_nn(Condition.C); break;
                case 0xDD: cycles = executeOpcodeDD(); break;

                case 0xE0: cycles = ret_cc(Condition.PO); break;
                case 0xE1: cycles = pop_qq(RegisterPairAF.HL); break;
                case 0xE2: cycles = jp_cc_nn(Condition.PO); break;
                case 0xE5: cycles = push_qq(RegisterPairAF.HL); break;

                case 0xE8: cycles = ret_cc(Condition.PE); break;
                case 0xE9: cycles = jp__hl__(); break;
                case 0xEA: cycles = jp_cc_nn(Condition.PE); break;

                case 0xEB: cycles = ex_de_hl(); break;

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
                case 0xFE: cycles = cp_n(); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0x{0:x2} @ 0x{1:x4}", m_instructionRegister, m_PC.Register - 1));
            }
            return cycles;
        }

        private uint executeOpcodeCB()
        {
            incrementR();
            uint cycles = 0;
            byte opcode = m_bus.Read(m_PC.Register);
            Console.WriteLine("0x{0:x4} 0x{1:x2} {2}", m_PC.Register, opcode, m_opCodesCB.ContainsKey(opcode) ? m_opCodesCB[opcode] : UNKNOWN_OPCODE);
            m_PC.Register++;
            switch (opcode)
            {
                case 0x13: cycles = rl_r(Register.E); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xCB 0x{0:x2} @ 0x{1:x4}", opcode, m_PC.Register - 2));
            }

            return cycles;
        }

        private uint executeOpcodeDD()
        {
            incrementR();
            uint cycles = 0;
            byte opcode = m_bus.Read(m_PC.Register);
            Console.WriteLine("0x{0:x4} 0x{1:x2} {2}", m_PC.Register, opcode, m_opCodesDD.ContainsKey(opcode) ? m_opCodesDD[opcode] : UNKNOWN_OPCODE);
            m_PC.Register++;
            switch (opcode)
            {
                case 0x35: cycles = dec_ixd(); break;

                case 0xBE: cycles = cp_ixd(); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xDD 0x{0:x2} @ 0x{1:x4}", opcode, m_PC.Register - 2));
            }

            return cycles;
        }

        private uint executeOpcodeED()
        {
            incrementR();
            uint cycles = 0;
            byte opcode = m_bus.Read(m_PC.Register);
            Console.WriteLine("0x{0:x4} 0x{1:x2} {2}", m_PC.Register, opcode, m_opCodesED.ContainsKey(opcode) ? m_opCodesED[opcode] : UNKNOWN_OPCODE);
            m_PC.Register++;
            switch (opcode)
            {
                case 0x42: cycles = sbc_hl_ss(RegisterPairSP.BC); break;
                case 0x43: cycles = ld__nn__dd(RegisterPairSP.BC); break;

                case 0x46: cycles = im_0(); break;
                case 0x47: cycles = ld_i_a(); break;

                case 0x4B: cycles = ld_dd__nn__(RegisterPairSP.BC); break;

                case 0x4F: cycles = ld_r_a(); break;

                case 0x52: cycles = sbc_hl_ss(RegisterPairSP.DE); break;
                case 0x53: cycles = ld__nn__dd(RegisterPairSP.DE); break;

                case 0x56: cycles = im_1(); break;

                case 0x5B: cycles = ld_dd__nn__(RegisterPairSP.DE); break;

                case 0x5E: cycles = im_2(); break;

                case 0x63: cycles = ld__nn__dd(RegisterPairSP.HL); break;

                case 0x73: cycles = ld__nn__dd(RegisterPairSP.SP); break;

                case 0xB1: cycles = cpir(); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xED 0x{0:x2} @ 0x{1:x4}", opcode, m_PC.Register - 2));
            }
            return cycles;
        }

        private uint executeOpcodeFD()
        {
            incrementR();
            uint cycles = 0;
            byte opcode = m_bus.Read(m_PC.Register);
            Console.WriteLine("0x{0:x4} 0x{1:x2} {2}", m_PC.Register, opcode, m_opCodesFD.ContainsKey(opcode) ? m_opCodesFD[opcode] : UNKNOWN_OPCODE);
            m_PC.Register++;
            switch (opcode)
            {
                case 0x21: cycles = ld_iy_nn(); break;

                case 0x35: cycles = dec_iyd(); break;
                case 0x36: cycles = ld_iyd_n(); break;

                case 0x96: cycles = sub_iyd(); break;

                case 0xBE: cycles = cp_iyd(); break;

                default:
                    throw new Exception(string.Format("Unhandled Opcode: 0xFD 0x{0:x2} @ 0x{1:x4}", opcode, m_PC.Register - 2));
            }
            return cycles;
        }


        private uint nop()
        {
            //Description: The CPU performs no operation during this machine cycle.
            //	M Cycles	T States	4 MHz E.T.
            //	1			4			1.00
            //Condition Bits Affected: None
            return 4;
        }

        private uint out__n__a()
        {
            //Description: The operand n is placed on the bottom half (A0 through A7) of the address
            //bus to select the I/O device at one of 256 possible ports. The contents of the
            //Accumulator (register A) also appear on the top half (A8 through A15) of
            //the address bus at this time. Then the byte contained in the Accumulator is
            //placed on the data bus and written to the selected peripheral device.
            //	M Cycles	T States		4 MHz E.T.
            //	3			11 (4, 3, 4)	2.75
            //Condition Bits Affected: None
            byte n = getN();
            ushort address = n;
            address |= (ushort)(m_AF.Hi << 8);
            m_bus.Write(address, m_AF.Hi, false);
            return 11;
        }

        private uint ld_dd_nn(RegisterPairSP reg)
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
            ushort nn = getNN();
            switch(reg)
            {
                case RegisterPairSP.BC: m_BC.Register = nn; break;
                case RegisterPairSP.DE: m_DE.Register = nn; break;
                case RegisterPairSP.HL: m_HL.Register = nn; break;
                case RegisterPairSP.SP: m_SP.Register = nn; break;
            }
            return 10;
        }

        private uint ld_r_r(Register reg1, Register reg2)
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
                case Register.B: data = m_BC.Hi; break;
                case Register.C: data = m_BC.Lo; break;
                case Register.D: data = m_DE.Hi; break;
                case Register.E: data = m_DE.Lo; break;
                case Register.H: data = m_HL.Hi; break;
                case Register.L: data = m_HL.Lo; break;
                case Register.A: data = m_AF.Hi; break;
            }
            switch (reg2)
            {
                case Register.B: m_BC.Hi = data; break;
                case Register.C: m_BC.Lo = data; break;
                case Register.D: m_DE.Hi = data; break;
                case Register.E: m_DE.Lo = data; break;
                case Register.H: m_HL.Hi = data; break;
                case Register.L: m_HL.Lo = data; break;
                case Register.A: m_AF.Hi = data; break;
            }
            return 4;
        }

        private uint ld_r_a()
        {
            //Description: The contents of the Accumulator are loaded to the Memory Refresh
            //register R.
            //	M Cycles	T States	MHz E.T.
            //	2			9 (4, 5)	2.25
            //Condition Bits Affected: None
            m_R = m_AF.Hi;
            return 9;
        }

        /// <summary>
        /// The 8-bit contents of memory location (HL) are loaded to register r, in which r identifies
        /// registers A, B, C, D, E, H, or L, assembled as follows in the object code:
        /// Register r
        /// A 111
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// M Cycles    T States    4 MHz E.T.
        /// 2           7 (4, 3)    1.75
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private uint ld_r_hl(Register reg)
        {
            byte data = m_bus.Read(m_HL.Register);
            switch (reg)
            {
                case Register.A: m_AF.Hi = data; break;
                case Register.B: m_BC.Hi = data; break;
                case Register.C: m_BC.Lo = data; break;
                case Register.D: m_DE.Hi = data; break;
                case Register.E: m_DE.Lo = data; break;
                case Register.H: m_HL.Hi = data; break;
                case Register.L: m_HL.Lo = data; break;
            }
            return 2;
        }

        private uint jr_e()
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
            sbyte e = (sbyte)getN();
            m_PC.Register = (ushort)(m_PC.Register + e);
            return 12;
        }

        private uint jr_nz_e()
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
            uint cycles = 7;
            sbyte e = (sbyte)getN();
            if (check_condition(Condition.NZ))
            {
                m_PC.Register = (ushort)(m_PC.Register + e);
                cycles = 12;
            }
            return cycles;
        }

        /// <summary>
        /// This instruction provides for conditional branching to other segments of a
        /// program depending on the results of a test on the Zero Flag. If the flag is
        /// equal to a 1, the value of the displacement e is added to the Program
        /// Counter (PC) and the next instruction is fetched from the location
        /// designated by the new contents of the PC. The jump is measured from the
        /// address of the instruction Op Code and has a range of -126 to +129 bytes.
        /// The assembler automatically adjusts for the twice incremented PC.
        /// If the Zero Flag is equal to a 0, the next instruction executed is taken from
        /// the location following this instruction. If the condition is met:
        /// M Cycles    T States        4 MHz E.T.
        /// 3           12 (4, 3, 5)    3.00
        /// If the condition is not met;
        /// M Cycles    T States        4 MHz E.T.
        /// 2           7 (4, 3)        1.75
        /// Condition Bits Affected: None
        /// </summary>
        /// <returns></returns>
        private uint jr_z_e()
        {
            uint cycles = 7;
            sbyte e = (sbyte)getN();
            if (check_condition(Condition.Z))
            {
                m_PC.Register = (ushort)(m_PC.Register + e);
                cycles = 12;
            }
            return cycles;
        }

        /// <summary>
        /// This instruction provides for conditional branching to other segments of a program
        /// depending on the results of a test on the Carry Flag. If the flag is equal to 0, the value of
        /// displacement e is added to the Program Counter (PC) and the next instruction is fetched
        /// from the location designated by the new contents of the PC. The jump is measured from
        /// the address of the instruction op code and contains a range of –126 to +129 bytes. The
        /// assembler automatically adjusts for the twice incremented PC.
        /// If the flag = 1, the next instruction executed is taken from the location following this
        /// instruction.
        /// If the condition is met:
        /// M Cycles    T States        4 MHz E.T.
        /// 3           12 (4, 3, 5)    3.00
        /// If the condition is not met:
        /// M Cycles    T States        4 MHz E.T.
        /// 7           7 (4, 3)        1.75
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <returns></returns>
        private uint jr_nc_e()
        {
            uint cycles = 7;
            sbyte e = (sbyte)getN();
            if (check_condition(Condition.NC))
            {
                m_PC.Register = (ushort)(m_PC.Register + e);
                cycles = 12;
            }
            return cycles;
        }

        /// <summary>
        /// This instruction provides for conditional branching to other segments of a program
        /// depending on the results of a test on the Carry Flag. If the flag = 1, the value of displacement
        /// e is added to the Program Counter (PC) and the next instruction is fetched from the
        /// location designated by the new contents of the PC. The jump is measured from the address
        /// of the instruction op code and contains a range of –126 to +129 bytes. The assembler automatically
        /// adjusts for the twice incremented PC.
        /// If the flag = 0, the next instruction executed is taken from the location following this
        /// instruction. If condition is met
        /// M Cycles    T States        4 MHz E.T.
        /// 3           12 (4, 3, 5)    3.00
        /// If condition is not met:
        /// M Cycles    T States    4 MHz E.T.
        /// 2           7 (4, 3)    1.75
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <returns></returns>
        private uint jr_c_e()
        {
            uint cycles = 7;
            sbyte e = (sbyte)getN();
            if (check_condition(Condition.C))
            {
                m_PC.Register = (ushort)(m_PC.Register + e);
                cycles = 12;
            }
            return cycles;
        }

        private uint jp_cc_nn(Condition flg)
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
            ushort nn = getNN();
            if (check_condition(flg))
            {
                m_PC.Register = nn;
            }
            return 10;
        }

        /// <summary>
        /// Register r
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// A 111
        /// The byte specified by the m operand is decremented.
        /// Instruction	M Cycles	T States				4 MHz E.T.
        /// DEC r		1 			4						1.00
        /// DEC (HL)	3 			11 (4, 4, 3				2.75
        /// DEC (IX+d)	6 			23 (4, 4, 3, 5, 4, 3)	5.75
        /// DEC (lY+d)	6			23 (4, 4, 3, 5, 4, 3)	5.75
        /// Condition Bits Affected:
        /// S is set if result is negative; reset otherwise
        /// Z is set if result is zero; reset otherwise
        /// H is set if borrow from bit 4, reset otherwise
        /// P/V is set if m was 80H before operation; reset otherwise
        /// N is set
        /// C is not affected
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private uint dec_r(Register r)
        {
            switch(r)
            {
                case Register.A: m_AF.Hi = dec(m_AF.Hi); break;
                case Register.B: m_BC.Hi = dec(m_BC.Hi); break;
                case Register.C: m_BC.Lo = dec(m_BC.Lo); break;
                case Register.D: m_DE.Hi = dec(m_DE.Hi); break;
                case Register.E: m_DE.Lo = dec(m_DE.Lo); break;
                case Register.H: m_HL.Hi = dec(m_HL.Hi); break;
                case Register.L: m_HL.Lo = dec(m_HL.Lo); break;
            }
            return 4;
        }

        /// <summary>
        /// The byte specified by the m operand is decremented.
        /// Instruction	M Cycles	T States				4 MHz E.T.
        /// DEC r		1 			4						1.00
        /// DEC (HL)	3 			11 (4, 4, 3				2.75
        /// DEC (IX+d)	6 			23 (4, 4, 3, 5, 4, 3)	5.75
        /// DEC (lY+d)	6			23 (4, 4, 3, 5, 4, 3)	5.75
        /// Condition Bits Affected:
        /// S is set if result is negative; reset otherwise
        /// Z is set if result is zero; reset otherwise
        /// H is set if borrow from bit 4, reset otherwise
        /// P/V is set if m was 80H before operation; reset otherwise
        /// N is set
        /// C is not affected
        /// </summary>
        /// <returns></returns>
        private uint dec_hl()
        {
            m_bus.Write(m_HL.Register, dec(m_bus.Read(m_HL.Register)));
            return 11;
        }

        /// <summary>
        /// The byte specified by the m operand is decremented.
        /// Instruction	M Cycles	T States				4 MHz E.T.
        /// DEC r		1 			4						1.00
        /// DEC (HL)	3 			11 (4, 4, 3				2.75
        /// DEC (IX+d)	6 			23 (4, 4, 3, 5, 4, 3)	5.75
        /// DEC (lY+d)	6			23 (4, 4, 3, 5, 4, 3)	5.75
        /// Condition Bits Affected:
        /// S is set if result is negative; reset otherwise
        /// Z is set if result is zero; reset otherwise
        /// H is set if borrow from bit 4, reset otherwise
        /// P/V is set if m was 80H before operation; reset otherwise
        /// N is set
        /// C is not affected
        /// </summary>
        /// <returns></returns>
        private uint dec_ixd()
        {
            sbyte d = (sbyte)m_bus.Read(m_PC.Register);
            m_PC.Register++;
            ushort memAddr = (ushort) (m_IX + d);
            byte memData = m_bus.Read(memAddr);
            m_bus.Write(memAddr, dec(memData));
            return 23;
        }

        /// <summary>
        /// The byte specified by the m operand is decremented.
        /// Instruction	M Cycles	T States				4 MHz E.T.
        /// DEC r		1 			4						1.00
        /// DEC (HL)	3 			11 (4, 4, 3				2.75
        /// DEC (IX+d)	6 			23 (4, 4, 3, 5, 4, 3)	5.75
        /// DEC (lY+d)	6			23 (4, 4, 3, 5, 4, 3)	5.75
        /// Condition Bits Affected:
        /// S is set if result is negative; reset otherwise
        /// Z is set if result is zero; reset otherwise
        /// H is set if borrow from bit 4, reset otherwise
        /// P/V is set if m was 80H before operation; reset otherwise
        /// N is set
        /// C is not affected
        /// </summary>
        /// <returns></returns>
        private uint dec_iyd()
        {
            sbyte d = (sbyte)m_bus.Read(m_PC.Register);
            m_PC.Register++;
            ushort memAddr = (ushort)(m_IY + d);
            byte memData = m_bus.Read(memAddr);
            m_bus.Write(memAddr, dec(memData));
            return 23;
        }

        private uint dec_ss(RegisterPairSP ss)
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
            switch(ss)
            {
                case RegisterPairSP.BC: m_BC.Register--; break;
                case RegisterPairSP.DE: m_DE.Register--; break;
                case RegisterPairSP.HL: m_HL.Register--; break;
                case RegisterPairSP.SP: m_SP.Register--; break;
            }
            return 6;
        }

        private uint ld_r_n(Register r)
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
            switch(r)
            {
                case Register.A: m_AF.Hi = n; break;
                case Register.B: m_BC.Hi = n; break;
                case Register.C: m_BC.Lo = n; break;
                case Register.D: m_DE.Hi = n; break;
                case Register.E: m_DE.Lo = n; break;
                case Register.H: m_HL.Hi = n; break;
                case Register.L: m_HL.Lo = n; break;
            }
            return 7;
        }

        private uint inc_ss(RegisterPairSP ss)
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
            switch(ss)
            {
                case RegisterPairSP.BC: m_BC.Register++; break;
                case RegisterPairSP.DE: m_DE.Register++; break;
                case RegisterPairSP.HL: m_HL.Register++; break;
                case RegisterPairSP.SP: m_SP.Register++; break;
            }
            return 6;
        }

        /// <summary>
        /// r identifies registers B, C, D, E, H, L, or A specified in the assembled object code field, as
        /// follows:
        /// Register r
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// A 111
        /// A logical AND operation is performed between the byte specified by the s operand and the
        /// byte contained in the Accumulator; the result is stored in the Accumulator.
        /// Instruction M Cycles    T States            4 MHz E.T.
        /// AND r       1           4                   1.00
        /// AND n       2           7 (4, 3)            1.75
        /// AND (HL)    2           7 (4, 3)            1.75
        /// AND (IX+d)  5           19 (4, 4, 3, 5, 3)  4.75
        /// AND (IX+d)  5           19 (4, 4, 3. 5, 3)  4.75
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set.
        /// P/V is reset if overflow; otherwise, it is reset.
        /// N is reset.
        /// C is reset.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private uint and_r(Register r)
        {
            switch (r)
            {
                case Register.A: m_AF.Hi = and(m_AF.Hi, m_AF.Hi); break;
                case Register.B: m_AF.Hi = and(m_AF.Hi, m_BC.Hi); break;
                case Register.C: m_AF.Hi = and(m_AF.Hi, m_BC.Lo); break;
                case Register.D: m_AF.Hi = and(m_AF.Hi, m_DE.Hi); break;
                case Register.E: m_AF.Hi = and(m_AF.Hi, m_DE.Lo); break;
                case Register.H: m_AF.Hi = and(m_AF.Hi, m_HL.Hi); break;
                case Register.L: m_AF.Hi = and(m_AF.Hi, m_HL.Lo); break;
            }
            return 4;
        }

        /// <summary>
        /// Register r is incremented and register r identifies any of the registers A, B, C, D, E, H, or
        /// L, assembled as follows in the object code.
        /// Register r
        /// A 111
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if r was 7Fh before operation; otherwise, it is reset.
        /// N is reset.
        /// C is not affected.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private uint inc_r(Register s)
        {
            switch (s)
            {
                case Register.A: m_AF.Hi = inc(m_AF.Hi); break;
                case Register.B: m_BC.Hi = inc(m_BC.Hi); break;
                case Register.C: m_BC.Lo = inc(m_BC.Lo); break;
                case Register.D: m_DE.Hi = inc(m_DE.Hi); break;
                case Register.E: m_DE.Lo = inc(m_DE.Lo); break;
                case Register.H: m_HL.Hi = inc(m_HL.Hi); break;
                case Register.L: m_HL.Lo = inc(m_HL.Lo); break;
            }
            return 4;
        }

        private uint cp_r(Register reg)
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
            switch(reg)
            {
                case Register.B: cp(m_AF.Hi, m_BC.Hi); break;
                case Register.C: cp(m_AF.Hi, m_BC.Lo); break;
                case Register.D: cp(m_AF.Hi, m_DE.Hi); break;
                case Register.E: cp(m_AF.Hi, m_DE.Lo); break;
                case Register.H: cp(m_AF.Hi, m_HL.Hi); break;
                case Register.L: cp(m_AF.Hi, m_HL.Lo); break;
                case Register.A: cp(m_AF.Hi, m_AF.Hi); break;
            }
            return 4;
        }

        private uint cp_n()
        {
            cp(m_AF.Hi, m_bus.Read(getN()));
            return 7;
        }

        private uint cp_hl()
        {
            cp(m_AF.Hi, m_bus.Read(m_HL.Register));
            return 7;
        }

        private uint cp_ixd()
        {
            cp(m_AF.Hi, m_bus.Read((byte)(m_bus.Read(m_PC.Register) + m_IX)));
            m_PC.Register++;
            return 19;
        }

        private uint cp_iyd()
        {
            cp(m_AF.Hi, m_bus.Read((byte)(m_bus.Read(m_PC.Register) + m_IY)));
            m_PC.Register++;
            return 19;
        }

        private uint jp__hl__()
        {
            //Description: The Program Counter (register pair PC) is loaded with the contents of the
            //HL register pair. The next instruction is fetched from the location
            //designated by the new contents of the PC.
            //	M Cycles	T States	4 MHz E.T.
            //	1			4			1.00
            //Condition Bits Affected: None
            m_PC.Register = m_HL.Register;
            return 4;
        }

        private uint jp_nn()
        {
            //Note: The first operand in this assembled object code is the low order
            //byte of a two-byte address.
            //Description: Operand nn is loaded to register pair PC (Program Counter). The next
            //instruction is fetched from the location designated by the new contents of
            //the PC.
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            m_PC.Register = getNN();
            return 10;
        }

        private uint ld__hl__n()
        {
            //Description: Integer n is loaded to the memory address specified by the contents of the
            //HL register pair.
            //	M Cycles	T States		4 MHz E.T.
            //	3			10 (4, 3, 3)	2.50
            //Condition Bits Affected: None
            m_bus.Write(m_HL.Register, getN());
            return 10;
        }

        private uint ld_sp_hl()
        {
            //Description: The contents of the register pair HL are loaded to the Stack Pointer (SP).
            //	M Cycles	T States	4 MHz E.T.
            //	1			6			1.5
            //Condition Bits Affected: None
            m_SP.Register = m_HL.Register;
            return 6;
        }

        /// <summary>
        /// The contents of the register pair qq are pushed to the external memory
        /// LIFO (last-in, first-out) Stack. The Stack Pointer (SP) register pair holds the
        /// 16-bit address of the current top of the Stack. This instruction first
        /// decrements SP and loads the high order byte of register pair qq to the
        /// memory address specified by the SP. The SP is decremented again and
        /// loads the low order byte of qq to the memory location corresponding to this
        /// new address in the SP. The operand qq identifies register pair BC, DE, HL,
        /// or AF, assembled as follows in the object code:
        /// Pair qq
        /// BC 00
        /// DE 01
        /// HL 10
        /// AF 11
        /// M Cycles	T States		4 MHz E.T.
        /// 3			11 (5, 3, 3)	2.75
        /// Condition Bits Affected: None
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        private uint push_qq(RegisterPairAF qq)
        {
            RegisterPair data = new RegisterPair();
            switch(qq)
            {
                case RegisterPairAF.BC: data.Register = m_BC.Register; break;
                case RegisterPairAF.DE: data.Register = m_DE.Register; break;
                case RegisterPairAF.HL: data.Register = m_HL.Register; break;
                case RegisterPairAF.AF: data.Register = m_AF.Register; break;
            }
            m_SP.Register--;
            m_bus.Write(m_SP.Register, data.Hi);
            m_SP.Register--;
            m_bus.Write(m_SP.Register, data.Lo);
            return 11;
        }

        private uint pop_qq(RegisterPairAF qq)
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
            RegisterPair data = new RegisterPair();
            data.Lo = m_bus.Read(m_SP.Register);
            m_SP.Register++;
            data.Hi = m_bus.Read(m_SP.Register);
            m_SP.Register++;
            switch(qq)
            {
                case RegisterPairAF.BC: m_BC.Register = data.Register; break;
                case RegisterPairAF.DE: m_DE.Register = data.Register; break;
                case RegisterPairAF.HL: m_HL.Register = data.Register; break;
                case RegisterPairAF.AF: m_AF.Register = data.Register; break;
            }
            return 10;
        }

        private uint ld_i_a()
        {
            //Description: The contents of the Accumulator are loaded to the Interrupt Control Vector
            //Register, I.
            //	M Cycles	T States	MHz E.T.
            //	2			9 (4, 5)	2.25
            //Condition Bits Affected: None
            m_I = m_AF.Hi;
            return 9;
        }

        private uint im_0()
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

        private uint im_1()
        {
            //Description: The IM 1 instruction sets interrupt mode 1. In this mode, the processor
            //responds to an interrupt by executing a restart to location 0038H.
            //	M Cycles	T States	4 MHz E.T.
            //	2			8 (4, 4)	2.00
            //Condition Bits Affected: None
            m_interruptMode = InterruptMode.Mode_1;
            return 8;
        }

        private uint im_2()
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

        private uint ld_iy_nn()
        {
            //Description: Integer nn is loaded to the Index Register IY. The first n operand after the
            //Op Code is the low order byte.
            //	M Cycles	T States		4 MHz E.T.
            //	4			14 (4, 4, 3, 3)	3.50
            //Condition Bits Affected: None
            m_IY = 0x0000;
            m_IY |= (short)(m_bus.Read(m_PC.Register) & 0x00FF);
            m_PC.Register++;
            m_IY |= (short)(m_bus.Read(m_PC.Register) << 8);
            m_PC.Register++;
            return 14;
        }

        private uint ld__nn__hl()
        {
            //Description: The contents of the low order portion of register pair HL (register L) are
            //loaded to memory address (nn), and the contents of the high order portion
            //of HL (register H) are loaded to the next highest memory address (nn+1).
            //The first n operand after the Op Code is the low order byte of nn.
            //	M Cycles	T States			4 MHz E.T.
            //	5			16 (4, 3, 3, 3, 3)	4.00
            //Condition Bits Affected: None
            ushort nn = getNN();
            m_bus.Write(nn, m_HL.Lo);
            m_bus.Write((ushort)(nn+1), m_HL.Hi);
            return 16;
        }

        private uint ld_hl__nn__()
        {
            //Description: The contents of memory address (nn) are loaded to the low order portion of
            //register pair HL (register L), and the contents of the next highest memory
            //address (nn+1) are loaded to the high order portion of HL (register H). The
            //first n operand after the Op Code is the low order byte of nn.
            //	M Cycles	T States			4 MHz E.T.
            //	5			16 (4, 3, 3, 3, 3)	4.00
            //Condition Bits Affected: None
            ushort nn = getNN();
            m_HL.Lo = m_bus.Read(nn);
            m_HL.Hi = m_bus.Read((ushort)(nn+1));
            return 16;
        }

        private uint ld_iyd_n()
        {
            //Description: Integer n is loaded to the memory location specified by the contents of the
            //Index Register summed with the two’s complement displacement integer d.
            //	M Cycles	T States			4 MHz E.T.
            //	5			19 (4, 4, 3, 5, 3)	2.50
            //Condition Bits Affected: None
            sbyte d = (sbyte)getN();
            byte n = getN();
            m_bus.Write((ushort)(m_IY + d), n);
            return 19;
        }

        private uint ld_r_iyd(Register r)
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
            sbyte d = (sbyte)getN();
            byte data = m_bus.Read((ushort)(m_IY + d));
            switch(r)
            {
                case Register.B: m_BC.Hi = data; break;
                case Register.C: m_BC.Lo = data; break;
                case Register.D: m_DE.Hi = data; break;
                case Register.E: m_DE.Lo = data; break;
                case Register.H: m_HL.Hi = data; break;
                case Register.L: m_HL.Lo = data; break;
                case Register.A: m_AF.Hi = data; break;
            }
            return 19;
        }

        private uint call_nn()
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
            ushort nn = getNN();
            m_SP.Register--;
            m_bus.Write(m_SP.Register, m_PC.Hi);
            m_SP.Register--;
            m_bus.Write(m_SP.Register, m_PC.Lo);
            m_PC.Register = nn;
            return 17;
        }

        /// <summary>
        /// If condition cc is true, this instruction pushes the current contents of the Program Counter
        /// (PC) onto the top of the external memory stack, then loads the operands nn to PC to point
        /// to the address in memory at which the first op code of a subroutine is to be fetched. At the
        /// end of the subroutine, a RETurn instruction can be used to return to the original program
        /// flow by popping the top of the stack back to PC. If condition cc is false, the Program
        /// Counter is incremented as usual, and the program continues with the next sequential
        /// instruction. The stack push is accomplished by first decrementing the current contents of
        /// the Stack Pointer (SP), loading the high-order byte of the PC contents to the memory
        /// address now pointed to by SP; then decrementing SP again, and loading the low-order
        /// byte of the PC contents to the top of the stack.
        /// Because this process is a 3-byte instruction, the Program Counter was incremented by
        /// three before the push is executed.
        /// Condition cc is programmed as one of eight statuses that corresponds to condition bits in
        /// the Flag Register (Register F). These eight statuses are defined in the following table,
        /// which also specifies the corresponding cc bit fields in the assembled object code.
        /// cc  Condition       Relevant Flag
        /// 000 Non-Zero        (NZ) Z
        /// 001 Zero            (Z) Z
        /// 010 Non Carry       (NC) C
        /// 011 Carry           (C) Z
        /// 100 Parity Odd      (PO) P/V
        /// 101 Parity Even     (PE) P/V
        /// 110 Sign Positive   (P) S
        /// 111 Sign Negative   (M) S
        /// If cc is true:
        /// M Cycles    T States            4 MHz E.T.
        /// 5           17 (4, 3, 4, 3, 3)  4.25
        /// If cc is false:
        /// M Cycles    T States            4 MHz E.T.
        /// 3           10 (4, 3, 3)        2.50
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private uint call_cc_nn(Condition condition)
        {
            uint cycles = 10;
            if (check_condition(condition))
            {
                ushort nn = getNN();
                m_SP.Register--;
                m_bus.Write(m_SP.Register, m_PC.Hi);
                m_SP.Register--;
                m_bus.Write(m_SP.Register, m_PC.Lo);
                m_PC.Register = nn;
                cycles = 17;
            }
            return cycles;
        }

        /// <summary>
        /// The contents of address (nn) are loaded to the low-order portion of register pair dd, and the
        /// contents of the next highest memory address (nn + 1) are loaded to the high-order portion
        /// of dd. Register pair dd defines BC, DE, HL, or SP register pairs, assembled as follows in
        /// the object code:
        /// Pair dd
        /// BC
        /// DE 01
        /// HL 10
        /// SP 11
        /// The first n operand after the op code is the low-order byte of (nn).
        /// M Cycles    T States                4 MHz E.T.
        /// 6           20 (4, 4, 3, 3, 3, 3)   5.00
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <param name="dd"></param>
        /// <returns></returns>
        private uint ld_dd__nn__(RegisterPairSP dd)
        {
            ushort nn = getNN();
            RegisterPair data = new RegisterPair();
            switch (dd)
            {
                case RegisterPairSP.BC: data.Register = m_BC.Register; break;
                case RegisterPairSP.DE: data.Register = m_DE.Register; break;
                case RegisterPairSP.HL: data.Register = m_HL.Register; break;
                case RegisterPairSP.SP: data.Register = m_SP.Register; break;
            }
            data.Lo = m_bus.Read(nn);
            data.Hi = m_bus.Read((ushort)(nn + 1));
            return 20;
        }

        /// <summary>
        /// The low order byte of register pair dd is loaded to memory address (nn); the
        /// upper byte is loaded to memory address (nn+1). Register pair dd defines
        /// either BC, DE, HL, or SP, assembled as follows in the object code:
        /// Pair dd
        /// BC 00
        /// DE 01
        /// HL 10
        /// SP 11
        /// The first n operand after the Op Code is the low order byte of a two byte
        /// memory address.
        /// M Cycles    T States                4 MHz E.T.
        /// 6           20 (4, 4, 3, 3, 3, 3)   5.00
        /// Condition Bits Affected: None
        /// </summary>
        /// <param name="dd"></param>
        /// <returns></returns>
        private uint ld__nn__dd(RegisterPairSP dd)
        {
            ushort nn = getNN();
            RegisterPair data = new RegisterPair();
            switch (dd)
            {
                case RegisterPairSP.BC: data.Register = m_BC.Register; break;
                case RegisterPairSP.DE: data.Register = m_DE.Register; break;
                case RegisterPairSP.HL: data.Register = m_HL.Register; break;
                case RegisterPairSP.SP: data.Register = m_SP.Register; break;
            }
            m_bus.Write(nn, data.Lo);
            m_bus.Write((ushort)(nn+1), data.Hi);
            return 20;
        }

        private uint ret()
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
            m_PC.Lo = m_bus.Read(m_SP.Register);
            m_SP.Register++;
            m_PC.Hi = m_bus.Read(m_SP.Register);
            m_SP.Register++;
            return 10;
        }

        private uint ret_cc(Condition cc)
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
            uint cycles = 5;
            if (check_condition(cc))
            {
                m_PC.Lo = m_bus.Read(m_SP.Register);
                m_SP.Register++;
                m_PC.Hi = m_bus.Read(m_SP.Register);
                m_SP.Register++;
                cycles = 11;
            }
            return cycles;
        }

        /// <summary>
        /// The contents of register r are added to the contents of the Accumulator, and the result is
        /// stored in the Accumulator. The r symbol identifies the registers A, B, C, D, E, H, or L,
        /// assembled as follows in the object code:
        /// Register r
        /// A 111
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if carry from bit 3; otherwise, it is reset.
        /// P/V is set if overflow; otherwise, it is reset.
        /// N is reset.
        /// C is set if carry from bit 7; otherwise, it is reset.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private uint add_a_r(Register r)
        {
            switch (r)
            {
                case Register.A: m_AF.Hi = add(m_AF.Hi, m_AF.Hi); break;
                case Register.B: m_AF.Hi = add(m_AF.Hi, m_BC.Hi); break;
                case Register.C: m_AF.Hi = add(m_AF.Hi, m_BC.Lo); break;
                case Register.D: m_AF.Hi = add(m_AF.Hi, m_DE.Hi); break;
                case Register.E: m_AF.Hi = add(m_AF.Hi, m_DE.Lo); break;
                case Register.H: m_AF.Hi = add(m_AF.Hi, m_HL.Hi); break;
                case Register.L: m_AF.Hi = add(m_AF.Hi, m_HL.Lo); break;
            }
            return 4;
        }

        /// <summary>
        /// The contents of register pair ss (any of register pairs BC, DE, HL, or SP) are added to the
        /// contents of register pair HL and the result is stored in HL. In the assembled object code,
        /// operand ss is specified as follows:
        /// Register
        /// Pair ss
        /// BC 00
        /// DE 01
        /// HL 10
        /// SP 11
        /// M Cycles    T States        4 MHz E.T.
        /// 3           11 (4, 4, 3)    2.75
        /// Condition Bits Affected
        /// S is not affected.
        /// Z is not affected.
        /// H is set if carry from bit 11; otherwise, it is reset.
        /// P/V is not affected.
        /// N is reset.
        /// C is set if carry from bit 15; otherwise, it is reset.
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private uint add_hl_ss(RegisterPairSP ss)
        {
            short data = 0x0000;
            switch(ss)
            {
                case RegisterPairSP.BC: data = (short)m_BC.Register; break;
                case RegisterPairSP.DE: data = (short)m_DE.Register; break;
                case RegisterPairSP.HL: data = (short)m_HL.Register; break;
                case RegisterPairSP.SP: data = (short)m_SP.Register; break;
            }
            short result = (short)((short)m_HL.Register + data);
            if ((short)((m_HL.Register & 0x0FFF) ^ (data & 0x0FFF)) > 0x0000) // carry from bit 11, not sure this will work
            {
                m_AF.Lo |= (byte)FlagMask.H;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NH;
            }
            m_AF.Lo &= (byte)FlagMask.NN;
            if ((short)(m_HL.Register ^ data) > 0x0000) // carry from bit 15, not sure this will work
            {
                m_AF.Lo |= (byte)FlagMask.C;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NC;
            }
            m_HL.Register = (ushort)result;
            return 11;
        }

        /// <summary>
        /// Register r
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// A 111
        /// The s operand is subtracted from the contents of the Accumulator, and the 
        /// result is stored in the Accumulator.
        /// Instruction M Cycle T States            4 MHz E.T.
        /// SUB r       1       4                   1.00
        /// SUB n       2       7 (4, 3)            1.75
        /// SUB (HL)    2       7 (4, 3)            1.75
        /// SUB (IX+d)  5       19 (4, 4, 3, 5, 3)  4.75
        /// SUB (lY+d)  5       19 (4, 4, 3, 5, 3)  4.75
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if overflow; otherwise, it is reset.
        /// N is set.
        /// C is set if borrow; otherwise, it is reset.
        /// </summary>
        /// <returns></returns>
        private uint sub_s(Register s)
        {
            uint cycles = 4;
            switch (s)
            {
                case Register.A: m_AF.Hi = sub(m_AF.Hi, m_AF.Hi); break;
                case Register.B: m_AF.Hi = sub(m_AF.Hi, m_BC.Hi); break;
                case Register.C: m_AF.Hi = sub(m_AF.Hi, m_BC.Lo); break;
                case Register.D: m_AF.Hi = sub(m_AF.Hi, m_DE.Hi); break;
                case Register.E: m_AF.Hi = sub(m_AF.Hi, m_DE.Lo); break;
                case Register.H: m_AF.Hi = sub(m_AF.Hi, m_HL.Hi); break;
                case Register.L: m_AF.Hi = sub(m_AF.Hi, m_HL.Lo); break;
            }
            return cycles;
        }

        private uint sub_hl()
        {
            m_AF.Hi = sub(m_AF.Hi, m_bus.Read(m_HL.Register));
            return 7;
        }

        private uint sub_n()
        {
            m_AF.Hi = sub(m_AF.Hi, getN());
            return 7;
        }

        private uint sub_ixd()
        {
            m_AF.Hi = sub(m_AF.Hi, m_bus.Read((ushort)(m_IX + (sbyte)getN())));
            return 19;
        }

        private uint sub_iyd()
        {
            m_AF.Hi = sub(m_AF.Hi, m_bus.Read((ushort)(m_IY + (sbyte)getN())));
            return 19;
        }

        /// <summary>
        /// The contents of the register pair ss (any of register pairs BC, DE, HL, or SP) and the Carry
        /// Flag (C flag in the F Register) are subtracted from the contents of register pair HL, and the
        /// result is stored in HL. In the assembled object code, operand ss is specified as follows:
        /// Register
        /// Pair ss
        /// BC 00
        /// DE 01
        /// HL 10
        /// SP 11
        /// M Cycles    T States        4 MHz E.T.
        /// 4           15 (4, 4, 4, 3) 3.75
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is set if borrow from bit 12; otherwise, it is reset.
        /// P/V is set if overflow; otherwise, it is reset.
        /// N is set.
        /// C is set if borrow; otherwise, it is reset.
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private uint sbc_hl_ss(RegisterPairSP ss)
        {
            ushort val = (ushort)(m_AF.Lo & (byte)Condition.C);
            switch (ss)
            {
                case RegisterPairSP.BC: val += m_BC.Register; break;
                case RegisterPairSP.DE: val += m_DE.Register; break;
                case RegisterPairSP.HL: val += m_HL.Register; break;
                case RegisterPairSP.SP: val += m_SP.Register; break;
            }
            m_HL.Register = sbc(m_HL.Register, val);
            return 15;
        }

        /// <summary>
        /// The HALT instruction suspends CPU operation until a subsequent interrupt
        /// or reset is received. While in the HALT state, the processor executes NOPs
        /// to maintain memory refresh logic.
        /// M Cycles	T States	4 MHz E.T.
        /// 1			4			1.00
        /// Condition Bits Affected: None
        /// </summary>
        /// <returns></returns>
        private uint halt()
        {
            m_halted = true;
            return 4;
        }

        /// <summary>
        /// The enable interrupt instruction sets both interrupt enable flip flops (IFFI
        /// and IFF2) to a logic 1, allowing recognition of any maskable interrupt. Note
        /// that during the execution of this instruction and the following instruction,
        /// maskable interrupts are disabled.
        /// M Cycles	T States	4 MHz E.T.
        /// 1			4			1.00
        /// Condition Bits Affected: None
        /// </summary>
        /// <returns></returns>
        private uint ei()
        {
            m_IFF1 = true;
            m_IFF2 = true;
            return 4;
        }

        /// <summary>
        /// The 2-byte contents of register pairs DE and HL are exchanged.
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <returns></returns>
        private uint ex_de_hl()
        {
            ushort temp;

            temp = m_DE.Register;
            m_DE.Register = m_HL.Register;
            m_HL.Register = temp;

            return 4;
        }

        /// <summary>
        /// Each 2-byte value in register pairs BC, DE, and HL is exchanged with the 2-
        /// byte value in BC', DE', and HL', respectively.
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// None.
        /// </summary>
        /// <returns></returns>
        private uint exx()
        {
            ushort temp;

            temp = m_BC.Register;
            m_BC.Register = m__BC.Register;
            m__BC.Register = temp;

            temp = m_DE.Register;
            m_DE.Register = m__DE.Register;
            m__DE.Register = temp;

            temp = m_HL.Register;
            m_HL.Register = m__HL.Register;
            m__HL.Register = temp;

            return 4;
        }

        /// <summary>
        /// The current Program Counter (PC) contents are pushed onto the external memory stack,
        /// and the Page 0 memory location assigned by operand p is loaded to the PC. Program execution
        /// then begins with the op code in the address now pointed to by PC. The push is performed
        /// by first decrementing the contents of the Stack Pointer (SP), loading the high-order
        /// byte of PC to the memory address now pointed to by SP, decrementing SP again, and loading
        /// the low-order byte of PC to the address now pointed to by SP. The Restart instruction
        /// allows for a jump to one of eight addresses indicated in the following table. The operand p
        /// is assembled to the object code using the corresponding T state.
        /// Because all addresses are stored in Page 0 of memory, the high-order byte of PC is loaded
        /// with 00h. The number selected from the p column of the table is loaded to the low-order
        /// byte of PC.
        /// p t
        /// 00h 000
        /// 08h 001
        /// 10h 010
        /// 18h 011
        /// 20h 100
        /// 28h 101
        /// 30h 110
        /// 38h 111
        /// M Cycles    T States        4 MHz E.T.
        /// 3           11 (5, 3, 3)    2.75
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private uint rst_p(ResetLocation p)
        {
            m_SP.Register--;
            m_bus.Write(m_SP.Register, m_PC.Hi);
            m_SP.Register--;
            m_bus.Write(m_SP.Register, m_PC.Lo);
            switch(p)
            {
                case ResetLocation.Addr00h: m_PC.Register = 0x00; break;
                case ResetLocation.Addr08h: m_PC.Register = 0x08; break;
                case ResetLocation.Addr10h: m_PC.Register = 0x10; break;
                case ResetLocation.Addr18h: m_PC.Register = 0x18; break;
                case ResetLocation.Addr20h: m_PC.Register = 0x20; break;
                case ResetLocation.Addr28h: m_PC.Register = 0x28; break;
                case ResetLocation.Addr30h: m_PC.Register = 0x30; break;
                case ResetLocation.Addr38h: m_PC.Register = 0x38; break;
            }
            return 3;
        }

        /// <summary>
        /// The contents of the memory location addressed by the HL register pair is compared with
        /// the contents of the Accumulator. During a compare operation, a condition bit is set. HL is
        /// incremented and the Byte Counter (register pair BC) is decremented. If decrementing
        /// causes BC to go to 0 or if A = (HL), the instruction is terminated. If BC is not 0 and A ≠
        /// (HL), the program counter is decremented by two and the instruction is repeated. Interrupts
        /// are recognized and two refresh cycles are executed after each data transfer.
        /// If BC is set to 0 before instruction execution, the instruction loops through 64 KB if no
        /// match is found.
        /// For BC ≠ 0 and A ≠ (HL):
        /// M Cycles    T States            4 MHz E.T.
        /// 5           21 (4, 4, 3, 5, 5)  5.25
        /// For BC = 0 and A = (HL):
        /// M Cycles    T States            4 MHz E.T.
        /// 4           16 (4, 4, 3, 5)     4.00
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if A equals (HL); otherwise, it is reset.
        /// H is set if borrow from bit 4; otherwise, it is reset.
        /// P/V is set if BC – 1 does not equal 0; otherwise, it is reset.
        /// N is set.
        /// C is not affected.
        /// </summary>
        /// <returns></returns>
        private uint cpir()
        {
            uint cycles = 16;
            byte data = m_bus.Read(m_HL.Register);
            cpn(m_AF.Hi, data);
            m_HL.Register++;
            m_BC.Register--;
            if (m_BC.Register != 0x0000)
            {
                m_AF.Lo |= (byte)Condition.PE;
            }
            else
            {
                m_AF.Lo &= (byte)Condition.PO;
            }
            if (m_BC.Register != 0x0000 && m_AF.Hi != data)
            {
                m_PC.Register -= 2;
                cycles = 21;
            }
            return cycles;
        }

        /// <summary>
        /// The contents of the Accumulator (Register A) are rotated left 1 bit position. The sign bit
        /// (bit 7) is copied to the Carry flag and also to bit 0. Bit 0 is the least-significant bit.
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is not affected.
        /// N is reset.
        /// C is data from bit 7 of Accumulator.
        /// </summary>
        /// <returns></returns>
        private uint rlca()
        {
            m_AF.Hi = rl(m_AF.Hi, true);
            return 4;
        }

        /// <summary>
        /// The contents of the Accumulator (Register A) are rotated right 1 bit position. Bit 0 is copied
        /// to the Carry flag and also to bit 7. Bit 0 is the least-significant bit.
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is not affected.
        /// N is reset.
        /// C is data from bit 0 of Accumulator.
        /// </summary>
        /// <returns></returns>
        private uint rrca()
        {
            m_AF.Hi = rr(m_AF.Hi, true);
            return 4;
        }

        /// <summary>
        /// Register r
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// A 111
        /// The contents of the m operand are rotated left 1 bit position. The contents of bit 7 are copied
        /// to the Carry flag, and the previous contents of the Carry flag are copied to bit 0.
        /// Instruction M Cycles    T States                4 MHz E.T.
        /// RL r        2           8 (4, 4)                2.00
        /// RL (HL)     4           15(4, 4, 4, 3)          3.75
        /// RL (IX+d)   6           23 (4, 4, 3, 5, 4, 3)   5.75
        /// RL (IY+d)   6           23 (4, 4, 3, 5, 4, 3)   5.75
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// N is reset.
        /// C is data from bit 7 of source register.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private uint rl_r(Register reg)
        {
            switch (reg)
            {
                case Register.A: m_AF.Hi = rl(m_AF.Hi, false); break;
                case Register.B: m_BC.Hi = rl(m_BC.Hi, false); break;
                case Register.C: m_BC.Lo = rl(m_BC.Lo, false); break;
                case Register.D: m_DE.Hi = rl(m_DE.Hi, false); break;
                case Register.E: m_DE.Lo = rl(m_DE.Lo, false); break;
                case Register.H: m_HL.Hi = rl(m_HL.Hi, false); break;
                case Register.L: m_HL.Lo = rl(m_HL.Lo, false); break;
            }
            return 8;
        }

        private uint rl_hl()
        {
            throw new NotImplementedException();
        }

        private uint rl_ixd()
        {
            throw new NotImplementedException();
        }

        private uint rl_iyd()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Carry flag in the F Register is set.
        /// M Cycles    T States    4 MHz E.T.
        /// 1           4           1.00
        /// Condition Bits Affected
        /// S is not affected.
        /// Z is not affected.
        /// H is reset.
        /// P/V is not affected.
        /// N is reset.
        /// C is set.
        /// </summary>
        /// <returns></returns>
        private uint scf()
        {
            m_AF.Lo &= (byte)FlagMask.NH;
            m_AF.Lo &= (byte)FlagMask.NN;
            m_AF.Lo |= (byte)FlagMask.C;
            return 4;
        }

        /// <summary>
        /// Register r
        /// B 000
        /// C 001
        /// D 010
        /// E 011
        /// H 100
        /// L 101
        /// A 1l1
        /// The logical exclusive-OR operation is performed between the byte specified by the s operand
        /// and the byte contained in the Accumulator; the result is stored in the Accumulator.
        /// Instruction M Cycles    T States            4 MHz E.T.
        /// XOR r       1           4                   1.00
        /// XOR n       2           7 (4, 3)            1.75
        /// XOR (HL)    2           7 (4, 3)            1.75
        /// XOR (IX+d)  5           19 (4, 4, 3, 5, 3)  4.75
        /// XOR (lY+d)  5           19 (4, 4, 3, 5, 3)  4.75
        /// Condition Bits Affected
        /// S is set if result is negative; otherwise, it is reset.
        /// Z is set if result is 0; otherwise, it is reset.
        /// H is reset.
        /// P/V is set if parity even; otherwise, it is reset.
        /// N is reset.
        /// C is reset.
        /// </summary>
        /// <returns></returns>
        private uint xor_r(Register reg)
        {
            switch(reg)
            {
                case Register.A: m_AF.Hi = xor(m_AF.Hi, m_AF.Hi); break;
                case Register.B: m_AF.Hi = xor(m_AF.Hi, m_BC.Hi); break;
                case Register.C: m_AF.Hi = xor(m_AF.Hi, m_BC.Lo); break;
                case Register.D: m_AF.Hi = xor(m_AF.Hi, m_DE.Hi); break;
                case Register.E: m_AF.Hi = xor(m_AF.Hi, m_DE.Lo); break;
                case Register.H: m_AF.Hi = xor(m_AF.Hi, m_HL.Hi); break;
                case Register.L: m_AF.Hi = xor(m_AF.Hi, m_HL.Lo); break;
            }
            return 4;
        }

        private uint xor_n()
        {
            throw new NotImplementedException();
        }

        private uint xor_hl()
        {
            throw new NotImplementedException();
        }

        private uint xor_ixd()
        {
            throw new NotImplementedException();
        }

        private uint xor_iyd()
        {
            throw new NotImplementedException();
        }


        private void incrementR()
        {
            m_R = (byte)((m_R + 1) & 0x7F);
        }

        private byte getN()
        {
            byte n;
            n = m_bus.Read(m_PC.Register);
            m_PC.Register++;
            return n;
        }

        private ushort getNN()
        {
            RegisterPair nn = new RegisterPair();
            nn.Lo = m_bus.Read(m_PC.Register);
            m_PC.Register++;
            nn.Hi = m_bus.Read(m_PC.Register);
            m_PC.Register++;
            return nn.Register;
        }

        private byte and(byte inputA, byte inputB)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if result is 0; otherwise, it is reset.
            // H is set.
            // P/V is reset if overflow; otherwise, it is reset.
            // N is reset.
            // C is reset.
            sbyte result = (sbyte)((sbyte)inputA & (sbyte)inputB);
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            m_AF.Lo |= (byte)FlagMask.H;
            if (((sbyte)inputA + (sbyte)inputB) > byte.MaxValue)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo &= (byte)FlagMask.NN;
            m_AF.Lo &= (byte)FlagMask.NC;
            return (byte)result;
        }

        private byte inc(byte input)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if result is 0; otherwise, it is reset.
            // H is set if carry from bit 3; otherwise, it is reset.
            // P/V is set if r was 7Fh before operation; otherwise, it is reset.
            // N is reset.
            // C is not affected.
            sbyte result = (sbyte)input;
            result++;
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            //TODO: H FLAG
            if (input == 0x7F)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo |= (byte)FlagMask.N;
            return (byte)result;
        }

        private byte dec(byte input)
        {
            // Condition Bits Affected:
            // S is set if result is negative; reset otherwise
            // Z is set if result is zero; reset otherwise
            // H is set if borrow from bit 4, reset otherwise
            // P/V is set if m was 80H before operation; reset otherwise
            // N is set
            // C is not affected
            sbyte result = (sbyte)input;
            result--;
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            //TODO: H FLAG
            if (input == 0x80)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo |= (byte)FlagMask.N;
            return (byte)result;
        }

        private byte rl(byte input, bool forA)
        {
            sbyte result = (sbyte)(input << 8);
            if ((input & 0x80) == 0x80)
            {
                result |= 0x01;
            }
            if (forA)
            {
                if (result < 0)
                {
                    m_AF.Lo |= (byte)FlagMask.S;
                }
                else
                {
                    m_AF.Lo &= (byte)FlagMask.NS;
                }
                if (result == 0)
                {
                    m_AF.Lo |= (byte)FlagMask.Z;
                }
                else
                {
                    m_AF.Lo &= (byte)FlagMask.NZ;
                }
                if ((result & 0x01) == 0x01)
                {
                    m_AF.Lo |= (byte)FlagMask.PE;
                }
                else
                {
                    m_AF.Lo &= (byte)FlagMask.PO;
                }
            }
            m_AF.Lo &= (byte)FlagMask.NH;
            m_AF.Lo &= (byte)FlagMask.NN;
            if ((input & 0x80) == 0x80)
            {
                m_AF.Lo |= (byte)FlagMask.C;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NC;
            }
            return (byte)result;
        }

        private byte rr(byte input, bool forA)
        {
            sbyte result = (sbyte)(input >> 8);
            if ((input & 0x01) == 0x01)
            {
                result = (sbyte)((byte)result | 0x80);
            }
            if (forA)
            {
                if (result < 0)
                {
                    m_AF.Lo |= (byte)FlagMask.S;
                }
                else
                {
                    m_AF.Lo &= (byte)FlagMask.NS;
                }
                if (result == 0)
                {
                    m_AF.Lo |= (byte)FlagMask.Z;
                }
                else
                {
                    m_AF.Lo &= (byte)FlagMask.NZ;
                }
                if ((result & 0x01) == 0x01)
                {
                    m_AF.Lo |= (byte)FlagMask.PE;
                }
                else
                {
                    m_AF.Lo &= (byte)FlagMask.PO;
                }
            }
            m_AF.Lo &= (byte)FlagMask.NH;
            m_AF.Lo &= (byte)FlagMask.NN;
            if ((input & 0x01) == 0x01)
            {
                m_AF.Lo |= (byte)FlagMask.C;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NC;
            }
            return (byte)result;
        }

        private byte add(byte inputA, byte inputB)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if result is 0; otherwise, it is reset.
            // H is set if carry from bit 3; otherwise, it is reset.
            // P/V is set if overflow; otherwise, it is reset.
            // N is reset.
            // C is set if carry from bit 7; otherwise, it is reset.
            sbyte result = (sbyte)((sbyte)inputA + (sbyte)inputB);
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            //TODO: H FLAG
            if (((sbyte)inputA + (sbyte)inputB) > byte.MaxValue)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo |= (byte)FlagMask.N;
            return (byte)result;
        }

        private byte sub(byte inputA, byte inputB)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if result is 0; otherwise, it is reset.
            // H is set if borrow from bit 4; otherwise, it is reset.
            // P/V is set if overflow; otherwise, it is reset.
            // N is set.
            // C is set if borrow; otherwise, it is reset.
            sbyte result = (sbyte)((sbyte)inputA - (sbyte)inputB);
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            //TODO: H FLAG
            if (((sbyte)inputA - (sbyte)inputB) > byte.MaxValue)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo |= (byte)FlagMask.N;
            return (byte)result;
        }

        private byte xor(byte inputA, byte inputB)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if result is 0; otherwise, it is reset.
            // H is reset.
            // P/V is set if parity even; otherwise, it is reset.
            // N is reset.
            // C is reset.
            sbyte result = (sbyte)(inputA ^ inputB);
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            m_AF.Hi &= (byte)FlagMask.NH;
            if ((result & 0x01) == 0x01)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo &= (byte)FlagMask.NN;
            m_AF.Lo &= (byte)FlagMask.NC;
            return (byte)result;
        }

        private ushort sbc(ushort inputA, ushort inputB)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if result is 0; otherwise, it is reset.
            // H is set if borrow from bit 12; otherwise, it is reset.
            // P/V is set if overflow; otherwise, it is reset.
            // N is set.
            // C is set if borrow; otherwise, it is reset.
            short result = (short)((short)inputA - (short)inputB);
            if (result < 0)
            {
                m_AF.Lo |= (byte)FlagMask.S;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NS;
            }
            if (result == 0)
            {
                m_AF.Lo |= (byte)FlagMask.Z;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NZ;
            }
            //TODO: H FLAG
            if (result == 0x7F)
            {
                m_AF.Lo |= (byte)FlagMask.PE;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.PO;
            }
            m_AF.Lo |= (byte)FlagMask.N;
            if ((short)inputA < (short)inputB)
            {
                m_AF.Lo |= (byte) FlagMask.C;
            }
            else
            {
                m_AF.Lo &= (byte)FlagMask.NC;
            }
            return (ushort)result;
        }

        private void cp(byte inputA, byte inputB)
        {
            if (inputA > inputB)
            {
                m_AF.Lo |= (byte)Condition.S;
            }
            else
            {
                m_AF.Lo &= (byte)Condition.NS;
            }
            if (inputA == inputB)
            {
                m_AF.Lo |= (byte)Condition.Z;
            }
            else
            {
                m_AF.Lo &= (byte)Condition.NZ;
            }
            // TODO: H FLAG
            // TODO: PV FLAG
            if (inputA < inputB)
            {
                m_AF.Lo |= (byte)Condition.C;
            }
            else
            {
                m_AF.Lo &= (byte)Condition.NC;
            }
        }

        private void cpn(byte inputA, byte inputB)
        {
            // Condition Bits Affected
            // S is set if result is negative; otherwise, it is reset.
            // Z is set if A equals (HL); otherwise, it is reset.
            // H is set if borrow from bit 4; otherwise, it is reset.
            // P/V is set if BC – 1 does not equal 0; otherwise, it is reset.
            // N is set.
            // C is not affected.
            if (inputA > inputB)
            {
                m_AF.Lo |= (byte)Condition.S;
            }
            else
            {
                m_AF.Lo &= (byte)Condition.NS;
            }
            if (inputA == inputB)
            {
                m_AF.Lo |= (byte)Condition.Z;
            }
            else
            {
                m_AF.Lo &= (byte)Condition.NZ;
            }
            // TODO: H FLAG
            // TODO: PV FLAG
            m_AF.Lo |= (byte)FlagMask.N;
        }

        private bool check_condition(Condition condition)
        {
            bool ret = false;
            switch (condition)
            {
                case Condition.Z: ret = (m_AF.Lo & (byte)Condition.Z) == (byte)Condition.Z; break;
                case Condition.NZ: ret = (m_AF.Lo | (byte)Condition.NZ) == (byte)Condition.NZ; break;
                case Condition.C: ret = (m_AF.Lo & (byte)Condition.C) == (byte)Condition.C; break;
                case Condition.NC: ret = (m_AF.Lo | (byte)Condition.NC) == (byte)Condition.NC; break;
                case Condition.PE: ret = (m_AF.Lo & (byte)Condition.PE) == (byte)Condition.PE; break;
                case Condition.PO: ret = (m_AF.Lo | (byte)Condition.PO) == (byte)Condition.PO; break;
                case Condition.S: ret = (m_AF.Lo & (byte)Condition.S) == (byte)Condition.S; break;
                case Condition.NS: ret = (m_AF.Lo | (byte)Condition.NS) == (byte)Condition.NS; break;
                default: throw new NotImplementedException();
            }
            return ret;
        }
    }
}
