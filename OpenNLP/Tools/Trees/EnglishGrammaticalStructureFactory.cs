using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Trees
{
    public class EnglishGrammaticalStructureFactory : IGrammaticalStructureFactory
    {
        /*private readonly Predicate<string> puncFilter;
        private readonly HeadFinder hf;*/

        /*public EnglishGrammaticalStructureFactory(Predicate<string> puncFilter):
            this(puncFilter, null){
          }

          public EnglishGrammaticalStructureFactory(Predicate<string> puncFilter, HeadFinder hf) {
            this.puncFilter = puncFilter;
            this.hf = hf;
          }*/

        public GrammaticalStructure NewGrammaticalStructure(Tree t)
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