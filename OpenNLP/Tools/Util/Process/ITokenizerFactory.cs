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
    /// - public static TokenizerFactory NewTokenizerFactory();
    /// - public static TokenizerFactory NewWordTokenizerFactory(string options);
    /// 
    /// These are expected by certain JavaNLP code (e.g., LexicalizedParser),
    /// which wants to produce a TokenizerFactory by reflection.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    /// <typeparam name="T">The type of the tokens returned by the Tokenizer</typeparam>
    public interface ITokenizerFactory<T> : IIteratorFromReaderFactory<T>
    {
        ITokenizer<T> GetTokenizer(TextReader r);

        ITokenizer<T> GetTokenizer(TextReader r, string extraOptions);

        void SetOptions(string options);
    }
}