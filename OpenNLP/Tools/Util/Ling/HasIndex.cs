using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /// <summary>
    /// @author grenager
    /// 
    /// Code...
    /// </summary>
    public interface HasIndex
    {
        string DocId();

        void SetDocId(string docId);

        int SentIndex();

        void SetSentIndex(int sentIndex);

        int Index();

        void SetIndex(int index);
    }
}