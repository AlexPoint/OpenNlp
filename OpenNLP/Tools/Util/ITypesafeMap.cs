using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    
    /// <summary>
    /// Type signature for a class that supports the basic operations required
    /// of a typesafe heterogeneous map.
    /// 
    /// @author dramage
    /// 
    /// Code ...
    /// </summary>
    public interface ITypesafeMap
    {

        /// <summary>
        /// Returns true if the map contains the given key.
        /// TODO [cdm 2014]: This is synonymous with containsKey(), but used less, so we should just eliminate it.
        /// </summary>
        bool Has(Type key);

        /// <summary>
        /// Returns the value associated with the given key or null if none is provided.
        /// </summary>
        object Get(Type key);

        /// <summary>
        /// Associates the given value with the given type for future calls to get.
        /// Returns the value removed or null if no value was present.
        /// </summary>
        object Set(Type key, object value);

        /// <summary>
        /// Removes the given key from the map, returning the value removed.
        /// </summary>
        object Remove(Type key);

        /// <summary>
        /// Collection of keys currently held in this map.
        /// Some implementations may have the returned set be immutable.
        /// </summary>
        Set<Type> KeySet();
        //public Set<Class<? extends Key<?>>> keySet();

        /// <summary>
        /// Returns true if contains the given key.
        /// </summary>
        bool ContainsKey(Type key);

        /// <summary>
        /// Returns the number of keys in the map.
        /// </summary>
        int Size();
    }

    /// <summary>
    /// Base type of keys for the map.
    /// The classes that implement Key are the keys themselves - not instances of those classes.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with this key.</typeparam>
    public interface IKey<T>
    {
    }
}