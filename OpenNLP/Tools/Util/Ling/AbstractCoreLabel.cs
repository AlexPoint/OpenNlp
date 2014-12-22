using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    public interface AbstractCoreLabel : Label, HasWord, HasIndex, HasTag, HasLemma, HasOffset, TypesafeMap
    {
        String ner();

  void setNER(String ner);

  String originalText();

  void setOriginalText(String originalText);

  String getString(Type key);
    }
}
