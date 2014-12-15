ZXEmulator
==========

TODO: Change memory access so that DFILE at 0x8000 is executed as NOP

---

http://www.retrotechnology.com/memship/zx81.txt

The ZX-81 was a classic Clive Sinclair "mad genius" product. Very *VERY*
tricky and clever. The original ZX-80 had about 11 generic chips; the
ZX-81 got this down to 4 chips with one being a simple custom part. The
keyboard was just the PC board itself with a metalized mylar overlay;
when you pressed, it flexed enough to short traces on the PC board.

The video circuit was even simpler than the 1802/1861. It had a Z80,
static bytewide RAM, and ROM. These parts worked normally to hold the
program and data, just like any microcomputer. But, at the start of the
top video scan line:

- The Z80 did a JMP to *execute* the ASCII data to be displayed in
	 the RAM.
- But, the hardware opened the Z80's data bus, and pullup resistors
	 jammed in a NOP instruction instead.
- The Z80 executed 32 NOPs in the time it took for one scan line.
- Therefore, the Z80's address bus incremented 32 times.
- This read 32 consecutive bytes from the RAM (the characters to
	 display on the first line).
- The data from the RAM was patched to the *address* inputs of the
	 ROM. They selected the bit pattern lookup table for that
	 character.
- The data from the ROM was latched into an 8-bit video shift register,
	 and was shifted out to become video. The top row of dots for
	 the characters is displayed.
- The Z80 finishes its 32 NOPs at the end of the line, and continues
	 with another 32 NOPs during the next scan line. Its lower
	 address lines A0-A4 still go to the RAM, and scan the same
	 32 charcters again. But the Z80's A5-A7 lines keep climbing;
	 they are the row counter and go to the ROM to select which
	 row of dots gets output.
- This scheme continued to generate 24 lines of 32 characters, each
	 in an 8x8 dot matrix.
- When the Z80's address finally counted to FFFF (end of RAM), it
	 wrapped around to 0000 (the ROM), and normal program execution
	 resumed again.

---

http://z80-heaven.wikidot.com/opcode-reference-chart

http://www.wearmouth.demon.co.uk/zx80.txt

http://www.wearmouth.demon.co.uk/zx81.htm
