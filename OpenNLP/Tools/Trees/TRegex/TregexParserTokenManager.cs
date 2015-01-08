using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex
{
    public class TregexParserTokenManager : TregexParserConstants
    {
        
        private int JjStopStringLiteralDfa_0(int pos, long active0)
        {
            switch (pos)
            {
                case 0:
                    if ((active0 & 0x8L) != 0L)
                        return 6;
                    if ((active0 & 0x100000L) != 0L)
                        return 1;
                    if ((active0 & 0x20L) != 0L)
                    {
                        jjmatchedKind = 4;
                        return 26;
                    }
                    return -1;
                case 1:
                    if ((active0 & 0x20L) != 0L)
                    {
                        if (jjmatchedPos == 0)
                        {
                            jjmatchedKind = 4;
                            jjmatchedPos = 0;
                        }
                        return -1;
                    }
                    return -1;
                case 2:
                    if ((active0 & 0x20L) != 0L)
                    {
                        if (jjmatchedPos == 0)
                        {
                            jjmatchedKind = 4;
                            jjmatchedPos = 0;
                        }
                        return -1;
                    }
                    return -1;
                default:
                    return -1;
            }
        }

        private int JjStartNfa_0(int pos, long active0)
        {
            return JjMoveNfa_0(JjStopStringLiteralDfa_0(pos, active0), pos + 1);
        }

        private int JjStopAtPos(int pos, int kind)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            return pos + 1;
        }

        private int JjMoveStringLiteralDfa0_0()
        {
            switch (CurChar)
            {
                case (char) 9:
                    return JjStartNfaWithStates_0(0, 3, 6);
                case (char) 10:
                    return JjStopAtPos(0, 13);
                case (char) 33:
                    return JjStopAtPos(0, 16);
                case (char) 35:
                    return JjStopAtPos(0, 18);
                case (char) 37:
                    return JjStopAtPos(0, 19);
                case (char) 38:
                    return JjStopAtPos(0, 22);
                case (char) 40:
                    return JjStopAtPos(0, 14);
                case (char) 41:
                    return JjStopAtPos(0, 15);
                case (char) 59:
                    return JjStopAtPos(0, 27);
                case (char) 60:
                    return JjMoveStringLiteralDfa1_0(0x20L);
                case (char) 61:
                    return JjStartNfaWithStates_0(0, 20, 1);
                case (char) 63:
                    return JjStopAtPos(0, 23);
                case (char) 64:
                    return JjStopAtPos(0, 17);
                case (char) 91:
                    return JjStopAtPos(0, 24);
                case (char) 93:
                    return JjStopAtPos(0, 25);
                case (char) 95:
                    return JjMoveStringLiteralDfa1_0(0x200L);
                case (char) 123:
                    return JjStopAtPos(0, 26);
                case (char) 124:
                    return JjStopAtPos(0, 12);
                case (char) 125:
                    return JjStopAtPos(0, 28);
                case (char) 126:
                    return JjStopAtPos(0, 21);
                default:
                    return JjMoveNfa_0(0, 0);
            }
        }

        private int JjMoveStringLiteralDfa1_0(long active0)
        {
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch (IOException e)
            {
                JjStopStringLiteralDfa_0(0, active0);
                return 1;
            }
            switch (CurChar)
            {
                case (char) 46:
                    return JjMoveStringLiteralDfa2_0(active0, 0x20L);
                case (char) 95:
                    if ((active0 & 0x200L) != 0L)
                        return JjStopAtPos(1, 9);
                    break;
                default:
                    break;
            }
            return JjStartNfa_0(0, active0);
        }

        private int JjMoveStringLiteralDfa2_0(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return JjStartNfa_0(0, old0);
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch (IOException e)
            {
                JjStopStringLiteralDfa_0(1, active0);
                return 2;
            }
            switch (CurChar)
            {
                case (char) 46:
                    return JjMoveStringLiteralDfa3_0(active0, 0x20L);
                default:
                    break;
            }
            return JjStartNfa_0(1, active0);
        }

        private int JjMoveStringLiteralDfa3_0(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return JjStartNfa_0(1, old0);
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch (IOException e)
            {
                JjStopStringLiteralDfa_0(2, active0);
                return 3;
            }
            switch (CurChar)
            {
                case (char) 46:
                    if ((active0 & 0x20L) != 0L)
                        return JjStopAtPos(3, 5);
                    break;
                default:
                    break;
            }
            return JjStartNfa_0(2, active0);
        }

        private int JjStartNfaWithStates_0(int pos, int kind, int state)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            try
            {
                CurChar = InputStream.ReadChar();
            }
            catch (IOException e)
            {
                return pos + 1;
            }
            return JjMoveNfa_0(state, pos + 1);
        }

        private static readonly long[] JjbitVec0 =
        {
            unchecked((long) 0xfffffffffffffffeL), unchecked((long) 0xffffffffffffffffL),
            unchecked((long) 0xffffffffffffffffL), unchecked((long) 0xffffffffffffffffL)
        };

        private static readonly long[] JjbitVec2 =
        {
            0x0L, 0x0L, unchecked((long) 0xffffffffffffffffL), unchecked((long) 0xffffffffffffffffL)
        };

        private int JjMoveNfa_0(int startState, int curPos)
        {
            int startsAt = 0;
            jjnewStateCnt = 42;
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
                            case 26:
                                if (CurChar == 43)
                                {
                                    if (kind > 6)
                                        kind = 6;
                                }
                                else if (CurChar == 61)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (CurChar == 60)
                                {
                                    JjCheckNAdd(33);
                                }
                                else if (CurChar == 35)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (CurChar == 58)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (CurChar == 45)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (CurChar == 44)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(31);
                                }
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(29);
                                }
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(16);
                                }
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(20);
                                }
                                if (CurChar == 60)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                break;
                            case 0:
                                if ((0x2c84ffffdbffL & l) != 0L)
                                {
                                    if (kind > 8)
                                        kind = 8;
                                    {
                                        JjCheckNAdd(6);
                                    }
                                }
                                else if ((0x3ff000000000000L & l) != 0L)
                                {
                                    if (kind > 7)
                                        kind = 7;
                                    {
                                        JjCheckNAdd(4);
                                    }
                                }
                                else if ((0x5400501000000000L & l) != 0L)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (CurChar == 47)
                                {
                                    JjCheckNAddStates(0, 2);
                                }
                                else if (CurChar == 61)
                                {
                                    JjCheckNAdd(1);
                                }
                                if (CurChar == 62)
                                {
                                    JjCheckNAddStates(3, 14);
                                }
                                else if (CurChar == 60)
                                {
                                    JjCheckNAddStates(15, 27);
                                }
                                else if (CurChar == 44)
                                {
                                    JjCheckNAddTwoStates(20, 23);
                                }
                                else if (CurChar == 46)
                                {
                                    JjCheckNAddTwoStates(18, 23);
                                }
                                else if (CurChar == 36)
                                {
                                    JjCheckNAddStates(28, 35);
                                }
                                else if (CurChar == 45)
                                {
                                    JjCheckNAdd(4);
                                }
                                break;
                            case 1:
                                if (CurChar == 61 && kind > 4)
                                    kind = 4;
                                break;
                            case 2:
                                if (CurChar == 61)
                                {
                                    JjCheckNAdd(1);
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
                                if (kind > 7)
                                    kind = 7;
                            {
                                JjCheckNAdd(4);
                            }
                                break;
                            case 5:
                                if ((0x2c84ffffdbffL & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                JjCheckNAdd(6);
                            }
                                break;
                            case 6:
                                if ((0xbff2c84ffffdbffL & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                JjCheckNAdd(6);
                            }
                                break;
                            case 7:
                            case 8:
                                if (CurChar == 47)
                                {
                                    JjCheckNAddStates(0, 2);
                                }
                                break;
                            case 10:
                                if ((unchecked((long) 0xffff7fffffffdbffL) & l) != 0L)
                                {
                                    JjCheckNAddStates(0, 2);
                                }
                                break;
                            case 11:
                                if (CurChar == 47 && kind > 10)
                                    kind = 10;
                                break;
                            case 13:
                                if (CurChar == 36)
                                {
                                    JjCheckNAddStates(28, 35);
                                }
                                break;
                            case 14:
                                if (CurChar == 43 && kind > 4)
                                    kind = 4;
                                break;
                            case 15:
                                if (CurChar == 43)
                                {
                                    JjCheckNAdd(14);
                                }
                                break;
                            case 16:
                                if (CurChar == 45 && kind > 4)
                                    kind = 4;
                                break;
                            case 17:
                                if (CurChar == 45)
                                {
                                    JjCheckNAdd(16);
                                }
                                break;
                            case 18:
                                if (CurChar == 46 && kind > 4)
                                    kind = 4;
                                break;
                            case 19:
                                if (CurChar == 46)
                                {
                                    JjCheckNAdd(18);
                                }
                                break;
                            case 20:
                                if (CurChar == 44 && kind > 4)
                                    kind = 4;
                                break;
                            case 21:
                                if (CurChar == 44)
                                {
                                    JjCheckNAdd(20);
                                }
                                break;
                            case 22:
                                if (CurChar == 46)
                                {
                                    JjCheckNAddTwoStates(18, 23);
                                }
                                break;
                            case 23:
                                if (CurChar == 43 && kind > 6)
                                    kind = 6;
                                break;
                            case 24:
                                if (CurChar == 44)
                                {
                                    JjCheckNAddTwoStates(20, 23);
                                }
                                break;
                            case 25:
                                if (CurChar == 60)
                                {
                                    JjCheckNAddStates(15, 27);
                                }
                                break;
                            case 27:
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(20);
                                }
                                break;
                            case 28:
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(16);
                                }
                                break;
                            case 30:
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(29);
                                }
                                break;
                            case 31:
                                if (CurChar == 58 && kind > 4)
                                    kind = 4;
                                break;
                            case 32:
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(31);
                                }
                                break;
                            case 33:
                                if (CurChar == 35 && kind > 4)
                                    kind = 4;
                                break;
                            case 34:
                                if (CurChar == 60)
                                {
                                    JjCheckNAdd(33);
                                }
                                break;
                            case 35:
                                if (CurChar == 62)
                                {
                                    JjCheckNAddStates(3, 14);
                                }
                                break;
                            case 36:
                                if (CurChar == 62 && kind > 4)
                                    kind = 4;
                                break;
                            case 37:
                                if (CurChar == 62)
                                {
                                    JjCheckNAdd(20);
                                }
                                break;
                            case 38:
                                if (CurChar == 62)
                                {
                                    JjCheckNAdd(16);
                                }
                                break;
                            case 39:
                                if (CurChar == 62)
                                {
                                    JjCheckNAdd(29);
                                }
                                break;
                            case 40:
                                if (CurChar == 62)
                                {
                                    JjCheckNAdd(31);
                                }
                                break;
                            case 41:
                                if (CurChar == 62)
                                {
                                    JjCheckNAdd(33);
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
                            case 26:
                            case 29:
                                if (CurChar == 96 && kind > 4)
                                    kind = 4;
                                break;
                            case 0:
                                if ((unchecked((long) 0x87ffffff57fffffeL) & l) != 0L)
                                {
                                    if (kind > 8)
                                        kind = 8;
                                    {
                                        JjCheckNAdd(6);
                                    }
                                }
                                if ((0x7fffffe07fffffeL & l) != 0L)
                                {
                                    if (kind > 11)
                                        kind = 11;
                                    {
                                        JjCheckNAdd(12);
                                    }
                                }
                                break;
                            case 5:
                                if ((unchecked((long) 0x87ffffff57fffffeL) & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                JjCheckNAdd(6);
                            }
                                break;
                            case 6:
                                if ((unchecked((long) 0xbfffffffd7fffffeL) & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                JjCheckNAdd(6);
                            }
                                break;
                            case 9:
                                if (CurChar == 92)
                                    jjstateSet[jjnewStateCnt++] = 8;
                                break;
                            case 10:
                            {
                                JjAddStates(0, 2);
                            }
                                break;
                            case 12:
                                if ((0x7fffffe07fffffeL & l) == 0L)
                                    break;
                                if (kind > 11)
                                    kind = 11;
                            {
                                JjCheckNAdd(12);
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
                    //var l1 = 1L << (hiByte & 077);
                    var l1 = 1L << (hiByte%64);
                    int i2 = (CurChar & 0xff) >> 6;
                    //var l2 = 1L << (curChar & 077);
                    var l2 = 1L << (CurChar%64);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                            case 6:
                                if (!JjCanMove_0(hiByte, i1, i2, l1, l2))
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                JjCheckNAdd(6);
                            }
                                break;
                            case 10:
                                if (JjCanMove_0(hiByte, i1, i2, l1, l2))
                                {
                                    JjAddStates(0, 2);
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
                if ((i = jjnewStateCnt) == (startsAt = 42 - (jjnewStateCnt = startsAt)))
                    return curPos;
                try
                {
                    CurChar = InputStream.ReadChar();
                }
                catch (IOException e)
                {
                    return curPos;
                }
            }
        }

        private static readonly int[] JjnextStates =
        {
            9, 10, 11, 36, 37, 38, 20, 16, 29, 39, 31, 40, 33, 41, 23, 26,
            27, 28, 20, 16, 29, 30, 31, 32, 33, 34, 1, 23, 15, 17, 14, 16,
            18, 19, 20, 21,
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
        
        /*public static readonly string[] jjstrLiteralImages =
        {
            "", null, null, null, null, @"\74\56\56\56", null, null, null, @"\137\137", null,
            null, @"\174", @"\12", @"\50", @"\51", @"\41", @"\100", @"\43", @"\45", @"\75", @"\176",
            @"\46", @"\77", @"\133", @"\135", @"\173", @"\73", @"\175",
        };*/

        /// <summary>
        /// Token literal values
        /// </summary>
        public static readonly string[] JjstrLiteralImages =
        {
            "", null, null, null, null, "<...", null, null, null, "__", null,
            null, "|", "\n", "(", ")", "!", "@", "#", "%", "=", "~",
            "&", "?", "[", "]", "{", ";", "}",
        };

        protected Token JjFillToken()
        {
            string im = JjstrLiteralImages[jjmatchedKind];
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

        private int curLexState = 0;
        private const int DefaultLexState = 0;
        private int jjnewStateCnt;
        private uint jjround;
        private int jjmatchedPos;
        private int jjmatchedKind;

        /// <summary>
        /// Get the next Token
        /// </summary>
        public Token GetNextToken()
        {
            Token matchedToken;
            int curPos = 0;

            //EOFLoop :
            for (;;)
            {
                start_EOF_loop:
                {
                }
                try
                {
                    CurChar = InputStream.BeginToken();
                }
                catch (IOException e)
                {
                    jjmatchedKind = 0;
                    jjmatchedPos = -1;
                    matchedToken = JjFillToken();
                    return matchedToken;
                }

                try
                {
                    InputStream.Backup(0);
                    while (CurChar <= 32 && (0x100002000L & (1L << CurChar)) != 0L)
                        CurChar = InputStream.BeginToken();
                }
                catch (IOException e1)
                {
                    //continue EOFLoop;
                    goto start_EOF_loop;
                }
                jjmatchedKind = 0x7fffffff;
                jjmatchedPos = 0;
                curPos = JjMoveStringLiteralDfa0_0();
                if (jjmatchedKind != 0x7fffffff)
                {
                    if (jjmatchedPos + 1 < curPos)
                        InputStream.Backup(curPos - jjmatchedPos - 1);
                    //if ((jjtoToken[jjmatchedKind >> 6] & (1L << (jjmatchedKind & 077))) != 0L)
                    if ((JjtoToken[jjmatchedKind >> 6] & (1L << (jjmatchedKind%64))) != 0L)
                    {
                        matchedToken = JjFillToken();
                        return matchedToken;
                    }
                    else
                    {
                        //continue EOFLoop;
                        goto start_EOF_loop;
                    }
                }
                int error_line = InputStream.GetEndLine();
                int error_column = InputStream.GetEndColumn();
                string error_after = null;
                bool EOFSeen = false;
                try
                {
                    InputStream.ReadChar();
                    InputStream.Backup(1);
                }
                catch (IOException e1)
                {
                    EOFSeen = true;
                    error_after = curPos <= 1 ? "" : InputStream.GetImage();
                    if (CurChar == '\n' || CurChar == '\r')
                    {
                        error_line++;
                        error_column = 0;
                    }
                    else
                        error_column++;
                }
                if (!EOFSeen)
                {
                    InputStream.Backup(1);
                    error_after = curPos <= 1 ? "" : InputStream.GetImage();
                }
                throw new TokenMgrException(EOFSeen, curLexState, error_line, error_column, error_after, CurChar,
                    TokenMgrException.LexError);
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
                jjstateSet[jjnewStateCnt++] = JjnextStates[start];
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
                JjCheckNAdd(JjnextStates[start]);
            } while (start++ != end);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TregexParserTokenManager(SimpleCharStream stream)
        {

            if (SimpleCharStream.staticFlag)
            {
                throw new Exception("ERROR: Cannot use a static CharStream class with a non-static lexical analyzer.");
            }

            InputStream = stream;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TregexParserTokenManager(SimpleCharStream stream, int lexState)
        {
            ReInit(stream);
            SwitchTo(lexState);
        }

        /// <summary>
        /// Reinitialise parser
        /// </summary>
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
            for (i = 42; i-- > 0;)
                jjrounds[i] = 0x80000000;
        }

        /// <summary>
        /// Reinitialise parser
        /// </summary>
        public void ReInit(SimpleCharStream stream, int lexState)
        {
            ReInit(stream);
            SwitchTo(lexState);
        }

        /// <summary>
        /// Switch to specified lex state
        /// </summary>
        public void SwitchTo(int lexState)
        {
            if (lexState >= 1 || lexState < 0)
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
            "DEFAULT",
        };

        private static readonly long[] JjtoToken =
        {
            0x1ffffff1L,
        };

        private static readonly long[] JjtoSkip =
        {
            0xeL,
        };

        protected SimpleCharStream InputStream;

        private readonly uint[] jjrounds = new uint[42];
        private readonly int[] jjstateSet = new int[2*42];


        protected char CurChar;
    }
}