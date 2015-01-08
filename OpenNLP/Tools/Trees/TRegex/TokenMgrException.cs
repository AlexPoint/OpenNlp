using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex
{
    public class TokenMgrException : Exception
    {
        /* Ordinals for various reasons why an Error of this type can be thrown. */

        /// <summary>
        /// Lexical error occurred
        /// </summary>
        public static readonly int LexError = 0;

        /// <summary>
        /// Tried to change to an invalid lexical state
        /// </summary>
        public static readonly int InvalidLexicalState = 2;

        /// <summary>
        /// Detected (and bailed out of) an infinite loop in the token manager.
        /// </summary>
        private static readonly int LoopDetected = 3;

        /// <summary>
        /// Indicates the reason why the exception is thrown. 
        /// It will have one of the above 4 values.
        /// </summary>
        private int errorCode;

        /// <summary>
        /// Replaces unprintable characters by their escaped (or unicode escaped)
        /// equivalents in the given string
        /// </summary>
        protected static string AddEscapes(string str)
        {
            var retval = new StringBuilder();
            char ch;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case (char) 0:
                        continue;
                    case '\b':
                        retval.Append("\\b");
                        continue;
                    case '\t':
                        retval.Append("\\t");
                        continue;
                    case '\n':
                        retval.Append("\\n");
                        continue;
                    case '\f':
                        retval.Append("\\f");
                        continue;
                    case '\r':
                        retval.Append("\\r");
                        continue;
                    case '\"':
                        retval.Append("\\\"");
                        continue;
                    case '\'':
                        retval.Append("\\\'");
                        continue;
                    case '\\':
                        retval.Append("\\\\");
                        continue;
                    default:
                        if ((ch = str[i]) < 0x20 || ch > 0x7e)
                        {
                            string s = "0000" + Convert.ToString(ch, 16);
                            retval.Append("\\u" + s.Substring(s.Length - 4, s.Length));
                        }
                        else
                        {
                            retval.Append(ch);
                        }
                        continue;
                }
            }
            return retval.ToString();
        }

        /// <summary>
        /// Returns a detailed message for the Error when it is thrown by the
        /// token manager to indicate a lexical error.
        /// Note: You can customize the lexical error message by modifying this method.
        /// </summary>
        /// <param name="eofSeen">indicates if EOF caused the lexical error</param>
        /// <param name="lexState">lexical state in which this error occurred</param>
        /// <param name="errorLine">line number when the error occurred</param>
        /// <param name="errorColumn">column number when the error occurred</param>
        /// <param name="errorAfter">prefix that was seen before this error occurred</param>
        /// <param name="curChar">the offending character</param>
        protected static string LexicalError(bool eofSeen, int lexState, int errorLine, int errorColumn,
            string errorAfter, char curChar)
        {
            return ("Lexical error at line " +
                    errorLine + ", column " +
                    errorColumn + ".  Encountered: " +
                    (eofSeen ? "<EOF> " : ("\"" + AddEscapes(curChar.ToString()) + "\"") + " (" + (int) curChar + "), ") +
                    "after : \"" + AddEscapes(errorAfter) + "\"");
        }

        /// <summary>
        /// You can also modify the body of this method to customize your error messages.
        /// For example, cases like LOOP_DETECTED and INVALID_LEXICAL_STATE are not
        /// of end-users concern, so you can return something like :
        ///      "Internal Error : Please file a bug report .... "
        /// from this method for such cases in the release version of your parser.
        /// </summary>
        public string GetMessage()
        {
            return base.Message;
        }

        /* Constructors of various flavors follow. */
        /// <summary>
        /// No arg constructor
        /// </summary>
        public TokenMgrException()
        {
        }

        /// <summary>
        /// Constructor with message and reason
        /// </summary>
        public TokenMgrException(string message, int reason) : base(message)
        {
            errorCode = reason;
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        public TokenMgrException(bool eofSeen, int lexState, int errorLine, int errorColumn, string errorAfter,
            char curChar, int reason) :
                this(LexicalError(eofSeen, lexState, errorLine, errorColumn, errorAfter, curChar), reason)
        {
        }
    }
}