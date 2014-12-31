using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntQuadruple:IntTuple
    {
        private static readonly long serialVersionUID = 7154973101012473479L;


  public IntQuadruple():base(4) {
  }

  public IntQuadruple(int src, int mid, int trgt, int trgt2):this() {
    elements[0] = src;
    elements[1] = mid;
    elements[2] = trgt;
    elements[3] = trgt2;
  }


  //@Override
  public override IntTuple getCopy() {
    IntQuadruple nT = new IntQuadruple(elements[0], elements[1], elements[2], elements[3]);
    return nT;
  }


  public int getSource() {
    return get(0);
  }


  public int getMiddle() {
    return get(1);
  }

  public int getTarget() {
    return get(2);
  }

  public int getTarget2() {
    return get(3);
  }
    }
}
