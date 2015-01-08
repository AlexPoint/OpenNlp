using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex
{
    /// <summary>
    /// An implementation of interface CharStream, where the stream is assumed to
    /// contain only ASCII characters (without unicode processing).
    /// </summary>
    public class SimpleCharStream
    {
        /// <summary>
        /// Whether parser is static
        /// </summary>
        public static readonly bool staticFlag = false;
        private int bufsize;
        private int available;
        private int tokenBegin;

        /// <summary>Position in buffer</summary>
        public int bufpos = -1;
        protected int[] bufline;
        protected int[] bufcolumn;

        protected int column = 0;
        protected int line = 1;

        protected bool prevCharIsCR = false;
        protected bool prevCharIsLF = false;

        protected TextReader inputStream;

        protected char[] buffer;
        protected int maxNextCharInd = 0;
        protected int inBuf = 0;
        protected int tabSize = 8;
        protected bool trackLineColumn = true;

        public void SetTabSize(int i)
        {
            tabSize = i;
        }

        public int GetTabSize()
        {
            return tabSize;
        }


        protected void ExpandBuff(bool wrapAround)
        {
            var newbuffer = new char[bufsize + 2048];
            var newbufline = new int[bufsize + 2048];
            var newbufcolumn = new int[bufsize + 2048];

            try
            {
                if (wrapAround)
                {
                    Array.Copy(buffer, tokenBegin, newbuffer, 0, bufsize - tokenBegin);
                    Array.Copy(buffer, 0, newbuffer, bufsize - tokenBegin, bufpos);
                    buffer = newbuffer;

                    Array.Copy(bufline, tokenBegin, newbufline, 0, bufsize - tokenBegin);
                    Array.Copy(bufline, 0, newbufline, bufsize - tokenBegin, bufpos);
                    bufline = newbufline;

                    Array.Copy(bufcolumn, tokenBegin, newbufcolumn, 0, bufsize - tokenBegin);
                    Array.Copy(bufcolumn, 0, newbufcolumn, bufsize - tokenBegin, bufpos);
                    bufcolumn = newbufcolumn;

                    maxNextCharInd = (bufpos += (bufsize - tokenBegin));
                }
                else
                {
                    Array.Copy(buffer, tokenBegin, newbuffer, 0, bufsize - tokenBegin);
                    buffer = newbuffer;

                    Array.Copy(bufline, tokenBegin, newbufline, 0, bufsize - tokenBegin);
                    bufline = newbufline;

                    Array.Copy(bufcolumn, tokenBegin, newbufcolumn, 0, bufsize - tokenBegin);
                    bufcolumn = newbufcolumn;

                    maxNextCharInd = (bufpos -= tokenBegin);
                }
            }
            catch (Exception t)
            {
                throw new SystemException(t.Message);
            }


            bufsize += 2048;
            available = bufsize;
            tokenBegin = 0;
        }

        protected void FillBuff()
        {
            if (maxNextCharInd == available)
            {
                if (available == bufsize)
                {
                    if (tokenBegin > 2048)
                    {
                        bufpos = maxNextCharInd = 0;
                        available = tokenBegin;
                    }
                    else if (tokenBegin < 0)
                        bufpos = maxNextCharInd = 0;
                    else
                        ExpandBuff(false);
                }
                else if (available > tokenBegin)
                    available = bufsize;
                else if ((tokenBegin - available) < 2048)
                    ExpandBuff(true);
                else
                    available = tokenBegin;
            }

            int i;
            try
            {
                //if ((i = inputStream.Read(buffer, maxNextCharInd, available - maxNextCharInd)) == -1)
                if ((i = inputStream.Read(buffer, maxNextCharInd, available - maxNextCharInd)) == 0)
                {
                    inputStream.Close();
                    throw new IOException();
                }
                else
                    maxNextCharInd += i;
                return;
            }
            catch (Exception e)
            {
                if (e is IOException || e is ObjectDisposedException)
                {
                    --bufpos;
                    Backup(0);
                    if (tokenBegin == -1)
                        tokenBegin = bufpos;
                }
                throw e;
            }
        }
        
        public char BeginToken()
        {
            tokenBegin = -1;
            char c = ReadChar();
            tokenBegin = bufpos;

            return c;
        }

        protected void UpdateLineColumn(char c)
        {
            column++;

            if (prevCharIsLF)
            {
                prevCharIsLF = false;
                line += (column = 1);
            }
            else if (prevCharIsCR)
            {
                prevCharIsCR = false;
                if (c == '\n')
                {
                    prevCharIsLF = true;
                }
                else
                    line += (column = 1);
            }

            switch (c)
            {
                case '\r':
                    prevCharIsCR = true;
                    break;
                case '\n':
                    prevCharIsLF = true;
                    break;
                case '\t':
                    column--;
                    column += (tabSize - (column%tabSize));
                    break;
                default:
                    break;
            }

            bufline[bufpos] = line;
            bufcolumn[bufpos] = column;
        }

        /// <summary>Read a character</summary>
        public char ReadChar()
        {
            if (inBuf > 0)
            {
                --inBuf;

                if (++bufpos == bufsize)
                    bufpos = 0;

                return buffer[bufpos];
            }

            if (++bufpos >= maxNextCharInd)
                FillBuff();

            char c = buffer[bufpos];

            UpdateLineColumn(c);
            return c;
        }

        [Obsolete("see GetEndColumn")]
        public int GetColumn()
        {
            return bufcolumn[bufpos];
        }

        [Obsolete("see GetEndLine")]
        public int GetLine()
        {
            return bufline[bufpos];
        }

        /// <summary>
        /// Get token end column number
        /// </summary>
        public int GetEndColumn()
        {
            return bufcolumn[bufpos];
        }

        /// <summary>
        /// Get token end line number
        /// </summary>
        public int GetEndLine()
        {
            return bufline[bufpos];
        }

        /// <summary>
        /// Get token beginning column number
        /// </summary>
        public int GetBeginColumn()
        {
            return bufcolumn[tokenBegin];
        }

        /// <summary>
        /// Get token beginning line number
        /// </summary>
        public int GetBeginLine()
        {
            return bufline[tokenBegin];
        }

        /// <summary>
        /// Backup a number of characters
        /// </summary>
        public void Backup(int amount)
        {

            inBuf += amount;
            if ((bufpos -= amount) < 0)
                bufpos += bufsize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SimpleCharStream( /*java.io.Reader*/ TextReader dstream, int startline,
            int startcolumn, int buffersize)
        {
            inputStream = dstream;
            line = startline;
            column = startcolumn - 1;

            available = bufsize = buffersize;
            buffer = new char[buffersize];
            bufline = new int[buffersize];
            bufcolumn = new int[buffersize];
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.Reader*/ TextReader dstream, int startline,
            int startcolumn) :
                this(dstream, startline, startcolumn, 4096)
        {
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.Reader*/ TextReader dstream) :
            this(dstream, 1, 1, 4096)
        {
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.Reader*/ TextReader dstream, int startline,
            int startcolumn, int buffersize)
        {
            inputStream = dstream;
            line = startline;
            column = startcolumn - 1;

            if (buffer == null || buffersize != buffer.Length)
            {
                available = bufsize = buffersize;
                buffer = new char[buffersize];
                bufline = new int[buffersize];
                bufcolumn = new int[buffersize];
            }
            prevCharIsLF = prevCharIsCR = false;
            tokenBegin = inBuf = maxNextCharInd = 0;
            bufpos = -1;
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.Reader*/ TextReader dstream, int startline,
            int startcolumn)
        {
            ReInit(dstream, startline, startcolumn, 4096);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.Reader*/ TextReader dstream)
        {
            ReInit(dstream, 1, 1, 4096);
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.InputStream*/ Stream dstream, string encoding, int startline,
            int startcolumn, int buffersize) /*throws java.io.UnsupportedEncodingException*/ :
                this(encoding == null
                    ? new /*java.io.InputStreamReader(dstream)*/ StreamReader(dstream)
                    : /*new java.io.InputStreamReader*/new StreamReader(dstream, Encoding.GetEncoding(encoding)),
                    startline, startcolumn, buffersize)
        {
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.InputStream*/ Stream dstream, int startline,
            int startcolumn, int buffersize) :
                this(new /*java.io.InputStreamReader*/ StreamReader(dstream), startline, startcolumn, buffersize)
        {
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.InputStream*/ Stream dstream, string encoding, int startline,
            int startcolumn) /*throws java.io.UnsupportedEncodingException*/ :
                this(dstream, encoding, startline, startcolumn, 4096)
        {
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.InputStream*/ Stream dstream, int startline,
            int startcolumn) :
                this(dstream, startline, startcolumn, 4096)
        {
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.InputStream*/ Stream dstream, string encoding)
            /*throws java.io.UnsupportedEncodingException*/ :
                this(dstream, encoding, 1, 1, 4096)
        {
        }

        /// <summary>Constructor</summary>
        public SimpleCharStream( /*java.io.InputStream*/ Stream dstream) :
            this(dstream, 1, 1, 4096)
        {
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.InputStream*/ Stream dstream, string encoding, int startline,
            int startcolumn, int buffersize) /*throws java.io.UnsupportedEncodingException*/
        {
            ReInit(
                encoding == null
                    ? new /*java.io.InputStreamReader*/ StreamReader(dstream)
                    : new /*java.io.InputStreamReader*/ StreamReader(dstream, Encoding.GetEncoding(encoding)), startline,
                startcolumn, buffersize);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.InputStream*/ Stream dstream, int startline,
            int startcolumn, int buffersize)
        {
            ReInit(new /*java.io.InputStreamReader*/ StreamReader(dstream), startline, startcolumn, buffersize);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.InputStream*/ Stream dstream, string encoding)
            /*throws java.io.UnsupportedEncodingException*/
        {
            ReInit(dstream, encoding, 1, 1, 4096);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.InputStream*/ Stream dstream)
        {
            ReInit(dstream, 1, 1, 4096);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.InputStream*/ Stream dstream, string encoding, int startline,
            int startcolumn) /*throws java.io.UnsupportedEncodingException*/
        {
            ReInit(dstream, encoding, startline, startcolumn, 4096);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit( /*java.io.InputStream*/ Stream dstream, int startline,
            int startcolumn)
        {
            ReInit(dstream, startline, startcolumn, 4096);
        }

        /// <summary>
        /// Get token literal value
        /// </summary>
        public string GetImage()
        {
            if (bufpos >= tokenBegin)
                return new string(buffer, tokenBegin, bufpos - tokenBegin + 1);
            else
                return new string(buffer, tokenBegin, bufsize - tokenBegin) +
                       new string(buffer, 0, bufpos + 1);
        }

        /// <summary>
        /// Get the suffix
        /// </summary>
        public char[] GetSuffix(int len)
        {
            char[] ret = new char[len];

            if ((bufpos + 1) >= len)
                Array.Copy(buffer, bufpos - len + 1, ret, 0, len);
            else
            {
                Array.Copy(buffer, bufsize - (len - bufpos - 1), ret, 0,
                    len - bufpos - 1);
                Array.Copy(buffer, 0, ret, len - bufpos - 1, bufpos + 1);
            }

            return ret;
        }

        /// <summary>
        /// Reset buffer when finished
        /// </summary>
        public void Done()
        {
            buffer = null;
            bufline = null;
            bufcolumn = null;
        }

        /// <summary>
        /// Adjust line and column numbers for the start of a token
        /// </summary>
        public void AdjustBeginLineColumn(int newLine, int newCol)
        {
            int start = tokenBegin;
            int len;

            if (bufpos >= tokenBegin)
            {
                len = bufpos - tokenBegin + inBuf + 1;
            }
            else
            {
                len = bufsize - tokenBegin + bufpos + 1 + inBuf;
            }

            int i = 0, j = 0, k = 0;
            int columnDiff = 0;

            while (i < len && bufline[j = start%bufsize] == bufline[k = ++start%bufsize])
            {
                bufline[j] = newLine;
                int nextColDiff = columnDiff + bufcolumn[k] - bufcolumn[j];
                bufcolumn[j] = newCol + columnDiff;
                columnDiff = nextColDiff;
                i++;
            }

            if (i < len)
            {
                bufline[j] = newLine++;
                bufcolumn[j] = newCol + columnDiff;

                while (i++ < len)
                {
                    if (bufline[j = start%bufsize] != bufline[++start%bufsize])
                        bufline[j] = newLine++;
                    else
                        bufline[j] = newLine;
                }
            }

            line = bufline[j];
            column = bufcolumn[j];
        }

        private bool GetTrackLineColumn()
        {
            return trackLineColumn;
        }

        private void SetTrackLineColumn(bool tlc)
        {
            trackLineColumn = tlc;
        }
    }
}