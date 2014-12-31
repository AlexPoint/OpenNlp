using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntUni:IntTuple
    {
        public IntUni():base(1) {
  }


  public IntUni(int src):this() {
    elements[0] = src;
  }


  public int getSource() {
    return elements[0];
  }

  public void setSource(int src) {
    elements[0] = src;
  }


  //@Override
  public override IntTuple getCopy() {
    IntUni nT = new IntUni(elements[0]);
    return nT;
  }

  public void add(int val) {
    elements[0] += val;
  }

  private static readonly long serialVersionUID = -7182556672628741200L;
    }
}
