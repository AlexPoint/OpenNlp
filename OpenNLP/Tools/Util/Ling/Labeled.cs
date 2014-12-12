using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    public interface Labeled
    {
        /**
   * Returns the Object's label.
   *
   * @return One of the labels of the object (if there are multiple labels,
   *         preferably the primary label, if it exists).
   *         Returns null if there is no label.
   */

  Label label();


  /**
   * Sets the label associated with this object.
   *
   * @param label The Label value
   */

  void setLabel(/*final*/ Label label);


  /**
   * Gives back all labels for this thing.
   *
   * @return A Collection of the Object's labels.  Returns an empty
   *         Collection if there are no labels.
   */

  ICollection<Label> labels();


  /**
   * Sets the labels associated with this object.
   *
   * @param labels The set of Label values
   */

  void setLabels(/*final*/ ICollection<Label> labels);
    }
}
