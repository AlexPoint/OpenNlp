using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /**
 * Something has gone wrong internally in Tsurgeon
 *
 * @author John Bauer
 */

    public class TsurgeonRuntimeException : SystemException
    {
        private static readonly long serialVersionUID = 1;

        /**
   * Creates a new exception with a message.
   *
   * @param message the message for the exception
   */

        public TsurgeonRuntimeException(String message) : base(message)
        {
        }
    }
}