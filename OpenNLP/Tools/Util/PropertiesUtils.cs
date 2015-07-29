using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public static class PropertiesUtils
    {
        /// <summary>
        /// Returns true iff the given Properties contains a property with the given
        /// key (name), and its value is not "false" or "no" or "off".
        /// </summary>
        /// <param name="props">Properties object</param>
        /// <param name="key">The key to test</param>
        public static bool HasProperty(Dictionary<string, string> props, string key)
        {
            string value = props[key];
            if (value == null)
            {
                return false;
            }
            value = value.ToLower();
            return ! (value.Equals("false") || value.Equals("no") || value.Equals("off"));
        }

        /// <summary>
        /// Convert from properties to string and from string to properties
        /// </summary>
        public static string AsString(Dictionary<string, string> props)
        {
            try
            {
                var sw = new StringWriter();
                props[sw.ToString()] = null;
                return sw.ToString();
            }
            catch (IOException ex)
            {
                throw new SystemException("Cannot convert property as string", ex);
            }
        }

        /*public static Dictionary<string,String> fromString(string str) {
            try {
              StringReader sr = new StringReader(str);
              Dictionary<string,String> props = new Dictionary<string,String>();
              props.load(sr);
              return props;
            } catch (IOException ex) {
              throw new SystemException(ex);
            }
          }*/

        // printing -------------------------------------------------------------------

        /*public static void printProperties(string message, Dictionary<string,String> properties,
                                     PrintStream stream) {
            if (message != null) {
              stream.println(message);
            }
            if (properties.isEmpty()) {
              stream.println("  [empty]");
            } else {
              List<Map.Entry<string, string>> entries = getSortedEntries(properties);
              for (Map.Entry<string, string> entry : entries) {
                if ( ! "".equals(entry.getKey())) {
                  stream.format("  %-30s = %s%n", entry.getKey(), entry.getValue());
                }
              }
            }
            stream.println();
          }*/
        
        /// <summary>
        /// Tired of Properties not behaving like a Dictionary of string
        /// This method will solve that problem for you.
        /// </summary>
        public static Dictionary<string, string> AsMap(Dictionary<string, string> properties)
        {
            /*Map<string, string> map = Generics.newHashMap();
            foreach (Entry<Object, Object> entry in properties.entrySet()) {
              map.put((string)entry.getKey(), (string)entry.getValue());
            }
            return map;*/
            return properties.ToDictionary(ent => ent.Key, ent => ent.Value);
        }

        public static List<KeyValuePair<string, string>> GetSortedEntries(Dictionary<string, string> properties)
        {
            //return Maps.sortedEntries(asMap(properties));
            return AsMap(properties).Select(e => e).ToList();
        }

        /**
   * Checks to make sure that all properties specified in <code>properties</code>
   * are known to the program by checking that each simply overrides
   * a default value.
   *
   * @param properties Current properties
   * @param defaults Default properties which lists all known keys
   */
        /*public static void checkProperties(Dictionary<string,String> properties, Dictionary<string,String> defaults) {
    Set<string> names = Generics.newHashSet();
    for (Enumeration<string> e = (Enumeration<string>) properties.propertyNames();
         e.hasMoreElements(); ) {
      names.add(e.nextElement());
    }
    for (Enumeration<string> e = (Enumeration<string>) defaults.propertyNames();
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
        
        /// <summary>
        /// Build a {@code Properties} object containing key-value pairs from
        /// the given data where the keys are prefixed with the given {@code prefix}.
        /// The keys in the returned object will be stripped of their common prefix.
        /// </summary>
        /// <param name="properties">Key-value data from which to extract pairs</param>
        /// <param name="prefix">Key-value pairs where the key has this prefix will be retained in the returned {@code Properties} object</param>
        /// <returns>A Properties object containing those key-value pairs from 
        /// {@code properties} where the key was prefixed by {@code prefix}. 
        /// This prefix is removed from all keys in the returned structure.</returns>
        public static Dictionary<string, string> ExtractPrefixedProperties(Dictionary<string, string> properties,
            string prefix)
        {
            var ret = new Dictionary<string, string>();

            foreach (var entry in properties)
            {
                string keyStr = entry.Key;

                if (keyStr.StartsWith(prefix))
                {
                    string newStr = keyStr.Substring(prefix.Length);
                    ret[newStr] = entry.Value;
                }
            }

            return ret;
        }

        /// <summary>
        /// Get the value of a property and automatically cast it to a specific type.
        /// This differs from the original Properties.getProperty() method in that you
        /// need to specify the desired type (e.g. Double.class) and the default value
        /// is an object of that type, i.e. a double 0.0 instead of the string "0.0".
        /// </summary>
        public static /*<E>*/ E Get<E>(Dictionary<string, string> props, string key, E defaultValue, Type type)
        {
            string value = props[key];
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

        /// <summary>
        /// Load an integer property.  If the key is not present, returns defaultValue.
        /// </summary>
        public static string GetString(Dictionary<string, string> props, string key, string defaultValue)
        {
            string value = props[key];
            if (value != null)
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Load an integer property.  If the key is not present, returns 0.
        /// </summary>
        public static int GetInt(Dictionary<string, string> props, string key)
        {
            return GetInt(props, key, 0);
        }

        /// <summary>
        /// Load an integer property.  If the key is not present, returns defaultValue.
        /// </summary>
        public static int GetInt(Dictionary<string, string> props, string key, int defaultValue)
        {
            string value = props[key];
            if (value != null)
            {
                return int.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Load an integer property as a long. If the key is not present, returns defaultValue.
        /// </summary>
        public static long GetLong(Dictionary<string, string> props, string key, long defaultValue)
        {
            string value = props[key];
            if (value != null)
            {
                return long.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Load a double property.  If the key is not present, returns 0.0.
        /// </summary>
        public static double GetDouble(Dictionary<string, string> props, string key)
        {
            return GetDouble(props, key, 0.0);
        }
        
        /// <summary>
        /// Load a double property.  If the key is not present, returns defaultValue.
        /// </summary>
        public static double GetDouble(Dictionary<string, string> props, string key, double defaultValue)
        {
            string value = props[key];
            if (value != null)
            {
                return Double.Parse(value);
            }
            else
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Load a bool property.  If the key is not present, returns false.
        /// </summary>
        public static bool GetBool(Dictionary<string, string> props, string key)
        {
            return GetBool(props, key, false);
        }

        /// <summary>
        /// Load a bool property.  If the key is not present, returns defaultValue.
        /// </summary>
        public static bool GetBool(Dictionary<string, string> props, string key,
            bool defaultValue)
        {
            string value = props[key];
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
        /*public static int[] getIntArray(Dictionary<string,String> props, string key) {
    Integer[] result = MetaClass.cast(props[key], Integer [].class);
    return ArrayUtils.toPrimitive(result);
  }*/

        /**
   * Loads a comma-separated list of doubles from Properties.  The list cannot include any whitespace.
   */
        /*public static double[] getDoubleArray(Dictionary<string,String> props, string key) {
    Double[] result = MetaClass.cast(props[key], Double [].class);
    return ArrayUtils.toPrimitive(result);
  }*/

        /**
   * Loads a comma-separated list of strings from Properties.  Commas may be quoted if needed, e.g.:
   *    property1 = value1,value2,"a quoted value",'another quoted value'
   *    
   * getStringArray(props, "property1") should return the same thing as
   *    new string[] { "value1", "value2", "a quoted value", "another quoted value" };
   */
        /*public static string[] getStringArray(Dictionary<string,String> props, string key) {
    string[] results = MetaClass.cast(props[key], string [].class);
    if (results == null) {
      results = new string[]{};
    }
    return results;
  }*/

        /*public static string[] getStringArray(Dictionary<string,String> props, string key, string[] defaults) {
    string[] results = MetaClass.cast(props[key], string [].class);
    if (results == null) {
      results = defaults;
    }
    return results;
  }*/

        public class Property
        {
            public string name;
            public string defaultValue;
            public string description;

            public Property(string name, string defaultValue, string description)
            {
                this.name = name;
                this.defaultValue = defaultValue;
                this.description = description;
            }
        }

        /*public static string getSignature(string name, Dictionary<string,String> properties, Dictionary<string,String>[] supportedProperties) {
    string prefix = (name != null && name.Any())? name + ".":"";
    // keep track of all relevant properties for this annotator here!
    StringBuilder sb = new StringBuilder();
    foreach (Dictionary<string,String> p in supportedProperties) {
      string pname = prefix + p.name;
      string pvalue = properties[pname] = p.defaultValue;
      sb.Append(pname).Append(":").Append(pvalue);
    }
    return sb.ToString();
  }*/
    }
}