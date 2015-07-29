using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class TsurgeonParserTokenManager : TsurgeonParserConstants
    {

        private int curLexState = 2;
        private const int DefaultLexState = 2;
        private int jjnewStateCnt;
        private uint jjround;
        private int jjmatchedPos;
        private int jjmatchedKind;

        protected SimpleCharStream InputStream;

        private readonly uint[] jjrounds = new uint[58];
        private readonly int[] jjstateSet = new int[2*58];


        protected char CurChar;

        private int JjStopAtPos(int pos, int kind)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            return pos + 1;
        }

        private int JjMoveStringLiteralDfa0_2()
        {
            switch ((int) CurChar)
            {
                case 91:
                    return JjStopAtPos(0, 5);
                case 97:
                    return JjMoveStringLiteralDfa1_2(0xe0000L);
                case 99:
                    return JjMoveStringLiteralDfa1_2(0x110000L);
                case 100:
                    return JjMoveStringLiteralDfa1_2(0x200L);
                case 101:
                    return JjMoveStringLiteralDfa1_2(0x1100L);
                case 105:
                    return JjMoveStringLiteralDfa1_2(0x2040L);
                case 109:
                    return JjMoveStringLiteralDfa1_2(0x4000L);
                case 110:
                    return JjMoveStringLiteralDfa1_2(0x80L);
                case 112:
                    return JjMoveStringLiteralDfa1_2(0x400L);
                case 114:
                    return JjMoveStringLiteralDfa1_2(0x8800L);
                default:
                    return 1;
            }
        }

        private int JjMoveStringLiteralDfa1_2(long active0)
        {
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 1;
            }
            switch ((int) CurChar)
            {
                case 100:
                    return JjMoveStringLiteralDfa2_2(active0, 0xe0000L);
                case 101:
                    return JjMoveStringLiteralDfa2_2(active0, 0x8a00L);
                case 102:
                    if ((active0 & 0x40L) != 0L)
                        return JjStopAtPos(1, 6);
                    break;
                case 110:
                    return JjMoveStringLiteralDfa2_2(active0, 0x2000L);
                case 111:
                    return JjMoveStringLiteralDfa2_2(active0, 0x104080L);
                case 114:
                    return JjMoveStringLiteralDfa2_2(active0, 0x10400L);
                case 120:
                    return JjMoveStringLiteralDfa2_2(active0, 0x1100L);
                default:
                    return 2;
            }
            return 2;
        }

        private int JjMoveStringLiteralDfa2_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 2;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 2;
            }
            switch ((int) CurChar)
            {
                case 99:
                    return JjMoveStringLiteralDfa3_2(active0, 0x1000L);
                case 101:
                    return JjMoveStringLiteralDfa3_2(active0, 0x10000L);
                case 105:
                    return JjMoveStringLiteralDfa3_2(active0, 0x100100L);
                case 106:
                    return JjMoveStringLiteralDfa3_2(active0, 0xe0000L);
                case 108:
                    return JjMoveStringLiteralDfa3_2(active0, 0xa00L);
                case 112:
                    return JjMoveStringLiteralDfa3_2(active0, 0x8000L);
                case 115:
                    return JjMoveStringLiteralDfa3_2(active0, 0x2000L);
                case 116:
                    if ((active0 & 0x80L) != 0L)
                        return JjStopAtPos(2, 7);
                    break;
                case 117:
                    return JjMoveStringLiteralDfa3_2(active0, 0x400L);
                case 118:
                    return JjMoveStringLiteralDfa3_2(active0, 0x4000L);
                default:
                    return 3;
            }
            return 3;
        }

        private int JjMoveStringLiteralDfa3_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 3;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 3;
            }
            switch ((int) CurChar)
            {
                case 97:
                    return JjMoveStringLiteralDfa4_2(active0, 0x10800L);
                case 101:
                    if ((active0 & 0x4000L) != 0L)
                        return JjStopAtPos(3, 14);
                    return JjMoveStringLiteralDfa4_2(active0, 0x2200L);
                case 105:
                    return JjMoveStringLiteralDfa4_2(active0, 0x1000L);
                case 108:
                    return JjMoveStringLiteralDfa4_2(active0, 0x8000L);
                case 110:
                    return JjMoveStringLiteralDfa4_2(active0, 0x100400L);
                case 111:
                    return JjMoveStringLiteralDfa4_2(active0, 0xe0000L);
                case 115:
                    return JjMoveStringLiteralDfa4_2(active0, 0x100L);
                default:
                    return 4;
            }
        }

        private int JjMoveStringLiteralDfa4_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 4;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 4;
            }
            switch ((int) CurChar)
            {
                case 97:
                    return JjMoveStringLiteralDfa5_2(active0, 0x8000L);
                case 98:
                    return JjMoveStringLiteralDfa5_2(active0, 0x800L);
                case 100:
                    return JjMoveStringLiteralDfa5_2(active0, 0x100000L);
                case 101:
                    if ((active0 & 0x400L) != 0L)
                        return JjStopAtPos(4, 10);
                    break;
                case 105:
                    return JjMoveStringLiteralDfa5_2(active0, 0xe0000L);
                case 114:
                    return JjMoveStringLiteralDfa5_2(active0, 0x2000L);
                case 115:
                    return JjMoveStringLiteralDfa5_2(active0, 0x1000L);
                case 116:
                    return JjMoveStringLiteralDfa5_2(active0, 0x10300L);
                default:
                    return 5;
            }
            return 5;
        }

        private int JjMoveStringLiteralDfa5_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 5;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 5;
            }
            switch ((int) CurChar)
            {
                case 99:
                    return JjMoveStringLiteralDfa6_2(active0, 0x8000L);
                case 101:
                    if ((active0 & 0x200L) != 0L)
                        return JjStopAtPos(5, 9);
                    else if ((active0 & 0x1000L) != 0L)
                        return JjStopAtPos(5, 12);
                    return JjMoveStringLiteralDfa6_2(active0, 0x110800L);
                case 110:
                    if ((active0 & 0x20000L) != 0L)
                    {
                        jjmatchedKind = 17;
                        jjmatchedPos = 5;
                    }
                    return JjMoveStringLiteralDfa6_2(active0, 0xc0000L);
                case 115:
                    if ((active0 & 0x100L) != 0L)
                        return JjStopAtPos(5, 8);
                    break;
                case 116:
                    if ((active0 & 0x2000L) != 0L)
                        return JjStopAtPos(5, 13);
                    break;
                default:
                    return 6;
            }
            return 6;
        }

        private int JjMoveStringLiteralDfa6_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 6;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 6;
            }
            switch ((int) CurChar)
            {
                case 70:
                    if ((active0 & 0x80000L) != 0L)
                        return JjStopAtPos(6, 19);
                    break;
                case 72:
                    if ((active0 & 0x40000L) != 0L)
                        return JjStopAtPos(6, 18);
                    break;
                case 83:
                    return JjMoveStringLiteralDfa7_2(active0, 0x10000L);
                case 101:
                    if ((active0 & 0x8000L) != 0L)
                        return JjStopAtPos(6, 15);
                    break;
                case 108:
                    if ((active0 & 0x800L) != 0L)
                        return JjStopAtPos(6, 11);
                    break;
                case 120:
                    if ((active0 & 0x100000L) != 0L)
                        return JjStopAtPos(6, 20);
                    break;
                default:
                    return 7;
            }
            return 7;
        }

        private int JjMoveStringLiteralDfa7_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 7;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 7;
            }
            switch ((int) CurChar)
            {
                case 117:
                    return JjMoveStringLiteralDfa8_2(active0, 0x10000L);
                default:
                    return 8;
            }
        }

        private int JjMoveStringLiteralDfa8_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 8;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 8;
            }
            switch ((int) CurChar)
            {
                case 98:
                    return JjMoveStringLiteralDfa9_2(active0, 0x10000L);
                default:
                    return 9;
            }
        }

        private int JjMoveStringLiteralDfa9_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 9;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 9;
            }
            switch ((int) CurChar)
            {
                case 116:
                    return JjMoveStringLiteralDfa10_2(active0, 0x10000L);
                default:
                    return 10;
            }
        }

        private int JjMoveStringLiteralDfa10_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 10;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 10;
            }
            switch ((int) CurChar)
            {
                case 114:
                    return JjMoveStringLiteralDfa11_2(active0, 0x10000L);
                default:
                    return 11;
            }
        }

        private int JjMoveStringLiteralDfa11_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 11;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 11;
            }
            switch ((int) CurChar)
            {
                case 101:
                    return JjMoveStringLiteralDfa12_2(active0, 0x10000L);
                default:
                    return 12;
            }
        }

        private int JjMoveStringLiteralDfa12_2(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return 12;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return 12;
            }
            switch ((int) CurChar)
            {
                case 101:
                    if ((active0 & 0x10000L) != 0L)
                        return JjStopAtPos(12, 16);
                    break;
                default:
                    return 13;
            }
            return 13;
        }

        private int JjStopStringLiteralDfa_0(int pos, long active0)
        {
            switch (pos)
            {
                default:
                    return -1;
            }
        }

        private int JjStartNfa_0(int pos, long active0)
        {
            return JjMoveNfa_0(JjStopStringLiteralDfa_0(pos, active0), pos + 1);
        }

        private int JjMoveStringLiteralDfa0_0()
        {
            switch ((int) CurChar)
            {
                case 9:
                    return JjStartNfaWithStates_0(0, 3, 13);
                case 41:
                    return JjStopAtPos(0, 32);
                case 91:
                    return JjStopAtPos(0, 5);
                case 93:
                    return JjStopAtPos(0, 22);
                default:
                    return JjMoveNfa_0(0, 0);
            }
        }

        private int JjStartNfaWithStates_0(int pos, int kind, int state)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return pos + 1;
            }
            return JjMoveNfa_0(state, pos + 1);
        }

        private static readonly long[] JjbitVec0 =
        {
            unchecked ((long) 0xfffffffffffffffeL), unchecked ((long) 0xffffffffffffffffL),
            unchecked ((long) 0xffffffffffffffffL), unchecked ((long) 0xffffffffffffffffL)
        };

        private static readonly long[] JjbitVec2 =
        {
            0x0L, 0x0L, unchecked ((long) 0xffffffffffffffffL), unchecked ((long) 0xffffffffffffffffL)
        };

        private int JjMoveNfa_0(int startState, int curPos)
        {
            int startsAt = 0;
            jjnewStateCnt = 58;
            int i = 1;
            jjstateSet[0] = startState;
            int kind = 0x7fffffff;
            for (;;)
            {
                if (++jjround == 0x7fffffff)
                    ReInitRounds();
                if (CurChar < 64)
                {
                    var l = 1L << CurChar;
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                                if ((unchecked((long) 0xfbfffcfeffffdbffL) & l) != 0L)
                                {
                                    if (kind > 30)
                                        kind = 30;
                                    {
                                        JjCheckNAdd(13);
                                    }
                                }
                                else if (CurChar == 40)
                                {
                                    if (kind > 31)
                                        kind = 31;
                                    {
                                        JjCheckNAdd(15);
                                    }
                                }
                                if ((0x240000000000L & l) != 0L)
                                {
                                    if (kind > 25)
                                        kind = 25;
                                    {
                                        JjCheckNAdd(1);
                                    }
                                }
                                else if (CurChar == 47)
                                {
                                    JjCheckNAddStates(0, 5);
                                }
                                else if (CurChar == 35)
                                {
                                    JjAddStates(6, 7);
                                }
                                else if (CurChar == 36)
                                {
                                    JjAddStates(8, 9);
                                }
                                else if (CurChar == 62)
                                {
                                    JjCheckNAddTwoStates(3, 4);
                                }
                                if (CurChar == 35)
                                {
                                    JjCheckNAdd(12);
                                }
                                break;
                            case 1:
                                if ((0x23ff240000000000L & l) == 0L)
                                    break;
                                if (kind > 25)
                                    kind = 25;
                            {
                                JjCheckNAdd(1);
                            }
                                break;
                            case 2:
                                if (CurChar == 62)
                                {
                                    JjCheckNAddTwoStates(3, 4);
                                }
                                break;
                            case 3:
                                if (CurChar == 45)
                                {
                                    JjCheckNAdd(4);
                                }
                                break;
                            case 4:
                                if ((0x3ff000000000000L & l) == 0L)
                                    break;
                                if (kind > 26)
                                    kind = 26;
                            {
                                JjCheckNAdd(4);
                            }
                                break;
                            case 6:
                                if ((unchecked((long) 0xffffffffffffdbffL) & l) != 0L)
                                {
                                    JjAddStates(10, 12);
                                }
                                break;
                            case 11:
                                if (CurChar == 35)
                                {
                                    JjCheckNAdd(12);
                                }
                                break;
                            case 12:
                                if ((0x3ff000000000000L & l) == 0L)
                                    break;
                                if (kind > 29)
                                    kind = 29;
                            {
                                JjCheckNAdd(12);
                            }
                                break;
                            case 13:
                                if ((unchecked((long) 0xfbfffcfeffffdbffL) & l) == 0L)
                                    break;
                                if (kind > 30)
                                    kind = 30;
                            {
                                JjCheckNAdd(13);
                            }
                                break;
                            case 14:
                                if (CurChar != 40)
                                    break;
                                if (kind > 31)
                                    kind = 31;
                            {
                                JjCheckNAdd(15);
                            }
                                break;
                            case 15:
                                if ((unchecked((long) 0xf3fffcfa00000000L) & l) == 0L)
                                    break;
                                if (kind > 31)
                                    kind = 31;
                            {
                                JjCheckNAdd(15);
                            }
                                break;
                            case 16:
                                if (CurChar == 36)
                                {
                                    JjAddStates(8, 9);
                                }
                                break;
                            case 17:
                                if (CurChar == 45 && kind > 26)
                                    kind = 26;
                                break;
                            case 18:
                                if (CurChar == 43 && kind > 26)
                                    kind = 26;
                                break;
                            case 19:
                                if (CurChar == 35)
                                {
                                    JjAddStates(6, 7);
                                }
                                break;
                            case 36:
                                if (CurChar == 47)
                                {
                                    JjCheckNAddStates(0, 5);
                                }
                                break;
                            case 37:
                                if (CurChar == 47)
                                {
                                    JjCheckNAddStates(13, 15);
                                }
                                break;
                            case 39:
                                if ((unchecked((long) 0xffffffffffffdbffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(13, 15);
                                }
                                break;
                            case 40:
                                if (CurChar == 47)
                                {
                                    JjCheckNAddStates(16, 18);
                                }
                                break;
                            case 41:
                                if (CurChar == 61)
                                {
                                    JjCheckNAdd(42);
                                }
                                break;
                            case 42:
                                if ((0x3ff000000000000L & l) == 0L)
                                    break;
                                if (kind > 24)
                                    kind = 24;
                            {
                                JjCheckNAddStates(19, 22);
                            }
                                break;
                            case 43:
                                if (CurChar == 37)
                                {
                                    JjCheckNAdd(44);
                                }
                                break;
                            case 44:
                                if ((0x3ff000000000000L & l) == 0L)
                                    break;
                                if (kind > 24)
                                    kind = 24;
                            {
                                JjCheckNAddStates(23, 26);
                            }
                                break;
                            case 45:
                            case 49:
                                if (CurChar == 47)
                                {
                                    JjCheckNAddStates(27, 29);
                                }
                                break;
                            case 46:
                                if ((unchecked((long) 0xffffffffffffdbffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(27, 29);
                                }
                                break;
                            case 47:
                                if (CurChar != 47)
                                    break;
                                if (kind > 24)
                                    kind = 24;
                            {
                                JjCheckNAddStates(16, 18);
                            }
                                break;
                            case 52:
                                if ((unchecked((long) 0xffffffffffffdbffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(30, 32);
                                }
                                break;
                            case 53:
                                if (CurChar == 47 && kind > 27)
                                    kind = 27;
                                break;
                            case 55:
                                if (CurChar == 47)
                                {
                                    JjCheckNAddStates(30, 32);
                                }
                                break;
                            default:
                                break;
                        }
                    } while (i != startsAt);
                }
                else if (CurChar < 128)
                {
                    //var l = 1L << (curChar & 077);
                    var l = 1L << (CurChar%64);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                                if ((unchecked((long) 0xffffffffd7ffffffL) & l) != 0L)
                                {
                                    if (kind > 30)
                                        kind = 30;
                                    {
                                        JjCheckNAdd(13);
                                    }
                                }
                                if ((0x7fffffe07fffffeL & l) != 0L)
                                {
                                    if (kind > 25)
                                        kind = 25;
                                    {
                                        JjCheckNAdd(1);
                                    }
                                }
                                else if (CurChar == 124)
                                {
                                    JjCheckNAddStates(10, 12);
                                }
                                break;
                            case 1:
                                if ((0x17fffffe87ffffffL & l) == 0L)
                                    break;
                                if (kind > 25)
                                    kind = 25;
                            {
                                JjCheckNAdd(1);
                            }
                                break;
                            case 5:
                            case 9:
                                if (CurChar == 124)
                                {
                                    JjCheckNAddStates(10, 12);
                                }
                                break;
                            case 6:
                                if ((unchecked((long) 0xefffffffefffffffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(10, 12);
                                }
                                break;
                            case 7:
                                if (CurChar == 124 && kind > 28)
                                    kind = 28;
                                break;
                            case 8:
                                if (CurChar == 92)
                                {
                                    JjAddStates(33, 34);
                                }
                                break;
                            case 10:
                                if (CurChar == 92)
                                {
                                    JjCheckNAddStates(10, 12);
                                }
                                break;
                            case 13:
                                if ((unchecked((long) 0xffffffffd7ffffffL) & l) == 0L)
                                    break;
                                if (kind > 30)
                                    kind = 30;
                            {
                                JjCheckNAdd(13);
                            }
                                break;
                            case 15:
                                if ((0x7fffffffffffffffL & l) == 0L)
                                    break;
                                if (kind > 31)
                                    kind = 31;
                                jjstateSet[jjnewStateCnt++] = 15;
                                break;
                            case 20:
                                if (CurChar == 116 && kind > 23)
                                    kind = 23;
                                break;
                            case 21:
                            case 29:
                                if (CurChar == 115)
                                {
                                    JjCheckNAdd(20);
                                }
                                break;
                            case 22:
                                if (CurChar == 111)
                                    jjstateSet[jjnewStateCnt++] = 21;
                                break;
                            case 23:
                                if (CurChar == 109)
                                    jjstateSet[jjnewStateCnt++] = 22;
                                break;
                            case 24:
                                if (CurChar == 116)
                                    jjstateSet[jjnewStateCnt++] = 23;
                                break;
                            case 25:
                                if (CurChar == 104)
                                    jjstateSet[jjnewStateCnt++] = 24;
                                break;
                            case 26:
                                if (CurChar == 103)
                                    jjstateSet[jjnewStateCnt++] = 25;
                                break;
                            case 27:
                                if (CurChar == 105)
                                    jjstateSet[jjnewStateCnt++] = 26;
                                break;
                            case 28:
                                if (CurChar == 114)
                                    jjstateSet[jjnewStateCnt++] = 27;
                                break;
                            case 30:
                                if (CurChar == 111)
                                    jjstateSet[jjnewStateCnt++] = 29;
                                break;
                            case 31:
                                if (CurChar == 109)
                                    jjstateSet[jjnewStateCnt++] = 30;
                                break;
                            case 32:
                                if (CurChar == 116)
                                    jjstateSet[jjnewStateCnt++] = 31;
                                break;
                            case 33:
                                if (CurChar == 102)
                                    jjstateSet[jjnewStateCnt++] = 32;
                                break;
                            case 34:
                                if (CurChar == 101)
                                    jjstateSet[jjnewStateCnt++] = 33;
                                break;
                            case 35:
                                if (CurChar == 108)
                                    jjstateSet[jjnewStateCnt++] = 34;
                                break;
                            case 38:
                                if (CurChar == 92)
                                    jjstateSet[jjnewStateCnt++] = 37;
                                break;
                            case 39:
                            {
                                JjAddStates(13, 15);
                            }
                                break;
                            case 42:
                                if ((0x7fffffe87fffffeL & l) == 0L)
                                    break;
                                if (kind > 24)
                                    kind = 24;
                            {
                                JjCheckNAddStates(19, 22);
                            }
                                break;
                            case 44:
                                if ((0x7fffffe87fffffeL & l) == 0L)
                                    break;
                                if (kind > 24)
                                    kind = 24;
                            {
                                JjCheckNAddStates(23, 26);
                            }
                                break;
                            case 46:
                                if ((unchecked((long) 0xffffffffcfffffffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(27, 29);
                                }
                                break;
                            case 48:
                                if (CurChar == 92)
                                {
                                    JjAddStates(35, 37);
                                }
                                break;
                            case 50:
                                if (CurChar == 93)
                                {
                                    JjCheckNAddStates(27, 29);
                                }
                                break;
                            case 51:
                                if (CurChar == 92)
                                {
                                    JjCheckNAddStates(27, 29);
                                }
                                break;
                            case 52:
                                if ((unchecked((long) 0xffffffffcfffffffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(30, 32);
                                }
                                break;
                            case 54:
                                if (CurChar == 92)
                                {
                                    JjAddStates(38, 40);
                                }
                                break;
                            case 56:
                                if (CurChar == 93)
                                {
                                    JjCheckNAddStates(30, 32);
                                }
                                break;
                            case 57:
                                if (CurChar == 92)
                                {
                                    JjCheckNAddStates(30, 32);
                                }
                                break;
                            default:
                                break;
                        }
                    } while (i != startsAt);
                }
                else
                {
                    int hiByte = (CurChar >> 8);
                    int i1 = hiByte >> 6;
                    //long l1 = 1L << (hiByte & 077);
                    long l1 = 1L << (hiByte%64);
                    int i2 = (CurChar & 0xff) >> 6;
                    //long l2 = 1L << (curChar & 077);
                    long l2 = 1L << (CurChar%64);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                            case 13:
                                if (!JjCanMove_0(hiByte, i1, i2, l1, l2))
                                    break;
                                if (kind > 30)
                                    kind = 30;
                            {
                                JjCheckNAdd(13);
                            }
                                break;
                            case 6:
                                if (JjCanMove_0(hiByte, i1, i2, l1, l2))
                                {
                                    JjAddStates(10, 12);
                                }
                                break;
                            case 39:
                                if (JjCanMove_0(hiByte, i1, i2, l1, l2))
                                {
                                    JjAddStates(13, 15);
                                }
                                break;
                            case 46:
                                if (JjCanMove_0(hiByte, i1, i2, l1, l2))
                                {
                                    JjAddStates(27, 29);
                                }
                                break;
                            case 52:
                                if (JjCanMove_0(hiByte, i1, i2, l1, l2))
                                {
                                    JjAddStates(30, 32);
                                }
                                break;
                            default:
                                if (i1 == 0 || l1 == 0 || i2 == 0 || l2 == 0) break;
                                else break;
                        }
                    } while (i != startsAt);
                }
                if (kind != 0x7fffffff)
                {
                    jjmatchedKind = kind;
                    jjmatchedPos = curPos;
                    kind = 0x7fffffff;
                }
                ++curPos;
                if ((i = jjnewStateCnt) == (startsAt = 58 - (jjnewStateCnt = startsAt)))
                    return curPos;
                try
                {
                    CurChar = InputStream.ReadChar();
                }
                catch ( IOException e)
                {
                    return curPos;
                }
            }
        }

        private int KjStopStringLiteralDfa_1(int pos, long active0)
        {
            switch (pos)
            {
                default:
                    return -1;
            }
        }

        private int JjStartNfa_1(int pos, long active0)
        {
            return JjMoveNfa_1(KjStopStringLiteralDfa_1(pos, active0), pos + 1);
        }

        private int JjMoveStringLiteralDfa0_1()
        {
            switch ((int) CurChar)
            {
                case 9:
                    return JjStartNfaWithStates_1(0, 3, 0);
                case 91:
                    return JjStopAtPos(0, 5);
                default:
                    return JjMoveNfa_1(0, 0);
            }
        }

        private int JjStartNfaWithStates_1(int pos, int kind, int state)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch ( IOException e)
            {
                return pos + 1;
            }
            return JjMoveNfa_1(state, pos + 1);
        }

        private int JjMoveNfa_1(int startState, int curPos)
        {
            int startsAt = 0;
            jjnewStateCnt = 1;
            int i = 1;
            jjstateSet[0] = startState;
            int kind = 0x7fffffff;
            for (;;)
            {
                if (++jjround == 0x7fffffff)
                    ReInitRounds();
                if (CurChar < 64)
                {
                    var l = 1L << CurChar;
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                                if ((unchecked((long) 0xfbfffcfeffffdbffL) & l) == 0L)
                                    break;
                                kind = 21;
                                jjstateSet[jjnewStateCnt++] = 0;
                                break;
                            default:
                                break;
                        }
                    } while (i != startsAt);
                }
                else if (CurChar < 128)
                {
                    //var l = 1L << (curChar & 077);
                    var l = 1L << (CurChar%64);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                                if ((unchecked ((long) 0xffffffffd7ffffffL) & l) == 0L)
                                    break;
                                kind = 21;
                                jjstateSet[jjnewStateCnt++] = 0;
                                break;
                            default:
                                break;
                        }
                    } while (i != startsAt);
                }
                else
                {
                    int hiByte = (CurChar >> 8);
                    int i1 = hiByte >> 6;
                    //long l1 = 1L << (hiByte & 077);
                    long l1 = 1L << (hiByte%64);
                    int i2 = (CurChar & 0xff) >> 6;
                    //long l2 = 1L << (curChar & 077);
                    long l2 = 1L << (CurChar%64);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                                if (!JjCanMove_0(hiByte, i1, i2, l1, l2))
                                    break;
                                if (kind > 21)
                                    kind = 21;
                                jjstateSet[jjnewStateCnt++] = 0;
                                break;
                            default:
                                if (i1 == 0 || l1 == 0 || i2 == 0 || l2 == 0) break;
                                else break;
                        }
                    } while (i != startsAt);
                }
                if (kind != 0x7fffffff)
                {
                    jjmatchedKind = kind;
                    jjmatchedPos = curPos;
                    kind = 0x7fffffff;
                }
                ++curPos;
                if ((i = jjnewStateCnt) == (startsAt = 1 - (jjnewStateCnt = startsAt)))
                    return curPos;
                try
                {
                    CurChar = InputStream.ReadChar();
                }
                catch ( IOException e)
                {
                    return curPos;
                }
            }
        }

        private static readonly int[] JjNextStates =
        {
            38, 39, 40, 52, 53, 54, 28, 35, 17, 18, 6, 7, 8, 38, 39, 40,
            41, 43, 45, 41, 42, 43, 45, 41, 43, 44, 45, 46, 47, 48, 52, 53,
            54, 9, 10, 49, 50, 51, 55, 56, 57,
        };

        private static bool JjCanMove_0(int hiByte, int i1, int i2, long l1, long l2)
        {
            switch (hiByte)
            {
                case 0:
                    return ((JjbitVec2[i2] & l2) != 0L);
                default:
                    if ((JjbitVec0[i1] & l1) != 0L)
                        return true;
                    return false;
            }
        }


        // literal (java) values
        /*public static readonly string[] jjstrLiteralImages =
        {
            "", null, null, null, null, "\x133", "\x151\x146", "\x156\x157\x164",
            "\x145\x170\x151\x163\x164\x163", "\x144\x145\x154\x145\x164\x145", "\x160\x162\x165\x156\x145",
            "\x162\x145\x154\x141\x142\x145\x154", "\x145\x170\x143\x151\x163\x145", "\x151\x156\x163\x145\x162\x164", "\x155\x157\x166\x145",
            "\x162\x145\x160\x154\x141\x143\x145", "\x143\x162\x145\x141\x164\x145\x123\x165\x142\x164\x162\x145\x145",
            "\x141\x144\x152\x157\x151\x156", "\x141\x144\x152\x157\x151\x156\x110", "\x141\x144\x152\x157\x151\x156\x106",
            "\x143\x157\x151\x156\x144\x145\x170", null, "\x135", null, null, null, null, null, null, null, null, null, "\x51",
        };*/

        /// <summary>Token literal values</summary>
        public static readonly string[] JjStrLiteralImages =
        {
            "", null, null, null, null, "[", "if", "not",
            "exists", "delete", "prune",
            "relabel", "excise", "insert", "move",
            "replace", "createSubtree",
            "adjoin", "adjoinH", "adjoinF",
            "coindex", null, "]", null, null, null, null, null, null, null, null, null, ")",
        };

        protected Token JjFillToken()
        {
            string im = JjStrLiteralImages[jjmatchedKind];
            string curTokenImage = (im == null) ? InputStream.GetImage() : im;
            int beginLine = InputStream.GetBeginLine();
            int beginColumn = InputStream.GetBeginColumn();
            int endLine = InputStream.GetEndLine();
            int endColumn = InputStream.GetEndColumn();
            Token t = Token.NewToken(jjmatchedKind, curTokenImage);

            t.BeginLine = beginLine;
            t.EndLine = endLine;
            t.BeginColumn = beginColumn;
            t.EndColumn = endColumn;

            return t;
        }


        /// <summary>Get the next Token</summary>
        public Token GetNextToken()
        {
            int curPos = 0;

            //EOFLoop :
            for (;;)
            {
                beginning_eof_loop:
                {
                    Token matchedToken;
                    try
                    {
                        CurChar = InputStream.BeginToken();
                    }
                    catch ( Exception e)
                    {
                        if (e is IOException || e is ObjectDisposedException)
                        {
                            jjmatchedKind = 0;
                            jjmatchedPos = -1;
                            matchedToken = JjFillToken();
                            return matchedToken;
                        }
                        throw e;
                    }

                    switch (curLexState)
                    {
                        case 0:
                            try
                            {
                                InputStream.Backup(0);
                                while (CurChar <= 32 && (0x100002400L & (1L << CurChar)) != 0L)
                                    CurChar = InputStream.BeginToken();
                            }
                            catch ( IOException e1)
                            {
                                //continue EOFLoop;
                                goto beginning_eof_loop;
                            }
                            jjmatchedKind = 0x7fffffff;
                            jjmatchedPos = 0;
                            curPos = JjMoveStringLiteralDfa0_0();
                            break;
                        case 1:
                            try
                            {
                                InputStream.Backup(0);
                                while (CurChar <= 32 && (0x100002400L & (1L << CurChar)) != 0L)
                                    CurChar = InputStream.BeginToken();
                            }
                            catch ( IOException e1)
                            {
                                //continue EOFLoop;
                                goto beginning_eof_loop;
                            }
                            jjmatchedKind = 0x7fffffff;
                            jjmatchedPos = 0;
                            curPos = JjMoveStringLiteralDfa0_1();
                            break;
                        case 2:
                            try
                            {
                                InputStream.Backup(0);
                                while (CurChar <= 32 && (0x100002600L & (1L << CurChar)) != 0L)
                                    CurChar = InputStream.BeginToken();
                            }
                            catch ( IOException e1)
                            {
                                //continue EOFLoop;
                                goto beginning_eof_loop;
                            }
                            jjmatchedKind = 0x7fffffff;
                            jjmatchedPos = 0;
                            curPos = JjMoveStringLiteralDfa0_2();
                            break;
                    }
                    if (jjmatchedKind != 0x7fffffff)
                    {
                        if (jjmatchedPos + 1 < curPos)
                            InputStream.Backup(curPos - jjmatchedPos - 1);
                        //if ((jjtoToken[jjmatchedKind >> 6] & (1L << (jjmatchedKind & 077))) != 0L)
                        if ((JjtoToken[jjmatchedKind >> 6] & (1L << (jjmatchedKind%64))) != 0L)
                        {
                            matchedToken = JjFillToken();
                            if (JjnewLexState[jjmatchedKind] != -1)
                                curLexState = JjnewLexState[jjmatchedKind];
                            return matchedToken;
                        }
                        else
                        {
                            if (JjnewLexState[jjmatchedKind] != -1)
                                curLexState = JjnewLexState[jjmatchedKind];
                            //continue EOFLoop;
                            goto beginning_eof_loop;
                        }
                    }
                    int errorLine = InputStream.GetEndLine();
                    int errorColumn = InputStream.GetEndColumn();
                    string errorAfter = null;
                    bool eofSeen = false;
                    try
                    {
                        InputStream.ReadChar();
                        InputStream.Backup(1);
                    }
                    catch ( IOException e1)
                    {
                        eofSeen = true;
                        errorAfter = curPos <= 1 ? "" : InputStream.GetImage();
                        if (CurChar == '\n' || CurChar == '\r')
                        {
                            errorLine++;
                            errorColumn = 0;
                        }
                        else
                            errorColumn++;
                    }
                    if (!eofSeen)
                    {
                        InputStream.Backup(1);
                        errorAfter = curPos <= 1 ? "" : InputStream.GetImage();
                    }
                    throw new TokenMgrException(eofSeen, curLexState, errorLine, errorColumn, errorAfter, CurChar,
                        TokenMgrException.LexError);
                }
            }
        }

        private void JjCheckNAdd(int state)
        {
            if (jjrounds[state] != jjround)
            {
                jjstateSet[jjnewStateCnt++] = state;
                jjrounds[state] = jjround;
            }
        }

        private void JjAddStates(int start, int end)
        {
            do
            {
                jjstateSet[jjnewStateCnt++] = JjNextStates[start];
            } while (start++ != end);
        }

        private void JjCheckNAddTwoStates(int state1, int state2)
        {
            JjCheckNAdd(state1);
            JjCheckNAdd(state2);
        }

        private void JjCheckNAddStates(int start, int end)
        {
            do
            {
                JjCheckNAdd(JjNextStates[start]);
            } while (start++ != end);
        }

        /// <summary>Constructor</summary>
        public TsurgeonParserTokenManager(SimpleCharStream stream)
        {

            if (SimpleCharStream.staticFlag)
                throw new Exception("ERROR: Cannot use a static CharStream class with a non-static lexical analyzer.");

            InputStream = stream;
        }

        /// <summary>Constructor</summary>
        public TsurgeonParserTokenManager(SimpleCharStream stream, int lexState)
        {
            ReInit(stream);
            SwitchTo(lexState);
        }

        /// <summary>Reinitialise parser</summary>
        public void ReInit(SimpleCharStream stream)
        {
            jjmatchedPos = jjnewStateCnt = 0;
            curLexState = DefaultLexState;
            InputStream = stream;
            ReInitRounds();
        }

        private void ReInitRounds()
        {
            int i;
            jjround = 0x80000001;
            for (i = 58; i-- > 0;)
                jjrounds[i] = 0x80000000;
        }

        /// <summary>Reinitialise parser</summary>
        public void ReInit(SimpleCharStream stream, int lexState)
        {
            ReInit(stream);
            SwitchTo(lexState);
        }

        /// <summary>Switch to specified lex state</summary>
        public void SwitchTo(int lexState)
        {
            if (lexState >= 3 || lexState < 0)
                throw new TokenMgrException(
                    "Error: Ignoring invalid lexical state : " + lexState + ". State unchanged.",
                    TokenMgrException.InvalidLexicalState);
            else
                curLexState = lexState;
        }
        
        /// <summary>
        /// Lexer state names.
        /// </summary>
        public static readonly string[] LexStateNames =
        {
            "OPERATION",
            "CONDITIONAL",
            "DEFAULT",
        };

        /// <summary>
        /// Lex State array
        /// </summary>
        public static readonly int[] JjnewLexState =
        {
            -1, -1, -1, -1, -1, 2, -1, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,
        };

        private static readonly long[] JjtoToken =
        {
            0x1ffffffe1L,
        };

        private static readonly long[] JjtoSkip =
        {
            0x1eL,
        };

    }

}