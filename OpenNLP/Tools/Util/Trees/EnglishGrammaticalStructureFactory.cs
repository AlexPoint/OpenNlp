using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Util.Trees
{
    public class EnglishGrammaticalStructureFactory : GrammaticalStructureFactory
    {
        /*private readonly Predicate<String> puncFilter;
  private readonly HeadFinder hf;*/

        public EnglishGrammaticalStructureFactory() /*:this(null, null)*/
        {
        }

        /*public EnglishGrammaticalStructureFactory(Predicate<String> puncFilter):
    this(puncFilter, null){
  }

  public EnglishGrammaticalStructureFactory(Predicate<String> puncFilter, HeadFinder hf) {
    this.puncFilter = puncFilter;
    this.hf = hf;
  }*/

        public GrammaticalStructure newGrammaticalStructure(Tree t)
        {
            /*if (puncFilter == null && hf == null) {*/
            return new EnglishGrammaticalStructure(t); /*
    } else if (hf == null) {
      return new EnglishGrammaticalStructure(t, puncFilter);
    } else {
      return new EnglishGrammaticalStructure(t, puncFilter, hf);
    }*/
        }
    }
}