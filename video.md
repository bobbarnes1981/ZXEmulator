
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

http://problemkaputt.de/zxdocs.htm

Display procedure Tech Details
The display data is more or less 'executed' by the CPU. When displaying a line, the BIOS takes the address of the first character, eg. 4123h, sets Bit 15, ie. C123h, and then jumps to that address.

The hardware now senses A15=HIGH and /M1=LOW (signalizing opcode read), upon this condition memory is mirrored from C000-FFFF to 4000-7FFF. The 'opcode' is presented to the databus as usually, the display circuit interpretes it as character data, and (if Bit 6 is zero) forces the databus to zero before the CPU realizes what is going on, causing a NOP opcode (00h) to be executed. Bit 7 of the stolen opcode is used as invert attribute, Bit 0-5 address one of the 64 characters in ROM at (I*100h+char*8+linecntr), the byte at that address is loaded into a shift register, and bits are shifted to the display at a rate of 6.5MHz (ie. 8 pixels within 4 CPU cycles).

However, when encountering an opcode with Bit 6 set, then the video circuit rejects the opcode (displays white, regardless of Bit 7 and 0-5), and the CPU executes the opcode as normal. Usually this would be the HALT opcode - before displaying a line, BIOS enables INTs and initializes the R register, which will produce an interrupt when Bit 6 of R becomes zero.
In this special case R is incremented at a fixed rate of 4 CPU cycles (video data executed as NOPs, followed by repeated HALT), so that line display is suspended at a fixed time, regardless of the collapsed or expanded length of the line.

As mentioned above, an additional register called linecntr is used to address the vertical position (0..7) whithin a character line. This register is reset during vertical retrace, and then incremented once per scanline. The BIOS thus needs to 'execute' each character line eight times, before starting to 'execute' the next character line.
