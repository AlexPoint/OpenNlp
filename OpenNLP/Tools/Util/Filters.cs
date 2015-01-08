using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /// <summary>
    /// Some simple implementations of the {@link java.util.function.Predicate} interface.
    /// 
    /// @author Christopher Manning
    /// @version 1.0
    /// </summary>
    public class Filters
    {

        /// <summary>
        /// The acceptFilter accepts everything.
        /// </summary>
        public static Predicate<T> AcceptFilter<T>()
        {
            return s => true;
            //return new CategoricalFilter<T>(true);
        }

        /// <summary>
        /// The rejectFilter accepts nothing.
        /// </summary>
        public static Predicate<T> RejectFilter<T>()
        {
            return s => false;
            //return new CategoricalFilter<T>(false);
        }

        /*private static sealed class CategoricalFilter<T> : Predicate<T>/*, Serializable#1# {

    private readonly bool judgment;

    protected CategoricalFilter(bool judgment) {
      this.judgment = judgment;
    }

    /**
     * Checks if the given object passes the filter.
     *
     * @param obj an object to test
     #1#
    public bool test(T obj) {
      return judgment;
    }

    public string ToString() {
      return "CategoricalFilter(" + judgment + ")";
    }

    public int hashCode() {
      return ToString().hashCode();
    }

    public bool equals(Object other) {
      if (other == this) {
        return true;
      }
      if (!(other instanceof CategoricalFilter)) {
        return false;
      }
      return ((CategoricalFilter) other).judgment == this.judgment;
    }
    
  }*/


        /*/**
   * The collectionAcceptFilter accepts a certain collection.
   #1#
  public static <E> Predicate<E> collectionAcceptFilter(E[] objs) {
    return new CollectionAcceptFilter<E>(Arrays.asList(objs), true);
  }*/
        
        /// <summary>
        /// The collectionAcceptFilter accepts a certain collection.
        /// </summary>
        public static Predicate<E> CollectionAcceptFilter<E>(IEnumerable<E> objs)
        {
            return s => objs.Any(o => o.Equals(s));
            //return new CollectionAcceptFilter<E>(objs, true);
        }

        /*/**
   * The collectionRejectFilter rejects a certain collection.
   #1#
  public static Predicate<E> collectionRejectFilter<E>(E[] objs) {
    return new CollectionAcceptFilter<E>(objs.ToList(), false);
  }*/

        /// <summary>
        /// The collectionRejectFilter rejects a certain collection.
        /// </summary>
        public static Predicate<E> CollectionRejectFilter<E>(IEnumerable<E> objs)
        {
            return s => objs.All(o => !o.Equals(s));
            //return new CollectionAcceptFilter<E>(objs, false);
        }

        /*private sealed class CollectionAcceptFilter<E> /*: Predicate<E>, Serializable#1# {

    private readonly IEnumerable<E> args;
    private readonly bool judgment;

    public CollectionAcceptFilter(IEnumerable<E> c, bool judgment) {
      this.args = c.ToList();
      this.judgment = judgment;
    }

    /**
     * Checks if the given object passes the filter.
     *
     * @param obj an object to test
     #1#
    public bool test(E obj) {
      if (args.contains(obj)) {
        return judgment;
      }
      return !judgment;
    }

    public string ToString() {
      return "(" + judgment +":" + args + ")";
    }

  } // end class CollectionAcceptFilter*/

        /// <summary>
        /// Filter that accepts only when both filters accept (AND).
        /// </summary>
        public static Predicate<E> AndFilter<E>(Predicate<E> f1, Predicate<E> f2)
        {
            return s => f1(s) && f2(s);
            //return (new CombinedFilter<E>(f1, f2, true));
        }

        /// <summary>
        /// Filter that accepts when either filter accepts (OR).
        /// </summary>
        public static Predicate<E> OrFilter<E>(Predicate<E> f1, Predicate<E> f2)
        {
            return s => f1(s) || f2(s);
            //return (new CombinedFilter<E>(f1, f2, false));
        }

        /*/**
   * Conjunction or disjunction of two filters.
   #1#
  private static class CombinedFilter<E> implements Predicate<E>, Serializable {
    private Predicate<E> f1, f2;
    private bool conjunction; // and vs. or

    public CombinedFilter(Predicate<E> f1, Predicate<E> f2, bool conjunction) {
      this.f1 = f1;
      this.f2 = f2;
      this.conjunction = conjunction;
    }

    public bool test(E o) {
      if (conjunction) {
        return (f1.test(o) && f2.test(o));
      }
      return (f1.test(o) || f2.test(o));
    }
  }*/

        /*/**
   * Disjunction of a list of filters.
   #1#
  public static class DisjFilter<T> implements Predicate<T>, Serializable {
    List<Predicate<T>> filters;

    public DisjFilter(List<Predicate<T>> filters) {
      this.filters = filters;
    }

    public DisjFilter(Predicate<T>... filters) {
      this.filters = new ArrayList<Predicate<T>>();
      this.filters.addAll(Arrays.asList(filters));
    }

    public void addFilter(Predicate<T> filter) {
      filters.add(filter);
    }

    public bool test(T obj) {
      for (Predicate<T> f:filters) {
        if (f.test(obj)) return true;
      }
      return false;
    }
  }*/

        /**
   * Conjunction of a list of filters.
   */
        /*public static class ConjFilter<T> implements Predicate<T>, Serializable {
    List<Predicate<T>> filters;

    public ConjFilter(List<Predicate<T>> filters) {
      this.filters = filters;
    }

    public ConjFilter(Predicate<T>... filters) {
      this.filters = new ArrayList<Predicate<T>>();
      this.filters.addAll(Arrays.asList(filters));
    }

    public void addFilter(Predicate<T> filter) {
      filters.add(filter);
    }

    public bool test(T obj) {
      for (Predicate<T> f:filters) {
        if (!f.test(obj)) return false;
      }
      return true;
    }
  }*/

        /// <summary>
        /// Filter that does the opposite of given filter (NOT).
        /// </summary>
        public static Predicate<E> NotFilter<E>(Predicate<E> filter)
        {
            return s => !filter(s);
            //return (new NegatedFilter<E>(filter));
        }

        /// <summary>
        /// Filter that's either negated or normal as specified.
        /// </summary>
        public static Predicate<E> SwitchedFilter<E>(Predicate<E> filter, bool negated)
        {
            return s => negated ? !filter(s) : filter(s);
            //return (new NegatedFilter<E>(filter, negated));
        }

        /**
   * Negation of a filter.
   */
        /*private static class NegatedFilter<E> implements Predicate<E>, Serializable {
    private Predicate<E> filter;
    private bool negated;

    public NegatedFilter(Predicate<E> filter, bool negated) {
      this.filter = filter;
      this.negated = negated;
    }

    public NegatedFilter(Predicate<E> filter) {
      this(filter, true);
    }

    public bool test(E o) {
      return (negated ^ filter.test(o)); // xor
    }

    public string ToString() {
      return "NOT(" + filter.ToString() + ")";
    }
  }*/

        /**
   * A filter that accepts a random fraction of the input it sees.
   */
        /*public static class RandomFilter<E> implements Predicate<E>, Serializable {
    readonly Random random;
    readonly double fraction;

    public RandomFilter() {
      this(0.1, new Random());
    }

    public RandomFilter(double fraction) {
      this(fraction, new Random());
    }

    public RandomFilter(double fraction, Random random) {
      this.fraction = fraction;
      this.random = random;
    }

    public bool test(E o) {
      return (random.nextDouble() < fraction);
    }
  }*/

        /**
   * Applies the given filter to each of the given elems, and returns the
   * list of elems that were accepted. The runtime type of the returned
   * array is the same as the passed in array.
   */
        /*public static <E> E[] filter(E[] elems, Predicate<E> filter) {
    List<E> filtered = new ArrayList<E>();
    for (E elem: elems) {
      if (filter.test(elem)) {
        filtered.add(elem);
      }
    }
    return (filtered.toArray((E[]) Array.newInstance(elems.getClass().getComponentType(), filtered.size())));
  }s*/

        /**
   * Removes all elems in the given Collection that aren't accepted by the given Filter.
   */
        /*public static <E> void retainAll(Collection<E> elems, Predicate<? super E> filter) {
    for (Iterator<E> iter = elems.iterator(); iter.hasNext();) {
      E elem = iter.next();
      if ( ! filter.test(elem)) {
        iter.remove();
      }
    }
  }*/
    }
}