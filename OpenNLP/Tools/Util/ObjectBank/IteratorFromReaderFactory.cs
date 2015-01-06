using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.ObjectBank
{
    public interface IteratorFromReaderFactory<T>
    {
        /** Return an iterator over the contents read from r.
   *
   * @param r Where to read objects from
   * @return An Iterator over the objects
   */
        IEnumerator<T> GetIterator(TextReader r);
    }
}