using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public interface HasWord
    {
        string word();
        void setWord(string word);
    }
}
