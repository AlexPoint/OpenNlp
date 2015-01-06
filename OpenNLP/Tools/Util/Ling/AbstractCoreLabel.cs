using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    public interface AbstractCoreLabel : Label, HasWord, HasIndex, HasTag, HasLemma, HasOffset, TypesafeMap
    {
        string Ner();

        void SetNER(string ner);

        string OriginalText();

        void SetOriginalText(string originalText);

        string GetString(Type key);
    }
}