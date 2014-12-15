using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXEmulatorLibrary
{
    public class ZX80
    {
        private int m_videoWidth = 256;
        private int m_videoHeight = 192;

        //  ____0___1___2___3___4___5___6___7___8___9___A___B___C___D___E___F____
        //  00 SPC  "  [: ][..][' ][ '][. ][ .][.']{::}{..}{''}GBP  $   :   ?  0F
        //  10  (   )   -   +   *   /   =   >   <   ;   ,   .   0   1   2   3  1F
        //  20  4   5   6   7   8   9   A   B   C   D   E   F   G   H   I   J  2F
        //  30  K   L   M   N   O   P   Q   R   S   T   U   V   W   X   Y   Z  3F
        public static readonly char[] CharacterMap = new char[]
        {
            ' ', '"', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '£', '$', ':', '?',
            '(', ')', '-', '+', '*', '/', '=', '>', '<', ';', ',', '.', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };
    }
}
