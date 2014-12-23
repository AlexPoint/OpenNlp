using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * An individual dependency between a governor and a dependent.
 * The governor and dependent are represented as a Label.
 * For example, these can be a
 * Word or a WordTag.  If one wishes the dependencies to preserve positions
 * in a sentence, then each can be a LabeledConstituent or CoreLabel.
 * Dependencies support an Object naming the dependency type.  This may be
 * null.  Dependencies have factories.
 *
 * @author Christopher Manning
 */
    public interface Dependency<G,D,N> where G : Label where D: Label
    {
        /**
   * Describes the governor (regent/head) of the dependency relation.
   * @return The governor of this dependency
   */
        G governor();

        /**
         * Describes the dependent (argument/modifier) of
         * the dependency relation.
         * @return the dependent of this dependency
         */
        D dependent();

        /**
         * Names the type of dependency (subject, instrument, ...).
         * This might be a String in the simplest case, but can provide for
         * arbitrary object types.
         * @return the name for this dependency type
         */
        N name();

        /**
         * Are two dependencies equal if you ignore the dependency name.
         * @param o The thing to compare against ignoring name
         * @return true iff the head and dependent are the same.
         */
        bool equalsIgnoreName(Object o);

        /**
         * Provide different printing options via a String keyword.
         * The main recognized option currently is "xml".  Otherwise the
         * default toString() is used.
         * @param format A format string, either "xml" or you get the default
         * @return A String representation of the dependency
         */
        String toString(String format);

        /**
         * Provide a factory for this kind of dependency
         * @return A DependencyFactory
         */
        DependencyFactory dependencyFactory();
    }
}
