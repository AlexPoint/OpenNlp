using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntTriple:IntTuple
    {
        private static readonly long serialVersionUID = -3744404627253652799L;

  public IntTriple():base(3) {
  }

  public IntTriple(int src, int mid, int trgt):this() {
    elements[0] = src;
    elements[1] = mid;
    elements[2] = trgt;
  }


  //@Override
  public IntTuple getCopy() {
    IntTriple nT = new IntTriple(elements[0], elements[1], elements[2]);
    return nT;
  }


  public int getSource() {
    return elements[0];
  }

  public int getTarget() {
    return elements[2];
  }

  public int getMiddle() {
    return elements[1];
  }
    }
}
