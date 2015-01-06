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
        string DocID();
        void SetDocID(string docID);
        int SentIndex();
        void SetSentIndex(int sentIndex);
        int Index();
        void SetIndex(int index);
    }
}