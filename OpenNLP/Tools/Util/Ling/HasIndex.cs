using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * @author grenager
 */

    public interface HasIndex
    {
        String docID();
        void setDocID(String docID);
        int sentIndex();
        void setSentIndex(int sentIndex);
        int index();
        void setIndex(int index);
    }
}