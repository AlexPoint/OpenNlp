using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /**
 * To make tokens like CoreMap or CoreLabel. An alternative to LexedTokenFactory
 * since this one has option to make tokens differently, which would have been
 * an overhead for LexedTokenFactory
 * 
 * @author Sonal Gupta
 * 
 * @param <IN>
 */
    public interface CoreTokenFactory<IN> where IN:CoreMap
    {
        IN makeToken();

        //IN makeToken(String[] keys, String[] values);

        IN makeToken(IN tokenToBeCopied);

    }
}
