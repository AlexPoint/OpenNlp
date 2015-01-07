using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.ObjectBank;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// A TokenizerFactory is used to convert a java.io.Reader into a Tokenizer
    /// (an extension of Iterator) over objects of type T represented by the text
    /// in the java.io.Reader.  It's mainly a convenience, since you could cast down anyway.
    /// 
    /// <i>IMPORTANT NOTE:</i>
    /// A TokenizerFactory should also provide two static methods:
    /// {@code public static TokenizerFactory<? extends HasWord> newTokenizerFactory(); }
    /// {@code public static TokenizerFactory<Word> newWordTokenizerFactory(String options); }
    /// 
    /// These are expected by certain JavaNLP code (e.g., LexicalizedParser),
    /// which wants to produce a TokenizerFactory by reflection.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    /// <typeparam name="T">The type of the tokens returned by the Tokenizer</typeparam>
    public interface TokenizerFactory<T> : IteratorFromReaderFactory<T>
    {
        Tokenizer<T> GetTokenizer(TextReader r);

        Tokenizer<T> GetTokenizer(TextReader r, string extraOptions);

        void SetOptions(string options);
    }
}