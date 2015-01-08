using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    
    /// <summary>
    /// For interning (canonicalizing) things.
    /// 
    /// It maps any object to a unique interned version which .equals the
    /// presented object.  If presented with a new object which has no
    /// previous interned version, the presented object becomes the
    /// interned version.  You can tell if your object has been chosen as
    /// the new unique representative by checking whether o == intern(o).
    /// The interners use WeakHashMap, meaning that if the only pointers
    /// to an interned item are the interners' backing maps, that item can
    /// still be garbage collected.  Since the gc thread can silently
    /// remove things from the backing map, there's no public way to get
    /// the backing map, but feel free to add one at your own risk.
    /// 
    /// Note that in general it is just as good or better to use the
    /// static Interner.globalIntern() method rather than making an
    /// instance of Interner and using the instance-level intern().
    /// 
    /// @author Dan Klein
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class Interner<T> where T : class
    {
        protected static Interner<Object> interner = new Interner<Object>();
        
        /// <summary>
        /// For getting the instance that global methods use.
        /// </summary>
        public static Interner<Object> GetGlobal()
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

        /// <summary>
        /// Returns a unique object o' that .equals the argument o.
        /// If o itself is returned, this is the first request for an object
        /// .equals to o.
        /// </summary>
        public static T GlobalIntern<T>(T o)
        {
            return (T) GetGlobal().Intern(o);
        }


        protected Dictionary<T, WeakReference<T>> map = new Dictionary<T, WeakReference<T>>();

        public void Clear()
        {
            map = new Dictionary<T, WeakReference<T>>();
        }

        /// <summary>
        /// Returns a unique object o' that .equals the argument o.
        /// If o itself is returned, this is the first request for an object
        /// .equals to o.
        /// </summary>
        public T Intern(T o)
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

        /// <summary>
        /// Returns a <code>Set</code> such that each element in the returned set
        /// is a unique object e' that .equals the corresponding element e in the
        /// original set.
        /// </summary>
        public Set<T> InternAll(Set<T> s)
        {
            var result = new Set<T>();
            foreach (T o in s)
            {
                result.Add(Intern(o));
            }
            return result;
        }

        public int Size()
        {
            return map.Count;
        }

    }
}