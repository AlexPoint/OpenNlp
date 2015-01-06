using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class PropertiesUtils
    {
        private PropertiesUtils()
        {
        }

        /**
   * Returns true iff the given Properties contains a property with the given
   * key (name), and its value is not "false" or "no" or "off".
   *
   * @param props Properties object
   * @param key The key to test
   * @return true iff the given Properties contains a property with the given
   * key (name), and its value is not "false" or "no" or "off".
   */

        public static bool hasProperty(Dictionary<String, String> props, String key)
        {
            String value = props[key];
            if (value == null)
            {
                return false;
            }
            value = value.ToLower();
            return ! (value.Equals("false") || value.Equals("no") || value.Equals("off"));
        }

        // Convert from properties to string and from string to properties
        public static String asString(Dictionary<String, String> props)
        {
            try
            {
                StringWriter sw = new StringWriter();
                props[sw.ToString()] = null;
                return sw.ToString();
            }
            catch (IOException ex)
            {
                throw new SystemException("Cannot convert property as string", ex);
            }
        }

        /*public static Dictionary<String,String> fromString(String str) {
    try {
      StringReader sr = new StringReader(str);
      Dictionary<String,String> props = new Dictionary<String,String>();
      props.load(sr);
      return props;
    } catch (IOException ex) {
      throw new SystemException(ex);
    }
  }*/

        // printing -------------------------------------------------------------------

        /*public static void printProperties(String message, Dictionary<String,String> properties,
                                     PrintStream stream) {
    if (message != null) {
      stream.println(message);
    }
    if (properties.isEmpty()) {
      stream.println("  [empty]");
    } else {
      List<Map.Entry<String, String>> entries = getSortedEntries(properties);
      for (Map.Entry<String, String> entry : entries) {
        if ( ! "".equals(entry.getKey())) {
          stream.format("  %-30s = %s%n", entry.getKey(), entry.getValue());
        }
      }
    }
    stream.println();
  }*/

        /* public static void printProperties(String message, Dictionary<String,String> properties) {
    printProperties(message, properties, System.out);
  }*/

        /**
   * Tired of Properties not behaving like {@code Map<String,String>}s?  This method will solve that problem for you.
   */

        public static Dictionary<String, String> asMap(Dictionary<String, String> properties)
        {
            /*Map<String, String> map = Generics.newHashMap();
    foreach (Entry<Object, Object> entry in properties.entrySet()) {
      map.put((String)entry.getKey(), (String)entry.getValue());
    }
    return map;*/
            return properties.ToDictionary(ent => ent.Key, ent => ent.Value);
        }

        public static List<KeyValuePair<String, String>> getSortedEntries(Dictionary<String, String> properties)
        {
            //return Maps.sortedEntries(asMap(properties));
            return asMap(properties).Select(e => e).ToList();
        }

        /**
   * Checks to make sure that all properties specified in <code>properties</code>
   * are known to the program by checking that each simply overrides
   * a default value.
   *
   * @param properties Current properties
   * @param defaults Default properties which lists all known keys
   */
        //@SuppressWarnings("unchecked")
        /*public static void checkProperties(Dictionary<String,String> properties, Dictionary<String,String> defaults) {
    Set<String> names = Generics.newHashSet();
    for (Enumeration<String> e = (Enumeration<String>) properties.propertyNames();
         e.hasMoreElements(); ) {
      names.add(e.nextElement());
    }
    for (Enumeration<String> e = (Enumeration<String>) defaults.propertyNames();
         e.hasMoreElements(); ) {
      names.remove(e.nextElement());
    }
    if (!names.isEmpty()) {
      if (names.size() == 1) {
        throw new ArgumentException("Unknown property: " + names.iterator().next());
      } else {
        throw new ArgumentException("Unknown properties: " + names);
      }
    }
  }*/

        /**
   * Build a {@code Properties} object containing key-value pairs from
   * the given data where the keys are prefixed with the given
   * {@code prefix}. The keys in the returned object will be stripped
   * of their common prefix.
   *
   * @param properties Key-value data from which to extract pairs
   * @param prefix Key-value pairs where the key has this prefix will
   *               be retained in the returned {@code Properties} object
   * @return A Properties object containing those key-value pairs from
   *         {@code properties} where the key was prefixed by
   *         {@code prefix}. This prefix is removed from all keys in
   *         the returned structure.
   */

        public static Dictionary<String, String> extractPrefixedProperties(Dictionary<String, String> properties,
            String prefix)
        {
            Dictionary<String, String> ret = new Dictionary<String, String>();

            foreach (var entry in properties)
            {
                String keyStr = entry.Key;

                if (keyStr.StartsWith(prefix))
                {
                    String newStr = keyStr.Substring(prefix.Length);
                    ret[newStr] = entry.Value;
                }
            }

            return ret;
        }


        /**
   * Get the value of a property and automatically cast it to a specific type.
   * This differs from the original Properties.getProperty() method in that you
   * need to specify the desired type (e.g. Double.class) and the default value
   * is an object of that type, i.e. a double 0.0 instead of the String "0.0".
   */
        //@SuppressWarnings("unchecked")
        public static /*<E>*/ E get<E>(Dictionary<String, String> props, String key, E defaultValue, Type type)
        {
            String value = props[key];
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                //return (E) MetaClass.cast(value, type);
                return (E) Convert.ChangeType(value, type);
            }
        }

        /**
   * Load an integer property.  If the key is not present, returns defaultValue.
   */

        public static String getString(Dictionary<String, String> props, String key, String defaultValue)
        {
            String value = props[key];
            if (value != null)
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        /**
   * Load an integer property.  If the key is not present, returns 0.
   */

        public static int getInt(Dictionary<String, String> props, String key)
        {
            return getInt(props, key, 0);
        }

        /**
   * Load an integer property.  If the key is not present, returns defaultValue.
   */

        public static int getInt(Dictionary<String, String> props, String key, int defaultValue)
        {
            String value = props[key];
            if (value != null)
            {
                return int.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }

        /**
   * Load an integer property as a long.  
   * If the key is not present, returns defaultValue.
   */

        public static long getLong(Dictionary<String, String> props, String key, long defaultValue)
        {
            String value = props[key];
            if (value != null)
            {
                return long.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }

        /**
   * Load a double property.  If the key is not present, returns 0.0.
   */

        public static double getDouble(Dictionary<String, String> props, String key)
        {
            return getDouble(props, key, 0.0);
        }

        /**
   * Load a double property.  If the key is not present, returns defaultValue.
   */

        public static double getDouble(Dictionary<String, String> props, String key, double defaultValue)
        {
            String value = props[key];
            if (value != null)
            {
                return Double.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }

        /**
   * Load a bool property.  If the key is not present, returns false.
   */

        public static bool getBool(Dictionary<String, String> props, String key)
        {
            return getBool(props, key, false);
        }

        /**
   * Load a bool property.  If the key is not present, returns defaultValue.
   */

        public static bool getBool(Dictionary<String, String> props, String key,
            bool defaultValue)
        {
            String value = props[key];
            if (value != null)
            {
                return bool.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }

        /**
   * Loads a comma-separated list of integers from Properties.  The list cannot include any whitespace.
   */
        /*public static int[] getIntArray(Dictionary<String,String> props, String key) {
    Integer[] result = MetaClass.cast(props[key], Integer [].class);
    return ArrayUtils.toPrimitive(result);
  }*/

        /**
   * Loads a comma-separated list of doubles from Properties.  The list cannot include any whitespace.
   */
        /*public static double[] getDoubleArray(Dictionary<String,String> props, String key) {
    Double[] result = MetaClass.cast(props[key], Double [].class);
    return ArrayUtils.toPrimitive(result);
  }*/

        /**
   * Loads a comma-separated list of strings from Properties.  Commas may be quoted if needed, e.g.:
   *    property1 = value1,value2,"a quoted value",'another quoted value'
   *    
   * getStringArray(props, "property1") should return the same thing as
   *    new String[] { "value1", "value2", "a quoted value", "another quoted value" };
   */
        /*public static String[] getStringArray(Dictionary<String,String> props, String key) {
    String[] results = MetaClass.cast(props[key], String [].class);
    if (results == null) {
      results = new String[]{};
    }
    return results;
  }*/

        /*public static String[] getStringArray(Dictionary<String,String> props, String key, String[] defaults) {
    String[] results = MetaClass.cast(props[key], String [].class);
    if (results == null) {
      results = defaults;
    }
    return results;
  }*/

        public /*static*/ class Property
        {
            public String name;
            public String defaultValue;
            public String description;

            public Property(String name, String defaultValue, String description)
            {
                this.name = name;
                this.defaultValue = defaultValue;
                this.description = description;
            }
        }

        /*public static String getSignature(String name, Dictionary<String,String> properties, Dictionary<String,String>[] supportedProperties) {
    String prefix = (name != null && name.Any())? name + ".":"";
    // keep track of all relevant properties for this annotator here!
    StringBuilder sb = new StringBuilder();
    foreach (Dictionary<String,String> p in supportedProperties) {
      String pname = prefix + p.name;
      String pvalue = properties[pname] = p.defaultValue;
      sb.Append(pname).Append(":").Append(pvalue);
    }
    return sb.ToString();
  }*/
    }
}