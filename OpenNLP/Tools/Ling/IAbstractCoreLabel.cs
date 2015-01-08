using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Ling
{
    public interface IAbstractCoreLabel : ILabel, IHasWord, IHasIndex, IHasTag, IHasLemma, IHasOffset, ITypesafeMap
    {
        string Ner();

        void SetNer(string ner);

        string OriginalText();

        void SetOriginalText(string originalText);

        string GetString(Type key);
    }
}