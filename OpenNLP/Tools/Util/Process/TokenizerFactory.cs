using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.ObjectBank;

namespace OpenNLP.Tools.Util.Process
{
    public interface TokenizerFactory<T> : IteratorFromReaderFactory<T>
    {
        Tokenizer<T> getTokenizer(TextReader r);

        Tokenizer<T> getTokenizer(TextReader r, string extraOptions);

        void setOptions(string options);
    }
}