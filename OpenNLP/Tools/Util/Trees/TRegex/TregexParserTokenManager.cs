using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    public class TregexParserTokenManager : TregexParserConstants
    {

        /*/** Debug output. #1#
  public  java.io.PrintStream debugStream = System.out;
  /** Set debug output. #1#
  public  void setDebugStream(java.io.PrintStream ds) { debugStream = ds; }*/

        private int jjStopStringLiteralDfa_0(int pos, long active0)
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

        private int jjStartNfa_0(int pos, long active0)
        {
            return jjMoveNfa_0(jjStopStringLiteralDfa_0(pos, active0), pos + 1);
        }

        private int jjStopAtPos(int pos, int kind)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            return pos + 1;
        }

        private int jjMoveStringLiteralDfa0_0()
        {
            switch (curChar)
            {
                case (char) 9:
                    return jjStartNfaWithStates_0(0, 3, 6);
                case (char) 10:
                    return jjStopAtPos(0, 13);
                case (char) 33:
                    return jjStopAtPos(0, 16);
                case (char) 35:
                    return jjStopAtPos(0, 18);
                case (char) 37:
                    return jjStopAtPos(0, 19);
                case (char) 38:
                    return jjStopAtPos(0, 22);
                case (char) 40:
                    return jjStopAtPos(0, 14);
                case (char) 41:
                    return jjStopAtPos(0, 15);
                case (char) 59:
                    return jjStopAtPos(0, 27);
                case (char) 60:
                    return jjMoveStringLiteralDfa1_0(0x20L);
                case (char) 61:
                    return jjStartNfaWithStates_0(0, 20, 1);
                case (char) 63:
                    return jjStopAtPos(0, 23);
                case (char) 64:
                    return jjStopAtPos(0, 17);
                case (char) 91:
                    return jjStopAtPos(0, 24);
                case (char) 93:
                    return jjStopAtPos(0, 25);
                case (char) 95:
                    return jjMoveStringLiteralDfa1_0(0x200L);
                case (char) 123:
                    return jjStopAtPos(0, 26);
                case (char) 124:
                    return jjStopAtPos(0, 12);
                case (char) 125:
                    return jjStopAtPos(0, 28);
                case (char) 126:
                    return jjStopAtPos(0, 21);
                default:
                    return jjMoveNfa_0(0, 0);
            }
        }

        private int jjMoveStringLiteralDfa1_0(long active0)
        {
            try
            {
                curChar = input_stream.readChar();
            }
            catch (IOException e)
            {
                jjStopStringLiteralDfa_0(0, active0);
                return 1;
            }
            switch (curChar)
            {
                case (char) 46:
                    return jjMoveStringLiteralDfa2_0(active0, 0x20L);
                case (char) 95:
                    if ((active0 & 0x200L) != 0L)
                        return jjStopAtPos(1, 9);
                    break;
                default:
                    break;
            }
            return jjStartNfa_0(0, active0);
        }

        private int jjMoveStringLiteralDfa2_0(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return jjStartNfa_0(0, old0);
            try
            {
                curChar = input_stream.readChar();
            }
            catch (IOException e)
            {
                jjStopStringLiteralDfa_0(1, active0);
                return 2;
            }
            switch (curChar)
            {
                case (char) 46:
                    return jjMoveStringLiteralDfa3_0(active0, 0x20L);
                default:
                    break;
            }
            return jjStartNfa_0(1, active0);
        }

        private int jjMoveStringLiteralDfa3_0(long old0, long active0)
        {
            if (((active0 &= old0)) == 0L)
                return jjStartNfa_0(1, old0);
            try
            {
                curChar = input_stream.readChar();
            }
            catch (IOException e)
            {
                jjStopStringLiteralDfa_0(2, active0);
                return 3;
            }
            switch (curChar)
            {
                case (char) 46:
                    if ((active0 & 0x20L) != 0L)
                        return jjStopAtPos(3, 5);
                    break;
                default:
                    break;
            }
            return jjStartNfa_0(2, active0);
        }

        private int jjStartNfaWithStates_0(int pos, int kind, int state)
        {
            jjmatchedKind = kind;
            jjmatchedPos = pos;
            try
            {
                curChar = input_stream.readChar();
            }
            catch (IOException e)
            {
                return pos + 1;
            }
            return jjMoveNfa_0(state, pos + 1);
        }

        private static readonly ulong[] jjbitVec0 =
        {
            0xfffffffffffffffeL, 0xffffffffffffffffL, 0xffffffffffffffffL, 0xffffffffffffffffL
        };

        private static readonly ulong[] jjbitVec2 =
        {
            0x0L, 0x0L, 0xffffffffffffffffL, 0xffffffffffffffffL
        };

        private int jjMoveNfa_0(int startState, int curPos)
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
                if (curChar < 64)
                {
                    ulong l = (ulong) 1L << curChar;
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 26:
                                if (curChar == 43)
                                {
                                    if (kind > 6)
                                        kind = 6;
                                }
                                else if (curChar == 61)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (curChar == 60)
                                {
                                    jjCheckNAdd(33);
                                }
                                else if (curChar == 35)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (curChar == 58)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (curChar == 45)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (curChar == 44)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(31);
                                }
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(29);
                                }
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(16);
                                }
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(20);
                                }
                                if (curChar == 60)
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
                                        jjCheckNAdd(6);
                                    }
                                }
                                else if ((0x3ff000000000000L & l) != 0L)
                                {
                                    if (kind > 7)
                                        kind = 7;
                                    {
                                        jjCheckNAdd(4);
                                    }
                                }
                                else if ((0x5400501000000000L & l) != 0L)
                                {
                                    if (kind > 4)
                                        kind = 4;
                                }
                                else if (curChar == 47)
                                {
                                    jjCheckNAddStates(0, 2);
                                }
                                else if (curChar == 61)
                                {
                                    jjCheckNAdd(1);
                                }
                                if (curChar == 62)
                                {
                                    jjCheckNAddStates(3, 14);
                                }
                                else if (curChar == 60)
                                {
                                    jjCheckNAddStates(15, 27);
                                }
                                else if (curChar == 44)
                                {
                                    jjCheckNAddTwoStates(20, 23);
                                }
                                else if (curChar == 46)
                                {
                                    jjCheckNAddTwoStates(18, 23);
                                }
                                else if (curChar == 36)
                                {
                                    jjCheckNAddStates(28, 35);
                                }
                                else if (curChar == 45)
                                {
                                    jjCheckNAdd(4);
                                }
                                break;
                            case 1:
                                if (curChar == 61 && kind > 4)
                                    kind = 4;
                                break;
                            case 2:
                                if (curChar == 61)
                                {
                                    jjCheckNAdd(1);
                                }
                                break;
                            case 3:
                                if (curChar == 45)
                                {
                                    jjCheckNAdd(4);
                                }
                                break;
                            case 4:
                                if ((0x3ff000000000000L & l) == 0L)
                                    break;
                                if (kind > 7)
                                    kind = 7;
                            {
                                jjCheckNAdd(4);
                            }
                                break;
                            case 5:
                                if ((0x2c84ffffdbffL & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                jjCheckNAdd(6);
                            }
                                break;
                            case 6:
                                if ((0xbff2c84ffffdbffL & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                jjCheckNAdd(6);
                            }
                                break;
                            case 7:
                            case 8:
                                if (curChar == 47)
                                {
                                    jjCheckNAddStates(0, 2);
                                }
                                break;
                            case 10:
                                if ((0xffff7fffffffdbffL & l) != 0L)
                                {
                                    jjCheckNAddStates(0, 2);
                                }
                                break;
                            case 11:
                                if (curChar == 47 && kind > 10)
                                    kind = 10;
                                break;
                            case 13:
                                if (curChar == 36)
                                {
                                    jjCheckNAddStates(28, 35);
                                }
                                break;
                            case 14:
                                if (curChar == 43 && kind > 4)
                                    kind = 4;
                                break;
                            case 15:
                                if (curChar == 43)
                                {
                                    jjCheckNAdd(14);
                                }
                                break;
                            case 16:
                                if (curChar == 45 && kind > 4)
                                    kind = 4;
                                break;
                            case 17:
                                if (curChar == 45)
                                {
                                    jjCheckNAdd(16);
                                }
                                break;
                            case 18:
                                if (curChar == 46 && kind > 4)
                                    kind = 4;
                                break;
                            case 19:
                                if (curChar == 46)
                                {
                                    jjCheckNAdd(18);
                                }
                                break;
                            case 20:
                                if (curChar == 44 && kind > 4)
                                    kind = 4;
                                break;
                            case 21:
                                if (curChar == 44)
                                {
                                    jjCheckNAdd(20);
                                }
                                break;
                            case 22:
                                if (curChar == 46)
                                {
                                    jjCheckNAddTwoStates(18, 23);
                                }
                                break;
                            case 23:
                                if (curChar == 43 && kind > 6)
                                    kind = 6;
                                break;
                            case 24:
                                if (curChar == 44)
                                {
                                    jjCheckNAddTwoStates(20, 23);
                                }
                                break;
                            case 25:
                                if (curChar == 60)
                                {
                                    jjCheckNAddStates(15, 27);
                                }
                                break;
                            case 27:
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(20);
                                }
                                break;
                            case 28:
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(16);
                                }
                                break;
                            case 30:
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(29);
                                }
                                break;
                            case 31:
                                if (curChar == 58 && kind > 4)
                                    kind = 4;
                                break;
                            case 32:
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(31);
                                }
                                break;
                            case 33:
                                if (curChar == 35 && kind > 4)
                                    kind = 4;
                                break;
                            case 34:
                                if (curChar == 60)
                                {
                                    jjCheckNAdd(33);
                                }
                                break;
                            case 35:
                                if (curChar == 62)
                                {
                                    jjCheckNAddStates(3, 14);
                                }
                                break;
                            case 36:
                                if (curChar == 62 && kind > 4)
                                    kind = 4;
                                break;
                            case 37:
                                if (curChar == 62)
                                {
                                    jjCheckNAdd(20);
                                }
                                break;
                            case 38:
                                if (curChar == 62)
                                {
                                    jjCheckNAdd(16);
                                }
                                break;
                            case 39:
                                if (curChar == 62)
                                {
                                    jjCheckNAdd(29);
                                }
                                break;
                            case 40:
                                if (curChar == 62)
                                {
                                    jjCheckNAdd(31);
                                }
                                break;
                            case 41:
                                if (curChar == 62)
                                {
                                    jjCheckNAdd(33);
                                }
                                break;
                            default:
                                break;
                        }
                    } while (i != startsAt);
                }
                else if (curChar < 128)
                {
                    ulong l = (ulong) 1L << (curChar & 077);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 26:
                            case 29:
                                if (curChar == 96 && kind > 4)
                                    kind = 4;
                                break;
                            case 0:
                                if ((0x87ffffff57fffffeL & l) != 0L)
                                {
                                    if (kind > 8)
                                        kind = 8;
                                    {
                                        jjCheckNAdd(6);
                                    }
                                }
                                if ((0x7fffffe07fffffeL & l) != 0L)
                                {
                                    if (kind > 11)
                                        kind = 11;
                                    {
                                        jjCheckNAdd(12);
                                    }
                                }
                                break;
                            case 5:
                                if ((0x87ffffff57fffffeL & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                jjCheckNAdd(6);
                            }
                                break;
                            case 6:
                                if ((0xbfffffffd7fffffeL & l) == 0L)
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                jjCheckNAdd(6);
                            }
                                break;
                            case 9:
                                if (curChar == 92)
                                    jjstateSet[jjnewStateCnt++] = 8;
                                break;
                            case 10:
                            {
                                jjAddStates(0, 2);
                            }
                                break;
                            case 12:
                                if ((0x7fffffe07fffffeL & l) == 0L)
                                    break;
                                if (kind > 11)
                                    kind = 11;
                            {
                                jjCheckNAdd(12);
                            }
                                break;
                            default:
                                break;
                        }
                    } while (i != startsAt);
                }
                else
                {
                    int hiByte = (curChar >> 8);
                    int i1 = hiByte >> 6;
                    ulong l1 = (ulong)1L << (hiByte & 077);
                    int i2 = (curChar & 0xff) >> 6;
                    ulong l2 = (ulong)1L << (curChar & 077);
                    do
                    {
                        switch (jjstateSet[--i])
                        {
                            case 0:
                            case 6:
                                if (!jjCanMove_0(hiByte, i1, i2, l1, l2))
                                    break;
                                if (kind > 8)
                                    kind = 8;
                            {
                                jjCheckNAdd(6);
                            }
                                break;
                            case 10:
                                if (jjCanMove_0(hiByte, i1, i2, l1, l2))
                                {
                                    jjAddStates(0, 2);
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
                    curChar = input_stream.readChar();
                }
                catch (IOException e)
                {
                    return curPos;
                }
            }
        }

        private static readonly int[] jjnextStates =
        {
            9, 10, 11, 36, 37, 38, 20, 16, 29, 39, 31, 40, 33, 41, 23, 26,
            27, 28, 20, 16, 29, 30, 31, 32, 33, 34, 1, 23, 15, 17, 14, 16,
            18, 19, 20, 21,
        };

        private static /*readonly*/ bool jjCanMove_0(int hiByte, int i1, int i2, ulong l1, ulong l2)
        {
            switch (hiByte)
            {
                case 0:
                    return ((jjbitVec2[i2] & l2) != 0L);
                default:
                    if ((jjbitVec0[i1] & l1) != 0L)
                        return true;
                    return false;
            }
        }

/** Token literal values. */

        public static readonly String[] jjstrLiteralImages =
        {
            "", null, null, null, null, @"\74\56\56\56", null, null, null, @"\137\137", null,
            null, @"\174", @"\12", @"\50", @"\51", @"\41", @"\100", @"\43", @"\45", @"\75", @"\176",
            @"\46", @"\77", @"\133", @"\135", @"\173", @"\73", @"\175",
        };

        protected Token jjFillToken()
        {
            /*readonly*/
            Token t;
            /*readonly */
            String curTokenImage;
            /*readonly */
            int beginLine;
            /*readonly */
            int endLine;
            /*readonly */
            int beginColumn;
            /*readonly */
            int endColumn;
            String im = jjstrLiteralImages[jjmatchedKind];
            curTokenImage = (im == null) ? input_stream.GetImage() : im;
            beginLine = input_stream.getBeginLine();
            beginColumn = input_stream.getBeginColumn();
            endLine = input_stream.getEndLine();
            endColumn = input_stream.getEndColumn();
            t = Token.newToken(jjmatchedKind, curTokenImage);

            t.beginLine = beginLine;
            t.endLine = endLine;
            t.beginColumn = beginColumn;
            t.endColumn = endColumn;

            return t;
        }

        private int curLexState = 0;
        private int defaultLexState = 0;
        private int jjnewStateCnt;
        private uint jjround;
        private int jjmatchedPos;
        private int jjmatchedKind;

/** Get the next Token. */

        public Token getNextToken()
        {
            Token matchedToken;
            int curPos = 0;

            //EOFLoop :
            for (;;)
            {
            start_EOF_loop:{}
                try
                {
                    curChar = input_stream.BeginToken();
                }
                catch (IOException e)
                {
                    jjmatchedKind = 0;
                    jjmatchedPos = -1;
                    matchedToken = jjFillToken();
                    return matchedToken;
                }

                try
                {
                    input_stream.backup(0);
                    while (curChar <= 32 && (0x100002000L & (1L << curChar)) != 0L)
                        curChar = input_stream.BeginToken();
                }
                catch (IOException e1)
                {
                    //continue EOFLoop;
                    goto start_EOF_loop;
                }
                jjmatchedKind = 0x7fffffff;
                jjmatchedPos = 0;
                curPos = jjMoveStringLiteralDfa0_0();
                if (jjmatchedKind != 0x7fffffff)
                {
                    if (jjmatchedPos + 1 < curPos)
                        input_stream.backup(curPos - jjmatchedPos - 1);
                    if ((jjtoToken[jjmatchedKind >> 6] & (1L << (jjmatchedKind & 077))) != 0L)
                    {
                        matchedToken = jjFillToken();
                        return matchedToken;
                    }
                    else
                    {
                        //continue EOFLoop;
                        goto start_EOF_loop;
                    }
                }
                int error_line = input_stream.getEndLine();
                int error_column = input_stream.getEndColumn();
                String error_after = null;
                bool EOFSeen = false;
                try
                {
                    input_stream.readChar();
                    input_stream.backup(1);
                }
                catch (IOException e1)
                {
                    EOFSeen = true;
                    error_after = curPos <= 1 ? "" : input_stream.GetImage();
                    if (curChar == '\n' || curChar == '\r')
                    {
                        error_line++;
                        error_column = 0;
                    }
                    else
                        error_column++;
                }
                if (!EOFSeen)
                {
                    input_stream.backup(1);
                    error_after = curPos <= 1 ? "" : input_stream.GetImage();
                }
                throw new TokenMgrException(EOFSeen, curLexState, error_line, error_column, error_after, curChar,
                    TokenMgrException.LEXICAL_ERROR);
            }
        }

        private void jjCheckNAdd(int state)
        {
            if (jjrounds[state] != jjround)
            {
                jjstateSet[jjnewStateCnt++] = state;
                jjrounds[state] = jjround;
            }
        }

        private void jjAddStates(int start, int end)
        {
            do
            {
                jjstateSet[jjnewStateCnt++] = jjnextStates[start];
            } while (start++ != end);
        }

        private void jjCheckNAddTwoStates(int state1, int state2)
        {
            jjCheckNAdd(state1);
            jjCheckNAdd(state2);
        }

        private void jjCheckNAddStates(int start, int end)
        {
            do
            {
                jjCheckNAdd(jjnextStates[start]);
            } while (start++ != end);
        }

        /** Constructor. */

        public TregexParserTokenManager(SimpleCharStream stream)
        {

            if (SimpleCharStream.staticFlag)
                throw new Exception("ERROR: Cannot use a static CharStream class with a non-static lexical analyzer.");

            input_stream = stream;
        }

        /** Constructor. */

        public TregexParserTokenManager(SimpleCharStream stream, int lexState)
        {
            ReInit(stream);
            SwitchTo(lexState);
        }

        /** Reinitialise parser. */

        public void ReInit(SimpleCharStream stream)
        {
            jjmatchedPos = jjnewStateCnt = 0;
            curLexState = defaultLexState;
            input_stream = stream;
            ReInitRounds();
        }

        private void ReInitRounds()
        {
            int i;
            jjround = 0x80000001;
            for (i = 42; i-- > 0;)
                jjrounds[i] = 0x80000000;
        }

        /** Reinitialise parser. */

        public void ReInit(SimpleCharStream stream, int lexState)
        {
            ReInit(stream);
            SwitchTo(lexState);
        }

        /** Switch to specified lex state. */

        public void SwitchTo(int lexState)
        {
            if (lexState >= 1 || lexState < 0)
                throw new TokenMgrException("Error: Ignoring invalid lexical state : " + lexState + ". State unchanged.",
                    TokenMgrException.INVALID_LEXICAL_STATE);
            else
                curLexState = lexState;
        }

/** Lexer state names. */

        public static readonly String[] lexStateNames =
        {
            "DEFAULT",
        };

        private static readonly long[] jjtoToken =
        {
            0x1ffffff1L,
        };

        private static readonly long[] jjtoSkip =
        {
            0xeL,
        };

        protected SimpleCharStream input_stream;

        private readonly uint[] jjrounds = new uint[42];
        private readonly int[] jjstateSet = new int[2*42];


        protected char curChar;
    }
}
