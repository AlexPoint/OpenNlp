using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * An individual dependency between a head and a dependent.
 * The head and dependent are represented as a Label.
 * For example, these can be a
 * Word or a WordTag.  If one wishes the dependencies to preserve positions
 * in a sentence, then each can be a NamedConstituent.
 *
 * @author Christopher Manning
 * @author Spence Green
 * 
 */
    public class NamedDependency: UnnamedDependency
    {
        private static readonly long serialVersionUID = -1635646451505721133L;

  private readonly Object vName;

  public NamedDependency(String regent, String dependent, Object name):
    base(regent, dependent){
    this.vName = name;
  }

  public NamedDependency(Label regent, Label dependent, Object name):
    base(regent, dependent){
    this.vName = name;
  }

  //@Override
  public Object name() {
    return vName;
  }

  //@Override
  public override int GetHashCode() {
      return regentText.GetHashCode() ^ dependentText.GetHashCode() ^ vName.GetHashCode();
  }

  //@Override
  public override bool Equals(Object o) {
    if (this == o) {
      return true;
    } else if ( !(o is NamedDependency)) {
      return false;
    }
    NamedDependency d = (NamedDependency) o;
    return equalsIgnoreName(o) && vName.Equals(d.vName);
  }

  //@Override
  public override String ToString() {
    return String.Format("{0} --{1}--> {2}", regentText, vName.ToString(), dependentText);
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
        return "  <dep>\n    <governor>" + XMLUtils.XmlEscape(governor().value()) + "</governor>\n    <dependent>" + XMLUtils.XmlEscape(dependent().value()) + "</dependent>\n  </dep>";
      case "predicate":
        return "dep(" + governor() + "," + dependent() + "," + name() + ")";
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
    public static readonly DependencyFactory df = new NamedDependencyFactory();
  }

  /**
   * A <code>DependencyFactory</code> acts as a factory for creating objects
   * of class <code>Dependency</code>
   */
  private /*static */class NamedDependencyFactory : DependencyFactory {
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
      return new NamedDependency(regent, dependent, name);
    }
  }
    }
}
