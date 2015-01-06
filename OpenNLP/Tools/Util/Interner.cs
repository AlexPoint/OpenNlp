using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /**
 * For interning (canonicalizing) things.
 * <p/>
 * It maps any object to a unique interned version which .equals the
 * presented object.  If presented with a new object which has no
 * previous interned version, the presented object becomes the
 * interned version.  You can tell if your object has been chosen as
 * the new unique representative by checking whether o == intern(o).
 * The interners use WeakHashMap, meaning that if the only pointers
 * to an interned item are the interners' backing maps, that item can
 * still be garbage collected.  Since the gc thread can silently
 * remove things from the backing map, there's no public way to get
 * the backing map, but feel free to add one at your own risk.
 * <p/>
 * Note that in general it is just as good or better to use the
 * static Interner.globalIntern() method rather than making an
 * instance of Interner and using the instance-level intern().
 * <p/>
 * Author: Dan Klein
 * Date: 9/28/03
 *
 * @author Dan Klein
 */

    public class Interner<T> where T : class
    {
        protected static Interner<Object> interner = new Interner<Object>();

        /**
   * For getting the instance that global methods use.
   */

        public static Interner<Object> getGlobal()
        {
            return interner;
        }

        /**
   * For supplying a new instance for the global methods to use.
   * 
   * @return the previous global interner.
   */
        /*public static Interner<Object> setGlobal(Interner<Object> interner) {
    Interner<Object> oldInterner = Interner.interner;
    Interner.interner = interner;
    return oldInterner;
  }*/

        /**
   * Returns a unique object o' that .equals the argument o.  If o
   * itself is returned, this is the first request for an object
   * .equals to o.
   */
        //@SuppressWarnings("unchecked")
        public static T globalIntern<T>(T o)
        {
            return (T) getGlobal().intern(o);
        }


        protected Dictionary<T, WeakReference<T>> map = new Dictionary<T, WeakReference<T>>();

        public void clear()
        {
            map = new Dictionary<T, WeakReference<T>>();
        }

        /**
   * Returns a unique object o' that .equals the argument o.  If o
   * itself is returned, this is the first request for an object
   * .equals to o.
   */

        public T intern(T o)
        {
            if (!map.ContainsKey(o))
            {
                var wRef = new WeakReference<T>(o);
                map.Add(o, wRef);
            }
            /*WeakReference<T> wRef = map.get(o);
    if (wRef == null) {
      wRef = Generics.newWeakReference(o);
      map.put(o, ref);
    }
    return ref.get();*/
            T target;
            var success = map[o].TryGetTarget(out target);
            // TODO: test if success
            return target;
        }

        /**
   * Returns a <code>Set</code> such that each element in the returned set
   * is a unique object e' that .equals the corresponding element e in the
   * original set.
   */

        public Set<T> internAll(Set<T> s)
        {
            Set<T> result = new Set<T>();
            foreach (T o in s)
            {
                result.Add(intern(o));
            }
            return result;
        }

        public int size()
        {
            return map.Count;
        }

        /**
   * Test method: interns its arguments and says whether they == themselves.
   */
        /*public static void main(String[] args) {
    for (int i = 0; i < args.length; i++) {
      String str = args[i];
      System.out.println(Interner.globalIntern(str) == str);
    }
  }*/
    }
}