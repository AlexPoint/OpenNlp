using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    public interface Scored
    {
        /**
   * @return The score of this thing.
   */
        double score();
    }
}
