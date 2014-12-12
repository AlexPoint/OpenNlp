using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    [Serializable]
    public class IntTuple:IComparable<IntTuple>
    {
        protected readonly int[] elements;

  private static readonly long serialVersionUID = 7266305463893511982L;


  public IntTuple(int[] arr) {
    elements = arr;
  }

  public IntTuple(int num) {
    elements = new int[num];
  }

  /*@Override*/
  public int CompareTo(IntTuple o) {
    int commonLen = Math.Min(o.Length(), Length());
    for (int i = 0; i < commonLen; i++) {
      int a = get(i);
      int b = o.get(i);
      if (a < b) return -1;
      if (b < a) return 1;
    }
    if (o.Length() == Length()) {
      return 0;
    } else {
      return (Length() < o.Length())? -1:1;
    }
  }

  public int get(int num) {
    return elements[num];
  }


  public void set(int num, int val) {
    elements[num] = val;
  }

  public void shiftLeft()
  {
      var shiftedArray = elements.Skip(1).ToArray();
      for (var i = 0; i < elements.Length; i++)
      {
          if (i < elements.Length - 1)
          {
              elements[i] = shiftedArray[i];
          }
          else
          {
              elements[i] = 0;
          }
      }
      
    /*System.arraycopy(elements, 1, elements, 0, elements.Length - 1);  // the API does guarantee that this works when src and dest overlap, as here
    elements[elements.Length - 1] = 0;*/
  }


  public IntTuple getCopy() {
      
    IntTuple copy = IntTuple.getIntTuple(elements.Length); //new IntTuple(numElements);
      for (var i = 0; i < elements.Length; i++)
      {
          copy.elements[i] = elements[i];
      }
    //System.arraycopy(elements, 0, copy.elements, 0, elements.Length);
      
    return copy;
  }


  public int[] elems() {
    return elements;
  }

  //@Override
  public override bool Equals(Object iO) {
    if (!(iO is IntTuple)) {
      return false;
    }
    IntTuple i = (IntTuple) iO;
    if (i.elements.Length != elements.Length) {
      return false;
    }
    for (int j = 0; j < elements.Length; j++) {
      if (elements[j] != i.get(j)) {
        return false;
      }
    }
    return true;
  }


  //@Override
  public override int GetHashCode() {
    int sum = 0;
    foreach (int element in elements) {
      sum = sum * 17 + element;
    }
    return sum;
  }


  public int Length() {
    return elements.Length;
  }


  public static IntTuple getIntTuple(int num) {
    if (num == 1) {
        return new IntUni();
    }
    if ((num == 2)) {
      return new IntPair();
    }
    if (num == 3) {
        return new IntTriple();
    }
    if (num == 4) {
        return new IntQuadruple();
    } else {
      return new IntTuple(num);
    }
  }


  public static IntTuple getIntTuple(List<int> integers) {
    IntTuple t = IntTuple.getIntTuple(integers.Count);
    for (int i = 0; i < t.Length(); i++) {
      t.set(i, integers[i]);
    }
    return t;
  }

  //@Override
  public override string ToString() {
    var name = new StringBuilder();
    for (int i = 0; i < elements.Length; i++) {
      name.Append(get(i));
      if (i < elements.Length - 1) {
        name.Append(' ');
      }
    }
    return name.ToString();
  }


  public static IntTuple concat(IntTuple t1, IntTuple t2) {
    int n1 = t1.Length();
    int n2 = t2.Length();
    IntTuple res = IntTuple.getIntTuple(n1 + n2);

    for (int j = 0; j < n1; j++) {
      res.set(j, t1.get(j));
    }
    for (int i = 0; i < n2; i++) {
      res.set(n1 + i, t2.get(i));
    }
    return res;
  }


  public void print() {
    String s = ToString();
    //System.out.print(s);
  }
    }
}
