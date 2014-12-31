using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * An individual dependency between a head and a dependent. The dependency
 * is associated with the token indices of the lexical items.
 * <p>
 * A key difference between this class and UnnamedDependency is the equals()
 * method. Equality of two UnnamedConcreteDependency objects is defined solely
 * with respect to the indices. The surface forms are not considered. This permits 
 * a use case in which dependencies in two different parse trees have slightly different 
 * pre-processing, possibly due to pre-processing.
 * 
 * @author Spence Green
 * 
 */
    public class UnnamedConcreteDependency : UnnamedDependency
    {
        private static readonly long serialVersionUID = -8836949694741145222L;

  private readonly int headIndex;
  private readonly int depIndex;

  public UnnamedConcreteDependency(String regent, int regentIndex, String dependent, int dependentIndex):
    base(regent, dependent){

    headIndex = regentIndex;
    depIndex = dependentIndex;
  }
  
  public UnnamedConcreteDependency(Label regent, int regentIndex, Label dependent, int dependentIndex):
    base(regent, dependent){
    
    headIndex = regentIndex;
    depIndex = dependentIndex;
  }

  public UnnamedConcreteDependency(Label regent, Label dep):
    base(regent, dep){

    if (governor() is HasIndex) {
      headIndex = ((HasIndex) governor()).index();
    } else {
      throw new ArgumentException("Label argument lacks IndexAnnotation.");
    }
    if (dependent() is HasIndex) {
      depIndex = ((HasIndex) dependent()).index();
    } else {
      throw new ArgumentException("Label argument lacks IndexAnnotation.");
    }
  }
  
  public int getGovernorIndex() { return headIndex; }
  
  public int getDependentIndex() { return depIndex; }
  
  //@Override
  public override int GetHashCode() {
    return headIndex * (depIndex << 16);
  }

  //@Override
  public override bool Equals(Object o) {
    if (this == o) {
      return true;
    } else if( !(o is UnnamedConcreteDependency)) {
      return false;
    }
    UnnamedConcreteDependency d = (UnnamedConcreteDependency) o;
    return headIndex == d.headIndex && depIndex == d.depIndex;
  }

  //@Override
  public override String ToString() {
    String headWord = getText(governor());
    String depWord = getText(dependent());
    return String.Format("{0} [{1}] --> {2} [{3}]", headWord, headIndex, depWord, depIndex);
  }

  /**
   * Provide different printing options via a String keyword.
   * The recognized options are currently "xml", and "predicate".
   * Otherwise the default toString() is used.
   */
  //@Override
  public String toString(String format) {
    switch (format) {
      case "xml":
        String govIdxStr = " idx=\"" + headIndex + "\"";
        String depIdxStr = " idx=\"" + depIndex + "\"";
        return "  <dep>\n    <governor" + govIdxStr + ">" + XMLUtils.XmlEscape(governor().value()) + "</governor>\n    <dependent" + depIdxStr + ">" + XMLUtils.XmlEscape(dependent().value()) + "</dependent>\n  </dep>";
      case "predicate":
        return "dep(" + governor() + "," + dependent() + ")";
      default:
        return ToString();
    }
  }

  //@Override
  public DependencyFactory dependencyFactory() {
    return DependencyFactoryHolder.df;
  }
  
  public static DependencyFactory factory() {
    return DependencyFactoryHolder.df;
  }

  // extra class guarantees correct lazy loading (Bloch p.194)
  private static class DependencyFactoryHolder {
    public static readonly DependencyFactory df = new UnnamedConcreteDependencyFactory();
  }

  /**
   * A <code>DependencyFactory</code> acts as a factory for creating objects
   * of class <code>Dependency</code>
   */
  private /*static */class UnnamedConcreteDependencyFactory : DependencyFactory {
    /**
     * Create a new <code>Dependency</code>.
     */
    public Dependency<Label, Label, Object> newDependency(Label regent, Label dependent) {
      return newDependency(regent, dependent, null);
    }

    /**
     * Create a new <code>Dependency</code>.
     */
    public Dependency<Label, Label, Object> newDependency(Label regent, Label dependent, Object name) {
      return new UnnamedConcreteDependency(regent, dependent);
    }
  }
    }
}
