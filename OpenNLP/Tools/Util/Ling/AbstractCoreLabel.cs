using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    public interface AbstractCoreLabel : Label, HasWord, HasIndex, HasTag, HasLemma, HasOffset, TypesafeMap
    {
        string ner();

        void setNER(string ner);

        string originalText();

        void setOriginalText(string originalText);

        string getString(Type key);
    }
}