using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /** Provides a Unicode-aware plain whitespace tokenizer.  This tokenizer separates words
 *  based on whitespace, including Unicode spaces such as the CJK ideographic space as well
 *  as traditional Unix whitespace characters.  It can optionally separate out and return
 *  newline characters, again recognizing all Unicode newline sequences.
 *  Designed to be called by <code>WhitespaceTokenizer</code>.
 *
 *  @author Roger Levy
 *  @author Christopher Manning
 */
    public class WhitespaceLexer
    {
        /** This character denotes the end of file */
  public static readonly int YYEOF = -1;

  /** initial size of the lookahead buffer */
  private static readonly int ZZ_BUFFERSIZE = 16384;

  /** lexical states */
  public static readonly int YYINITIAL = 0;

  /**
   * ZZ_LEXSTATE[l] is the state in the DFA for the lexical state l
   * ZZ_LEXSTATE[l+1] is the state in the DFA for the lexical state l
   *                  at the beginning of a line
   * l is of the form l = 2*k, k a non negative integer
   */
  private static readonly int[] ZZ_LEXSTATE = { 0, 0 };

  /**
   * Translates characters to character classes
   */
  /*private static readonly String ZZ_CMAP_PACKED =
    "\11\0\1\4\1\2\2\3\1\1\22\0\1\4\144\0\1\3\32\0"+
    "\1\4\u15df\0\1\4\u018d\0\1\4\u07f3\0\12\4\34\0\1\3\1\3"+
    "\5\0\1\4\57\0\2\4\u0f9f\0\1\4\ucfff\0";*/

  /**
   * Translates characters to character classes
   */
  //private static readonly char [] ZZ_CMAP = zzUnpackCMap(ZZ_CMAP_PACKED);

  /**
   * Translates DFA states to action switch labels.
   */
  //private static readonly int [] ZZ_ACTION = zzUnpackAction();

  /*private static readonly String ZZ_ACTION_PACKED_0 =
    "\1\0\1\1\2\2\1\3";*/

  /*private static int [] zzUnpackAction() {
    int [] result = new int[5];
    int offset = 0;
    offset = zzUnpackAction(ZZ_ACTION_PACKED_0, offset, result);
    return result;
  }*/

  private static int zzUnpackAction(String packed, int offset, int [] result) {
    int i = 0;       /* index in packed string  */
    int j = offset;  /* index in unpacked array */
    int l = packed.Length;
    while (i < l) {
      int count = packed[i++];
      int value = packed[i++];
      do result[j++] = value; while (--count > 0);
    }
    return j;
  }


  /**
   * Translates a state to a row index in the transition table
   */
  //private static readonly int [] ZZ_ROWMAP = zzUnpackRowMap();

  /*private static readonly String ZZ_ROWMAP_PACKED_0 =
    "\0\0\0\5\0\12\0\17\0\24";*/

  /*private static int [] zzUnpackRowMap() {
    int [] result = new int[5];
    int offset = 0;
    offset = zzUnpackRowMap(ZZ_ROWMAP_PACKED_0, offset, result);
    return result;
  }*/

  private static int zzUnpackRowMap(String packed, int offset, int [] result) {
    int i = 0;  /* index in packed string  */
    int j = offset;  /* index in unpacked array */
    int l = packed.Length;
    while (i < l) {
      int high = packed[i++] << 16;
      result[j++] = high | packed[i++];
    }
    return j;
  }

  /**
   * The transition table of the DFA
   */
  //private static readonly int [] ZZ_TRANS = zzUnpackTrans();

  /*private static readonly String ZZ_TRANS_PACKED_0 =
    "\1\2\1\3\2\4\1\5\1\2\6\0\1\4\13\0"+
    "\1\5";*/

  /*private static int [] zzUnpackTrans() {
    int [] result = new int[25];
    int offset = 0;
    offset = zzUnpackTrans(ZZ_TRANS_PACKED_0, offset, result);
    return result;
  }*/

  private static int zzUnpackTrans(String packed, int offset, int [] result) {
    int i = 0;       /* index in packed string  */
    int j = offset;  /* index in unpacked array */
    int l = packed.Length;
    while (i < l) {
      int count = packed[i++];
      int value = packed[i++];
      value--;
      do result[j++] = value; while (--count > 0);
    }
    return j;
  }


  /* error codes */
  private static readonly int ZZ_UNKNOWN_ERROR = 0;
  private static readonly int ZZ_NO_MATCH = 1;
  private static readonly int ZZ_PUSHBACK_2BIG = 2;

  /* error messages for the codes above */
  private static readonly String[] ZZ_ERROR_MSG = {
    "Unkown internal scanner error",
    "Error: could not match input",
    "Error: pushback value was too large"
  };

  /**
   * ZZ_ATTRIBUTE[aState] contains the attributes of state <code>aState</code>
   */
  //private static readonly int [] ZZ_ATTRIBUTE = zzUnpackAttribute();

  /*private static readonly String ZZ_ATTRIBUTE_PACKED_0 =
    "\1\0\2\1\1\11\1\1";*/

  /*private static int [] zzUnpackAttribute() {
    int [] result = new int[5];
    int offset = 0;
    offset = zzUnpackAttribute(ZZ_ATTRIBUTE_PACKED_0, offset, result);
    return result;
  }*/

  private static int zzUnpackAttribute(String packed, int offset, int [] result) {
    int i = 0;       /* index in packed string  */
    int j = offset;  /* index in unpacked array */
    int l = packed.Length;
    while (i < l) {
      int count = packed[i++];
      int value = packed[i++];
      do result[j++] = value; while (--count > 0);
    }
    return j;
  }

  /** the input device */
  private TextReader zzReader;

  /** the current state of the DFA */
  private int zzState;

  /** the current lexical state */
  private int zzLexicalState = YYINITIAL;

  /** this buffer contains the current text to be matched and is
      the source of the yytext() string */
  private char[] zzBuffer = new char[ZZ_BUFFERSIZE];

  /** the textposition at the last accepting state */
  private int zzMarkedPos;

  /** the current text position in the buffer */
  private int zzCurrentPos;

  /** startRead marks the beginning of the yytext() string in the buffer */
  private int zzStartRead;

  /** endRead marks the last character in the buffer, that has been read
      from input */
  private int zzEndRead;

  /** number of newlines encountered up to the start of the matched text */
  private int yyline;

  /** the number of characters up to the start of the matched text */
  private int yychar;

  /**
   * the number of characters from the last newline up to the start of the
   * matched text
   */
  private int yycolumn;

  /**
   * zzAtBOL == true <=> the scanner is currently at the beginning of a line
   */
  private bool zzAtBOL = true;

  /** zzAtEOF == true <=> the scanner is at the EOF */
  private bool zzAtEOF;

  /** denotes if the user-EOF-code has already been executed */
  private bool zzEOFDone;

  /* user code: */
/**
 * See: http://www.w3.org/TR/newline on Web newline chars: NEL, LS, PS.
   See: http://unicode.org/reports/tr13/tr13-9.html and
   http://www.unicode.org/unicode/reports/tr18/#Line_Boundaries
   for Unicode conventions,
   including other separators (vertical tab and form feed).
   <br>
   We do not interpret the zero width joiner/non-joiner (U+200C,
   U+200D) as white spaces.
   <br>
   No longer %standalone.  See WhitespaceTokenizer for a main method.
 */

  public WhitespaceLexer(TextReader r, LexedTokenFactory<object> tf):this(r){
    this.tokenFactory = tf;
  }

  private LexedTokenFactory<object> tokenFactory;

  public static readonly String NEWLINE = "\n";


  /**
   * Creates a new scanner
   * There is also a java.io.InputStream version of this constructor.
   *
   * @param   in  the java.io.Reader to read input from.
   */
  WhitespaceLexer(TextReader input) {
    this.zzReader = input;
  }

  /**
   * Creates a new scanner.
   * There is also java.io.Reader version of this constructor.
   *
   * @param   in  the java.io.Inputstream to read input from.
   */
  WhitespaceLexer(Stream input):
    this(new StreamReader(input, Encoding.UTF8)){}

  /**
   * Unpacks the compressed character translation table.
   *
   * @param packed   the packed character translation table
   * @return         the unpacked character translation table
   */
  private static char[] zzUnpackCMap(String packed) {
    char [] map = new char[0x10000];
    int i = 0;  /* index in packed string  */
    int j = 0;  /* index in unpacked array */
    while (i < 54) {
      int  count = packed[i++];
      char value = packed[i++];
      do map[j++] = value; while (--count > 0);
    }
    return map;
  }


  /**
   * Refills the input buffer.
   *
   * @return      <code>false</code>, iff there was new input.
   *
   * @exception   java.io.IOException  if any I/O-Error occurs
   */
  private bool zzRefill() /*throws java.io.IOException */{

    /* first: make room (if you can) */
    if (zzStartRead > 0) {
      Array.Copy(zzBuffer, zzStartRead,
                       zzBuffer, 0,
                       zzEndRead-zzStartRead);

      /* translate stored positions */
      zzEndRead-= zzStartRead;
      zzCurrentPos-= zzStartRead;
      zzMarkedPos-= zzStartRead;
      zzStartRead = 0;
    }

    /* is the buffer big enough? */
    if (zzCurrentPos >= zzBuffer.Length) {
      /* if not: blow it up */
      char[] newBuffer = new char[zzCurrentPos*2];
      Array.Copy(zzBuffer, 0, newBuffer, 0, zzBuffer.Length);
      zzBuffer = newBuffer;
    }

    /* readonlyly: fill the buffer with new input */
    int numRead = zzReader.Read(zzBuffer, zzEndRead,
                                            zzBuffer.Length-zzEndRead);

    if (numRead > 0) {
      zzEndRead+= numRead;
      return false;
    }
    // unlikely but not impossible: read 0 characters, but not at end of stream
    if (numRead == 0) {
      int c = zzReader.Read();
      if (c == -1) {
        return true;
      } else {
        zzBuffer[zzEndRead++] = (char) c;
        return false;
      }
    }

    // numRead < 0
    return true;
  }


  /**
   * Closes the input stream.
   */
  public /*readonly */void yyclose() /*throws java.io.IOException*/ {
    zzAtEOF = true;            /* indicate end of file */
    zzEndRead = zzStartRead;  /* invalidate buffer    */

    if (zzReader != null)
      zzReader.Close();
  }


  /**
   * Resets the scanner to read from a new input stream.
   * Does not close the old reader.
   *
   * All internal variables are reset, the old input stream
   * <b>cannot</b> be reused (internal buffer is discarded and lost).
   * Lexical state is set to <tt>ZZ_INITIAL</tt>.
   *
   * Internal scan buffer is resized down to its initial length, if it has grown.
   *
   * @param reader   the new input stream
   */
  public /*readonly*/ void yyreset(TextReader reader) {
    zzReader = reader;
    zzAtBOL  = true;
    zzAtEOF  = false;
    zzEOFDone = false;
    zzEndRead = zzStartRead = 0;
    zzCurrentPos = zzMarkedPos = 0;
    yyline = yychar = yycolumn = 0;
    zzLexicalState = YYINITIAL;
    if (zzBuffer.Length > ZZ_BUFFERSIZE)
      zzBuffer = new char[ZZ_BUFFERSIZE];
  }


  /**
   * Returns the current lexical state.
   */
  public /*readonly*/ int yystate() {
    return zzLexicalState;
  }


  /**
   * Enters a new lexical state
   *
   * @param newState the new lexical state
   */
  public /*readonly*/ void yybegin(int newState) {
    zzLexicalState = newState;
  }


  /**
   * Returns the text matched by the current regular expression.
   */
  public /*readonly*/ String yytext() {
    return new String( zzBuffer, zzStartRead, zzMarkedPos-zzStartRead );
  }


  /**
   * Returns the character at position <tt>pos</tt> from the
   * matched text.
   *
   * It is equivalent to yytext()[pos), but faster
   *
   * @param pos the position of the character to fetch.
   *            A value from 0 to yyLength-1.
   *
   * @return the character at position pos
   */
  public /*readonly*/ char yycharat(int pos) {
    return zzBuffer[zzStartRead+pos];
  }


  /**
   * Returns the length of the matched text region.
   */
  public /*readonly*/ int yyLength() {
    return zzMarkedPos-zzStartRead;
  }


  /**
   * Reports an error that occured while scanning.
   *
   * In a wellformed scanner (no or only correct usage of
   * yypushback(int) and a match-all fallback rule) this method
   * will only be called with things that "Can't Possibly Happen".
   * If this method is called, something is seriously wrong
   * (e.g. a JFlex bug producing a faulty scanner etc.).
   *
   * Usual syntax/scanner level error handling should be done
   * in error fallback rules.
   *
   * @param   errorCode  the code of the errormessage to display
   */
  private void zzScanError(int errorCode) {
    String message;
    try {
      message = ZZ_ERROR_MSG[errorCode];
    }
    catch (IndexOutOfRangeException e) {
      message = ZZ_ERROR_MSG[ZZ_UNKNOWN_ERROR];
    }

    throw new Exception(message);
  }


  /**
   * Pushes the specified amount of characters back into the input stream.
   *
   * They will be read again by then next call of the scanning method
   *
   * @param number  the number of characters to be read again.
   *                This number must not be greater than yyLength!
   */
  public void yypushback(int number)  {
    if ( number > yyLength() )
      zzScanError(ZZ_PUSHBACK_2BIG);

    zzMarkedPos -= number;
  }


  /**
   * Resumes scanning until the next regular expression is matched,
   * the end of input is encountered or an I/O-Error occurs.
   *
   * @return      the next token
   * @exception   java.io.IOException  if any I/O-Error occurs
   */
  public Object next() /*throws java.io.IOException*/ {
    int zzInput;
    int zzAction;

    // cached fields:
    int zzCurrentPosL;
    int zzMarkedPosL;
    int zzEndReadL = zzEndRead;
    char [] zzBufferL = zzBuffer;
    char [] zzCMapL = ZZ_CMAP;

    int [] zzTransL = ZZ_TRANS;
    int [] zzRowMapL = ZZ_ROWMAP;
    int [] zzAttrL = ZZ_ATTRIBUTE;

    while (true) {
      zzMarkedPosL = zzMarkedPos;

      yychar+= zzMarkedPosL-zzStartRead;

      zzAction = -1;

      zzCurrentPosL = zzCurrentPos = zzStartRead = zzMarkedPosL;

      zzState = ZZ_LEXSTATE[zzLexicalState];

      // set up zzAction for empty match case:
      int zzAttributes = zzAttrL[zzState];
      if ( (zzAttributes & 1) == 1 ) {
        zzAction = zzState;
      }


      zzForAction: {
        while (true) {

          if (zzCurrentPosL < zzEndReadL)
            zzInput = zzBufferL[zzCurrentPosL++];
          else if (zzAtEOF) {
            zzInput = YYEOF;
            //break zzForAction;
goto post_zzForAction;
          }
          else {
            // store back cached positions
            zzCurrentPos  = zzCurrentPosL;
            zzMarkedPos   = zzMarkedPosL;
            bool eof = zzRefill();
            // get translated positions and possibly new buffer
            zzCurrentPosL  = zzCurrentPos;
            zzMarkedPosL   = zzMarkedPos;
            zzBufferL      = zzBuffer;
            zzEndReadL     = zzEndRead;
            if (eof) {
              zzInput = YYEOF;
              //break zzForAction;
goto post_zzForAction;
            }
            else {
              zzInput = zzBufferL[zzCurrentPosL++];
            }
          }
          int zzNext = zzTransL[ zzRowMapL[zzState] + zzCMapL[zzInput] ];
            if (zzNext == -1)
            {
                //break zzForAction;
            goto post_zzForAction;
            }
          zzState = zzNext;

          zzAttributes = zzAttrL[zzState];
          if ( (zzAttributes & 1) == 1 ) {
            zzAction = zzState;
            zzMarkedPosL = zzCurrentPosL;
              if ((zzAttributes & 8) == 8)
              {
                  //break zzForAction;
goto post_zzForAction;
              }
          }

        }
      }
    post_zzForAction:{}

      // store back cached position
      zzMarkedPos = zzMarkedPosL;

      switch (zzAction < 0 ? zzAction : ZZ_ACTION[zzAction]) {
        case 1:
          { return tokenFactory.makeToken(yytext(), yychar, yyLength());
          }
        case 4: break;
        case 2:
          { return tokenFactory.makeToken(NEWLINE, yychar, yyLength());
          }
        case 5: break;
        case 3:
          {
          }
              break;
          case 6: break;
        default:
          if (zzInput == YYEOF && zzStartRead == zzCurrentPos) {
            zzAtEOF = true;
              {
                return null;
              }
          }
          else {
            zzScanError(ZZ_NO_MATCH);
          }
              break;
      }
    }
  }
    }
}
