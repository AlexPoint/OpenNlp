using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    
    /// <summary>
    /// Class for random string things, including output formatting and command line argument parsing.
    /// 
    /// @author Dan Klein
    /// @author Christopher Manning
    /// @author Tim Grow (grow@stanford.edu)
    /// @author Chris Cox
    /// @version 2006/02/03
    /// </summary>
    public static class StringUtils
    {
        public static readonly string[] EMPTY_STRING_ARRAY = new string[0];
        private static readonly string PROP = "prop";
        private static readonly string PROPS = "props";
        private static readonly string PROPERTIES = "properties";
        private static readonly string ARGS = "args";
        private static readonly string ARGUMENTS = "arguments";
        
        /// <summary>
        /// Say whether this regular expression can be found inside this string.
        /// This method provides one of the two "missing" convenience methods 
        /// for regular expressions in the string class in JDK1.4.
        /// This is the one you'll want to use all the time if you're used to Perl.
        /// What were they smoking?
        /// </summary>
        /// <param name="str">string to search for match in</param>
        /// <param name="regex">string to compile as the regular expression</param>
        /// <returns>Whether the regex can be found in str</returns>
        public static bool Find(string str, string regex)
        {
            return Regex.IsMatch(str, regex);
        }
        
        /// <summary>
        /// Convenience method: a case-insensitive variant of Collection.contains
        /// </summary>
        /// <returns>true if s case-insensitively matches a string in c</returns>
        public static bool ContainsIgnoreCase(List<string> c, string s)
        {
            foreach (string squote in c)
            {
                if (squote.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Say whether this regular expression can be found at the beginning of this string. 
        /// This method provides one of the two "missing" convenience methods 
        /// for regular expressions in the string class in JDK1.4.
        /// </summary>
        /// <param name="str">string to search for match at start of</param>
        /// <param name="regex">string to compile as the regular expression</param>
        /// <returns>Whether the regex can be found at the start of str</returns>
        public static bool LookingAt(string str, string regex)
        {
            return Regex.IsMatch(str, "^" + regex);
            //return Pattern.compile(regex).matcher(str).lookingAt();
        }
        
        /// <summary>
        /// Takes a string of the form "x1=y1,x2=y2,..." 
        /// such that each y is an integer and each x is a key.
        /// A string[] s is returned such that s[yn]=xn
        /// </summary>
        /// <param name="map">
        /// A string of the form "x1=y1,x2=y2,..." such that each y is an integer and each x is a key.
        /// </param>
        /// <returns>A string[] s is returned such that s[yn]=xn</returns>
        public static string[] MapStringToArray(string map)
        {
            string[] m = map.Split(new[] {'[', ',', ';', ']'});
            int maxIndex = 0;
            var keys = new string[m.Length];
            var indices = new int[m.Length];
            for (int i = 0; i < m.Length; i++)
            {
                int index = m[i].LastIndexOf('=');
                keys[i] = m[i].Substring(0, index);
                indices[i] = int.Parse(m[i].Substring(index + 1));
                if (indices[i] > maxIndex)
                {
                    maxIndex = indices[i];
                }
            }
            var mapArr = new string[maxIndex + 1];
            //Arrays.fill(mapArr, null);
            for (int i = 0; i < m.Length; i++)
            {
                mapArr[indices[i]] = keys[i];
            }
            return mapArr;
        }

        /// <summary>
        /// Takes a string of the form "x1=y1,x2=y2,..." and returns Map
        /// </summary>
        /// <param name="map">A string of the form "x1=y1,x2=y2,..."</param>
        /// <returns>A Map m is returned such that m.get(xn) = yn</returns>
        public static Dictionary<string, string> MapStringToMap(string map)
        {
            string[] m = map.Split(new[] {'[', ',', ';', ']'});
            var res = new Dictionary<string, string>();
            foreach (string str in m)
            {
                int index = str.LastIndexOf('=');
                string key = str.Substring(0, index);
                string val = str.Substring(index + 1);
                res.Add(key.Trim(), val.Trim());
            }
            return res;
        }

        public static List<Regex> RegexesToPatterns(IEnumerable<string> regexes)
        {
            var patterns = new List<Regex>();
            foreach (string regex in regexes)
            {
                patterns.Add(new Regex(regex));
            }
            return patterns;
        }

        /**
   * Given a pattern and a string, returns a list with the values of the
   * captured groups in the pattern. If the pattern does not match, returns
   * null. Note that this uses Matcher.find() rather than Matcher.matches().
   * If str is null, returns null.
   */
        /*public static List<string> regexGroups(Regex regex, string str) {
    if (str == null) {
      return null;
    }

    Matcher matcher = regex.matcher(str);
    if (!matcher.find()) {
      return null;
    }

    List<string> groups = new List<string>();
    for (int index = 1; index <= matcher.groupCount(); index++) {
      groups.Add(matcher.group(index));
    }

    return groups;
  }*/

        /**
   * Say whether this regular expression matches
   * this string.  This method is the same as the string.matches() method,
   * and is included just to give a call that is parallel to the other
   * static regex methods in this class.
   *
   * @param str   string to search for match at start of
   * @param regex string to compile as the regular expression
   * @return Whether the regex matches the whole of this str
   */
        /*public static bool matches(string str, string regex) {
    return Pattern.compile(regex).matcher(str).matches();
  }*/


        /*public static Set<string> stringToSet(string str, string delimiter)
  {
    Set<string> ret = null;
    if (str != null) {
      string[] fields = str.Split(delimiter);
      ret = Generics.newHashSet(fields.Length);
      for (string field:fields) {
        field = field.Trim();
        ret.Add(field);
      }
    }
    return ret;
  }*/


        /*public static string joinWords(Iterable<? extends HasWord> l, string glue) {
    StringBuilder sb = new StringBuilder();
    bool first = true;
    for (HasWord o : l) {
      if ( ! first) {
        sb.Append(glue);
      } else {
        first = false;
      }
      sb.Append(o.word());
    }
    return sb.ToString();
  }


  public static <E> string join(List<? extends E> l, string glue, Function<E,String> toStringFunc, int start, int end) {
    StringBuilder sb = new StringBuilder();
    bool first = true;
    start = Math.max(start, 0);
    end = Math.min(end, l.size());
    for (int i = start; i < end; i++) {
      if ( ! first) {
        sb.Append(glue);
      } else {
        first = false;
      }
      sb.Append(toStringFunc.apply(l.get(i)));
    }
    return sb.ToString();
  }

  public static string joinWords(List<? extends HasWord> l, string glue, int start, int end) {
    return join(l, glue, in -> in.word(), start, end);
  }

  public static readonly Function<Object,String> DEFAULT_TOSTRING = new Function<Object, string>() {
    @Override
    public string apply(Object in) {
      return in.ToString();
    }
  };

  public static string joinFields(List<? extends CoreMap> l, readonly Class field, readonly string defaultFieldValue,
                                  string glue, int start, int end, readonly Function<Object,String> toStringFunc) {
    return join(l, glue, new Function<CoreMap, string>() {
      public string apply(CoreMap in) {
        Object val = in.get(field);
        return (val != null)? toStringFunc.apply(val):defaultFieldValue;
      }
    }, start, end);
  }

  public static string joinFields(List<? extends CoreMap> l, readonly Class field, readonly string defaultFieldValue,
                                  string glue, int start, int end) {
    return joinFields(l, field, defaultFieldValue, glue, start, end, DEFAULT_TOSTRING);
  }

  public static string joinFields(List<? extends CoreMap> l, readonly Class field, readonly Function<Object,String> toStringFunc) {
    return joinFields(l, field, "-", " ", 0, l.size(), toStringFunc);
  }

  public static string joinFields(List<? extends CoreMap> l, readonly Class field) {
    return joinFields(l, field, "-", " ", 0, l.size());
  }

  public static string joinMultipleFields(List<? extends CoreMap> l, readonly Class[] fields, readonly string defaultFieldValue,
                                          readonly string fieldGlue, string glue, int start, int end, readonly Function<Object,String> toStringFunc) {
    return join(l, glue, new Function<CoreMap, string>() {
      public string apply(CoreMap in) {
        StringBuilder sb = new StringBuilder();
        for (Class field: fields) {
          if (sb.Length() > 0) {
            sb.Append(fieldGlue);
          }
          Object val = in.get(field);
          string str = (val != null)? toStringFunc.apply(val):defaultFieldValue;
          sb.Append(str);
        }
        return sb.ToString();
      }
    }, start, end);
  }

  public static string joinMultipleFields(List<? extends CoreMap> l, readonly Class[] fields, readonly Function<Object,String> toStringFunc) {
    return joinMultipleFields(l, fields, "-", "/", " ", 0, l.size(), toStringFunc);
  }

  public static string joinMultipleFields(List<? extends CoreMap> l, readonly Class[] fields, readonly string defaultFieldValue,
                                          readonly string fieldGlue, string glue, int start, int end) {
    return joinMultipleFields(l, fields, defaultFieldValue, fieldGlue, glue, start, end, DEFAULT_TOSTRING);
  }

  public static string joinMultipleFields(List<? extends CoreMap> l, readonly Class[] fields) {
    return joinMultipleFields(l, fields, "-", "/", " ", 0, l.size());
  }

  /**
   * Joins all the tokens together (more or less) according to their original whitespace.
   * It assumes all whitespace was " "
   * @param tokens list of tokens which implement {@link HasOffset} and {@link HasWord}
   * @return a string of the tokens with the appropriate amount of spacing
   #1#
  public static string joinWithOriginalWhiteSpace(List<CoreLabel> tokens) {
    if (tokens.isEmpty()) {
      return "";
    }

    CoreLabel lastToken = tokens.get(0);
    StringBuilder buffer = new StringBuilder(lastToken.word());

    for (int i = 1; i < tokens.size(); i++) {
      CoreLabel currentToken = tokens.get(i);
      int numSpaces = currentToken.beginPosition() - lastToken.endPosition();
      if (numSpaces < 0) {
        numSpaces = 0;
      }

      buffer.Append(repeat(' ', numSpaces)).Append(currentToken.word());
      lastToken = currentToken;
    }

    return buffer.ToString();
  }

  /**
   * Joins each elem in the {@code Collection} with the given glue.
   * For example, given a list of {@code Integers}, you can create
   * a comma-separated list by calling {@code join(numbers, ", ")}.
   #1#
  public static <X> string join(Iterable<X> l, string glue) {
    StringBuilder sb = new StringBuilder();
    bool first = true;
    for (X o : l) {
      if ( ! first) {
        sb.Append(glue);
      } else {
        first = false;
      }
      sb.Append(o);
    }
    return sb.ToString();
  }

// Omitted; I'm pretty sure this are redundant with the above
//  /**
//   * Joins each elem in the List with the given glue. For example, given a
//   * list
//   * of Integers, you can create a comma-separated list by calling
//   * <tt>join(numbers, ", ")</tt>.
//   #1#
//  public static string join(List l, string glue) {
//    StringBuilder sb = new StringBuilder();
//    for (int i = 0, sz = l.size(); i < sz; i++) {
//      if (i > 0) {
//        sb.Append(glue);
//      }
//      sb.Append(l.get(i).ToString());
//    }
//    return sb.ToString();
//  }

  /**
   * Joins each elem in the array with the given glue. For example, given a
   * list of ints, you can create a comma-separated list by calling
   * <code>join(numbers, ", ")</code>.
   #1#
  public static string join(Object[] elements, string glue) {
    return (join(Arrays.asList(elements), glue));
  }

  /**
   * Joins elems with a space.
   #1#
  public static string join(Iterable<?> l) {
    return join(l, " ");
  }

  /**
   * Joins elements with a space.
   #1#
  public static string join(Object[] elements) {
    return (join(elements, " "));
  }*/
        
        /// <summary>
        /// Splits on whitespace (\\s+).
        /// </summary>
        /// <param name="s">string to split</param>
        public static List<string> SplitOnWhitespaces(string s)
        {
            return Split(s, "\\s+");
        }
        
        /// <summary>
        /// Splits the given string using the given regex as delimiters.
        /// This method is the same as the string.Split() method 
        /// (except it throws the results in a List), and is included 
        /// just to give a call that is parallel to the other static regex methods in this class.
        /// </summary>
        /// <param name="str">string to split up</param>
        /// <param name="regex">string to compile as the regular expression</param>
        /// <returns>List of strings resulting from splitting on the regex</returns>
        public static List<string> Split(string str, string regex)
        {
            return Regex.Split(str, regex).ToList();
        }

        /*public static string[] splitOnChar(string input, char delimiter) {
    // State
    string[] out = new string[input.Length() + 1];
    int nextIndex = 0;
    int lastDelimiterIndex = -1;
    char[] chars = input.toCharArray();
    // Split
    for ( int i = 0; i <= chars.Length; ++i ) {
      if (i >= chars.Length || chars[i] == delimiter) {
        char[] tokenChars = new char[i - (lastDelimiterIndex + 1)];
        System.arraycopy(chars, lastDelimiterIndex + 1, tokenChars, 0, tokenChars.Length);
        out[nextIndex] = new string(tokenChars);
        nextIndex += 1;
        lastDelimiterIndex = i;
      }
    }
    // Clean Result
    string[] trimmedOut = new string[nextIndex];
    System.arraycopy(out, 0, trimmedOut, 0, trimmedOut.Length);
    return trimmedOut;
  }

  /**
   * Splits a string into whitespace tokenized fields based on a delimiter. For example,
   * "aa bb | bb cc | ccc ddd" would be split into "[aa,bb],[bb,cc],[ccc,ddd]" based on
   * the delimiter "|". This method uses the old StringTokenizer class, which is up to
   * 3x faster than the regex-based "split()" methods.
   *
   * @param delimiter
   * @return
   #1#
  public static List<List<string>> splitFieldsFast(string str, string delimiter) {
    List<List<string>> fields = Generics.newArrayList();
    StringTokenizer tokenizer = new StringTokenizer(str.Trim());
    List<string> currentField = Generics.newArrayList();
    while(tokenizer.hasMoreTokens()) {
      string token = tokenizer.nextToken();
      if (token.equals(delimiter)) {
        fields.Add(currentField);
        currentField = Generics.newArrayList();
      } else {
        currentField.Add(token.Trim());
      }
    }
    if (currentField.size() > 0) {
      fields.Add(currentField);
    }
    return fields;
  }

  /** Split a string into tokens.  Because there is a tokenRegex as well as a
   *  separatorRegex (unlike for the conventional split), you can do things
   *  like correctly split quoted strings or parenthesized arguments.
   *  However, it doesn't do the unquoting of quoted strings for you.
   *  An empty string argument is returned at the beginning, if valueRegex
   *  accepts the empty string and str begins with separatorRegex.
   *  But str can end with either valueRegex or separatorRegex and this does
   *  not generate an empty string at the end (indeed, valueRegex need not
   *  even accept the empty string in this case.  However, if it does accept
   *  the empty string and there are multiple trailing separators, then
   *  empty values will be returned.
   *
   *  @param str The string to split
   *  @param valueRegex Must match a token. You may wish to let it match the empty String
   *  @param separatorRegex Must match a separator
   *  @return The List of tokens
   *  @throws IllegalArgumentException if str cannot be tokenized by the two regex
   #1#
  public static List<string> valueSplit(string str, string valueRegex, string separatorRegex) {
    Pattern vPat = Pattern.compile(valueRegex);
    Pattern sPat = Pattern.compile(separatorRegex);
    List<string> ret = new ArrayList<string>();
    while (str.Length() > 0) {
      Matcher vm = vPat.matcher(str);
      if (vm.lookingAt()) {
        ret.Add(vm.group());
        str = str.Substring(vm.end());
        // string got = vm.group();
      } else {
        throw new IllegalArgumentException("valueSplit: " + valueRegex + " doesn't match " + str);
      }
      if (str.Length() > 0) {
        Matcher sm = sPat.matcher(str);
        if (sm.lookingAt()) {
          str = str.Substring(sm.end());
          // string got = sm.group();
        } else {
          throw new IllegalArgumentException("valueSplit: " + separatorRegex + " doesn't match " + str);
        }
      }
    } // end while
    return ret;
  }*/

        /// <summary>
        /// Return a string of length a minimum of totalChars characters 
        /// by padding the input string str at the right end with spaces.
        /// If str is already longer than totalChars, it is returned unchanged.
        /// </summary>
        public static string Pad(string str, int totalChars)
        {
            if (str == null)
            {
                str = "null";
            }
            int slen = str.Length;
            var sb = new StringBuilder(str);
            for (int i = 0; i < totalChars - slen; i++)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Pads the ToString value of the given Object.
        /// </summary>
        public static string Pad(Object obj, int totalChars)
        {
            return Pad(obj.ToString(), totalChars);
        }
        
        /// <summary>
        /// Pad or trim so as to produce a string of exactly a certain length.
        /// </summary>
        /// <param name="str">The string to be padded or truncated</param>
        /// <param name="num">The desired length</param>
        public static string PadOrTrim(string str, int num)
        {
            if (str == null)
            {
                str = "null";
            }
            int leng = str.Length;
            if (leng < num)
            {
                var sb = new StringBuilder(str);
                for (int i = 0; i < num - leng; i++)
                {
                    sb.Append(' ');
                }
                return sb.ToString();
            }
            else if (leng > num)
            {
                return str.Substring(0, num);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Pad or trim so as to produce a string of exactly a certain length.
        /// </summary>
        /// <param name="str">The string to be padded or truncated</param>
        /// <param name="num">The desired length</param>
        public static string PadLeftOrTrim(string str, int num)
        {
            if (str == null)
            {
                str = "null";
            }
            int leng = str.Length;
            if (leng < num)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < num - leng; i++)
                {
                    sb.Append(' ');
                }
                sb.Append(str);
                return sb.ToString();
            }
            else if (leng > num)
            {
                return str.Substring(str.Length - num);
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Pad or trim the ToString value of the given Object.
        /// </summary>
        public static string PadOrTrim(Object obj, int totalChars)
        {
            return PadOrTrim(obj.ToString(), totalChars);
        }

        /// <summary>
        /// Pads the given string to the left with the given character 
        /// to ensure that it's at least totalChars long.
        /// </summary>
        public static string PadLeft(string str, int totalChars, char ch)
        {
            if (str == null)
            {
                str = "null";
            }
            var sb = new StringBuilder();
            for (int i = 0, num = totalChars - str.Length; i < num; i++)
            {
                sb.Append(ch);
            }
            sb.Append(str);
            return sb.ToString();
        }

        /// <summary>
        /// Pads the given string to the left with spaces to ensure 
        /// that it's at least totalChars long.
        /// </summary>
        public static string PadLeft(string str, int totalChars)
        {
            return PadLeft(str, totalChars, ' ');
        }


        public static string PadLeft(Object obj, int totalChars)
        {
            return PadLeft(obj.ToString(), totalChars);
        }

        public static string PadLeft(int i, int totalChars)
        {
            return PadLeft(i, totalChars);
        }

        public static string PadLeft(double d, int totalChars)
        {
            return PadLeft(d, totalChars);
        }
        
        /// <summary>
        /// Returns s if it's at most maxWidth chars, otherwise chops right side to fit.
        /// </summary>
        public static string Trim(string s, int maxWidth)
        {
            if (s.Length <= maxWidth)
            {
                return (s);
            }
            return (s.Substring(0, maxWidth));
        }

        public static string Trim(Object obj, int maxWidth)
        {
            return Trim(obj.ToString(), maxWidth);
        }

        public static string Repeat(string s, int times)
        {
            if (times == 0)
            {
                return "";
            }
            var sb = new StringBuilder(times*s.Length);
            for (int i = 0; i < times; i++)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static string Repeat(char ch, int times)
        {
            if (times == 0)
            {
                return "";
            }
            var sb = new StringBuilder(times);
            for (int i = 0; i < times; i++)
            {
                sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a "clean" version of the given filename in which spaces 
        /// have been converted to dashes and all non-alphanumeric chars are underscores.
        /// </summary>
        public static string FileNameClean(string s)
        {
            char[] chars = s.ToCharArray();
            var sb = new StringBuilder();
            foreach (char c in chars)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '_'))
                {
                    sb.Append(c);
                }
                else
                {
                    if (c == ' ' || c == '-')
                    {
                        sb.Append('_');
                    }
                    else
                    {
                        sb.Append('x').Append((int) c).Append('x');
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the index of the <i>n</i>th occurrence of ch in s, 
        /// or -1 if there are less than n occurrences of ch.
        /// </summary>
        public static int NthIndex(string s, char ch, int n)
        {
            int index = 0;
            for (int i = 0; i < n; i++)
            {
                // if we're already at the end of the string,
                // and we need to find another ch, return -1
                if (index == s.Length - 1)
                {
                    return -1;
                }
                index = s.IndexOf(ch, index + 1);
                if (index == -1)
                {
                    return (-1);
                }
            }
            return index;
        }

        /// <summary>
        /// This returns a string from decimal digit smallestDigit to decimal digit biggest digit. 
        /// Smallest digit is labeled 1, and the limits are inclusive.
        /// </summary>
        public static string Truncate(int n, int smallestDigit, int biggestDigit)
        {
            int numDigits = biggestDigit - smallestDigit + 1;
            var result = new char[numDigits];
            for (int j = 1; j < smallestDigit; j++)
            {
                n = n/10;
            }
            for (int j = numDigits - 1; j >= 0; j--)
            {
                result[j] = (char) (n%10);
                n = n/10;
            }
            return new string(result);
        }

        /**
   * Parses command line arguments into a Map. Arguments of the form
   * <p/>
   * -flag1 arg1a arg1b ... arg1m -flag2 -flag3 arg3a ... arg3n
   * <p/>
   * will be parsed so that the flag is a key in the Map (including
   * the hyphen) and its value will be a {@link String}[] containing
   * the optional arguments (if present).  The non-flag values not
   * captured as flag arguments are collected into a string[] array
   * and returned as the value of <code>null</code> in the Map.  In
   * this invocation, flags cannot take arguments, so all the {@link
   * String} array values other than the value for <code>null</code>
   * will be zero-length.
   *
   * @param args A command-line arguments array
   * @return a {@link Map} of flag names to flag argument {@link
   *         String} arrays.
   */
        /*public static Map<string, string[]> argsToMap(string[] args) {
    return argsToMap(args, Collections.<string,Integer>emptyMap());
  }*/

        /**
   * Parses command line arguments into a Map. Arguments of the form
   * <p/>
   * -flag1 arg1a arg1b ... arg1m -flag2 -flag3 arg3a ... arg3n
   * <p/>
   * will be parsed so that the flag is a key in the Map (including
   * the hyphen) and its value will be a {@link String}[] containing
   * the optional arguments (if present).  The non-flag values not
   * captured as flag arguments are collected into a string[] array
   * and returned as the value of <code>null</code> in the Map.  In
   * this invocation, the maximum number of arguments for each flag
   * can be specified as an {@link Integer} value of the appropriate
   * flag key in the <code>flagsToNumArgs</code> {@link Map}
   * argument. (By default, flags cannot take arguments.)
   * <p/>
   * Example of usage:
   * <p/>
   * <code>
   * Map flagsToNumArgs = new HashMap();
   * flagsToNumArgs.put("-x",new Integer(2));
   * flagsToNumArgs.put("-d",new Integer(1));
   * Map result = argsToMap(args,flagsToNumArgs);
   * </code>
   * <p/>
   * If a given flag appears more than once, the extra args are appended to
   * the string[] value for that flag.
   *
   * @param args           the argument array to be parsed
   * @param flagsToNumArgs a {@link Map} of flag names to {@link Integer}
   *                       values specifying the number of arguments
   *                       for that flag (default min 0, max 1).
   * @return a {@link Map} of flag names to flag argument {@link String}
   */
        /*public static Map<string, string[]> argsToMap(string[] args, Map<string, Integer> flagsToNumArgs) {
    Map<string, string[]> result = Generics.newHashMap();
    List<string> remainingArgs = new ArrayList<string>();
    for (int i = 0; i < args.Length; i++) {
      string key = args[i];
      if (key.charAt(0) == '-') { // found a flag
        Integer numFlagArgs = flagsToNumArgs.get(key);
        int max = numFlagArgs == null ? 1 : numFlagArgs.intValue();
        int min = numFlagArgs == null ? 0 : numFlagArgs.intValue();
        List<string> flagArgs = new ArrayList<string>();
        for (int j = 0; j < max && i + 1 < args.Length && (j < min || args[i + 1].Length() == 0 || args[i + 1].charAt(0) != '-'); i++, j++) {
          flagArgs.Add(args[i + 1]);
        }
        if (result.containsKey(key)) { // append the second specification into the args.
          string[] newFlagArg = new string[result.get(key).Length + flagsToNumArgs.get(key)];
          int oldNumArgs = result.get(key).Length;
          System.arraycopy(result.get(key), 0, newFlagArg, 0, oldNumArgs);
          for (int j = 0; j < flagArgs.size(); j++) {
            newFlagArg[j + oldNumArgs] = flagArgs.get(j);
          }
          result.put(key, newFlagArg);
        } else {
          result.put(key, flagArgs.toArray(new string[flagArgs.size()]));
        }
      } else {
        remainingArgs.Add(args[i]);
      }
    }
    result.put(null, remainingArgs.toArray(new string[remainingArgs.size()]));
    return result;
  }*/

        /**
   * In this version each flag has zero or one argument. It has one argument
   * if there is a thing following a flag that does not begin with '-'.  See
   * {@link #argsToProperties(string[], Map)} for full documentation.
   *
   * @param args Command line arguments
   * @return A Properties object representing the arguments.
   */
        /*public static Properties argsToProperties(String... args) {
    return argsToProperties(args, Collections.<string,Integer>emptyMap());
  }*/

        /**
   * Analogous to {@link #argsToMap}.  However, there are several key differences between this method and {@link #argsToMap}:
   * <ul>
   * <li> Hyphens are stripped from flag names </li>
   * <li> Since Properties objects are string to string mappings, the default number of arguments to a flag is
   * assumed to be 1 and not 0. </li>
   * <li> Furthermore, the list of arguments not bound to a flag is mapped to the "" property, not null </li>
   * <li> The special flags "-prop", "-props", or "-properties" will load the property file specified by its argument. </li>
   * <li> The value for flags without arguments is set to "true" </li>
   * <li> If a flag has multiple arguments, the value of the property is all
   * of the arguments joined together with a space (" ") character between
   * them.</li>
   * <li> The value strings are trimmed so trailing spaces do not stop you from loading a file</li>
   * </ul>
   *
   * @param args Command line arguments
   * @param flagsToNumArgs Map of how many arguments flags should have. The keys are without the minus signs.
   * @return A Properties object representing the arguments.
   */
        /*public static Properties argsToProperties(string[] args, Map<string,Integer> flagsToNumArgs) {
    Properties result = new Properties();
    List<string> remainingArgs = new ArrayList<string>();
    for (int i = 0; i < args.Length; i++) {
      string key = args[i];
      if (key.Length() > 0 && key.charAt(0) == '-') { // found a flag
        if (key.Length() > 1 && key.charAt(1) == '-')
          key = key.Substring(2); // strip off 2 hyphens
        else
          key = key.Substring(1); // strip off the hyphen

        Integer maxFlagArgs = flagsToNumArgs.get(key);
        int max = maxFlagArgs == null ? 1 : maxFlagArgs;
        int min = maxFlagArgs == null ? 0 : maxFlagArgs;
        List<string> flagArgs = new ArrayList<string>();
        // cdm oct 2007: add length check to allow for empty string argument!
        for (int j = 0; j < max && i + 1 < args.Length && (j < min || args[i + 1].Length() == 0 || args[i + 1].charAt(0) != '-'); i++, j++) {
          flagArgs.Add(args[i + 1]);
        }
        if (flagArgs.isEmpty()) {
          result.setProperty(key, "true");
        } else {
          result.setProperty(key, join(flagArgs, " "));
          if (key.equalsIgnoreCase(PROP) || key.equalsIgnoreCase(PROPS) || key.equalsIgnoreCase(PROPERTIES) || key.equalsIgnoreCase(ARGUMENTS) || key.equalsIgnoreCase(ARGS))
          {
            try {
              InputStream is = IOUtils.getInputStreamFromURLOrClasspathOrFileSystem(result.getProperty(key));
              InputStreamReader reader = new InputStreamReader(is, "utf-8");
              result.remove(key); // location of this line is critical
              result.load(reader);
              // trim all values
              for(Object propKey : result.keySet()){
                string newVal = result.getProperty((string)propKey);
                result.setProperty((string)propKey,newVal.Trim());
              }
              is.close();
            } catch (IOException e) {
              result.remove(key);
              throw new RuntimeIOException(e);
            }
          }
        }
      } else {
        remainingArgs.Add(args[i]);
      }
    }
    if (!remainingArgs.isEmpty()) {
      result.setProperty("", join(remainingArgs, " "));
    }

    if (result.containsKey(PROP)) {
      string file = result.getProperty(PROP);
      result.remove(PROP);
      Properties toAdd = argsToProperties(new string[]{"-prop", file});
      for (Enumeration<?> e = toAdd.propertyNames(); e.hasMoreElements(); ) {
        string key = (string) e.nextElement();
        string val = toAdd.getProperty(key);
        if (!result.containsKey(key)) {
          result.setProperty(key, val);
        }
      }
    }

    return result;
  }*/


        /**
   * This method reads in properties listed in a file in the format prop=value, one property per line.
   * Although <code>Properties.load(InputStream)</code> exists, I implemented this method to trim the lines,
   * something not implemented in the <code>load()</code> method.
   * @param filename A properties file to read
   * @return The corresponding Properties object
   */
        /*public static Properties propFileToProperties(string filename) {
    Properties result = new Properties();
    try {
      InputStream is = new BufferedInputStream(new FileInputStream(filename));
      result.load(is);
      // trim all values
      for (Object propKey : result.keySet()){
        string newVal = result.getProperty((string)propKey);
        result.setProperty((string)propKey,newVal.Trim());
      }
      is.close();
      return result;
    } catch (IOException e) {
      throw new RuntimeIOException("propFileToProperties could not read properties file: " + filename, e);
    }
  }*/
        
        /// <summary>
        /// Converts a comma-separated string (with whitespace optionally allowed after the comma) 
        /// representing properties to a Properties object.
        /// Each property is "property=value".
        /// The value for properties without an explicitly given value is set to "true".
        /// This can be used for a 2nd level of properties, for example, 
        /// when you have a commandline argument like "-outputOptions style=xml,tags".
        /// </summary>
        public static Dictionary<string, string> StringToProperties(string str)
        {
            var result = new Dictionary<string, string>();
            return StringToProperties(str, result);
        }
        
        /// <summary>
        /// This method updates a Properties object based on a comma-separated string 
        /// (with whitespace optionally allowed after the comma) representing properties to a Properties object.
        /// Each property is "property=value".
        /// The value for properties without an explicitly given value is set to "true".
        /// </summary>
        public static Dictionary<string, string> StringToProperties(string str, Dictionary<string, string> props)
        {
            string[] propsStr = Regex.Split(str.Trim(), ",\\s*");
            foreach (string term in propsStr)
            {
                int divLoc = term.IndexOf('=');
                string key;
                string value;
                if (divLoc >= 0)
                {
                    key = term.Substring(0, divLoc).Trim();
                    value = term.Substring(divLoc + 1).Trim();
                }
                else
                {
                    key = term.Trim();
                    value = "true";
                }
                props[key] = value;
            }
            return props;
        }

        /**
   * If any of the given list of properties are not found, returns the
   * name of that property.  Otherwise, returns null.
   */
        /*public static string checkRequiredProperties(Properties props,
                                               string ... requiredProps) {
    for (string required : requiredProps) {
      if (props.getProperty(required) == null) {
        return required;
      }
    }
    return null;
  }*/


        /**
   * Prints to a file.  If the file already exists, appends if
   * <code>append=true</code>, and overwrites if <code>append=false</code>.
   */
        /*public static void printToFile(File file, string message, bool append,
                                 bool printLn, string encoding) {
    PrintWriter pw = null;
    try {
      Writer fw;
      if (encoding != null) {
        fw = new OutputStreamWriter(new FileOutputStream(file, append),
                                         encoding);
      } else {
        fw = new FileWriter(file, append);
      }
      pw = new PrintWriter(fw);
      if (printLn) {
        pw.println(message);
      } else {
        pw.print(message);
      }
    } catch (Exception e) {
      e.printStackTrace();
    } readonlyly {
      if (pw != null) {
        pw.flush();
        pw.close();
      }
    }
  }*/


        /**
   * Prints to a file.  If the file already exists, appends if
   * <code>append=true</code>, and overwrites if <code>append=false</code>.
   */
        /*public static void printToFileLn(File file, string message, bool append) {
    PrintWriter pw = null;
    try {
      Writer fw = new FileWriter(file, append);
      pw = new PrintWriter(fw);
      pw.println(message);
    } catch (Exception e) {
      e.printStackTrace();
    } readonlyly {
      if (pw != null) {
        pw.flush();
        pw.close();
      }
    }
  }*/

        /**
   * Prints to a file.  If the file already exists, appends if
   * <code>append=true</code>, and overwrites if <code>append=false</code>.
   */
        /*public static void printToFile(File file, string message, bool append) {
    PrintWriter pw = null;
    try {
      Writer fw = new FileWriter(file, append);
      pw = new PrintWriter(fw);
      pw.print(message);
    } catch (Exception e) {
      e.printStackTrace();
    } readonlyly {
      if (pw != null) {
        pw.flush();
        pw.close();
      }
    }
  }*/


        /**
   * Prints to a file.  If the file does not exist, rewrites the file;
   * does not append.
   */
        /*public static void printToFile(File file, string message) {
    printToFile(file, message, false);
  }*/

        /**
   * Prints to a file.  If the file already exists, appends if
   * <code>append=true</code>, and overwrites if <code>append=false</code>
   */
        /*public static void printToFile(string filename, string message, bool append) {
    printToFile(new File(filename), message, append);
  }*/

        /**
   * Prints to a file.  If the file already exists, appends if
   * <code>append=true</code>, and overwrites if <code>append=false</code>
   */
        /*public static void printToFileLn(string filename, string message, bool append) {
    printToFileLn(new File(filename), message, append);
  }*/


        /**
   * Prints to a file.  If the file does not exist, rewrites the file;
   * does not append.
   */
        /*public static void printToFile(string filename, string message) {
    printToFile(new File(filename), message, false);
  }*/

        /**
   * A simpler form of command line argument parsing.
   * Dan thinks this is highly superior to the overly complexified code that
   * comes before it.
   * Parses command line arguments into a Map. Arguments of the form
   * -flag1 arg1 -flag2 -flag3 arg3
   * will be parsed so that the flag is a key in the Map (including the hyphen)
   * and the
   * optional argument will be its value (if present).
   *
   * @return A Map from keys to possible values (string or null)
   */
        /*@SuppressWarnings("unchecked")
  public static Map<string, string> parseCommandLineArguments(string[] args) {
    return (Map)parseCommandLineArguments(args, false);
  }*/

        /**
   * A simpler form of command line argument parsing.
   * Dan thinks this is highly superior to the overly complexified code that
   * comes before it.
   * Parses command line arguments into a Map. Arguments of the form
   * -flag1 arg1 -flag2 -flag3 arg3
   * will be parsed so that the flag is a key in the Map (including the hyphen)
   * and the
   * optional argument will be its value (if present).
   * In this version, if the argument is numeric, it will be a Double value
   * in the map, not a string.
   *
   * @return A Map from keys to possible values (string or null)
   */
        /*public static Map<string, Object> parseCommandLineArguments(string[] args, bool parseNumbers) {
    Map<string, Object> result = Generics.newHashMap();
    for (int i = 0; i < args.Length; i++) {
      string key = args[i];
      if (key.charAt(0) == '-') {
        if (i + 1 < args.Length) {
          string value = args[i + 1];
          if (value.charAt(0) != '-') {
            if (parseNumbers) {
              Object numericValue = value;
              try {
                numericValue = Double.parseDouble(value);
              } catch (NumberFormatException e2) {
                // ignore
              }
              result.put(key, numericValue);
            } else {
              result.put(key, value);
            }
            i++;
          } else {
            result.put(key, null);
          }
        } else {
          result.put(key, null);
        }
      }
    }
    return result;
  }*/

        public static string StripNonAlphaNumerics(string orig)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < orig.Length; i++)
            {
                char c = orig[i];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /*public static string stripSGML(string orig) {
      Pattern sgmlPattern = Pattern.compile("<.*?>", Pattern.DOTALL);
      Matcher sgmlMatcher = sgmlPattern.matcher(orig);
      return sgmlMatcher.replaceAll("");
  }*/

        public static string EscapeString(string s, char[] charsToEscape, char escapeChar)
        {
            var result = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == escapeChar)
                {
                    result.Append(escapeChar);
                }
                else
                {
                    foreach (char charToEscape in charsToEscape)
                    {
                        if (c == charToEscape)
                        {
                            result.Append(escapeChar);
                            break;
                        }
                    }
                }
                result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// This function splits the string s into multiple strings using the splitChar.
        /// However, it provides a quoting facility: it is possible to quote strings with the quoteChar.
        /// If the quoteChar occurs within the quotedExpression, it must be prefaced by the escapeChar.
        /// This routine can be useful for processing a line of a CSV file.
        /// </summary>
        /// <param name="s">The string to split into fields. Cannot be null.</param>
        /// <param name="splitChar">The character to split on</param>
        /// <param name="quoteChar">The character to quote items with</param>
        /// <param name="escapeChar">The character to escape the quoteChar with</param>
        /// <returns>An array of strings that s is split into</returns>
        public static string[] SplitOnCharWithQuoting(string s, char splitChar, char quoteChar, char escapeChar)
        {
            var result = new List<string>();
            int i = 0;
            int length = s.Length;
            var b = new StringBuilder();
            while (i < length)
            {
                char curr = s[i];
                if (curr == splitChar)
                {
                    // add last buffer
                    // cdm 2014: Do this even if the field is empty!
                    // if (b.Length() > 0) {
                    result.Add(b.ToString());
                    b = new StringBuilder();
                    // }
                    i++;
                }
                else if (curr == quoteChar)
                {
                    // find next instance of quoteChar
                    i++;
                    while (i < length)
                    {
                        curr = s[i];
                        // mrsmith: changed this condition from
                        // if (curr == escapeChar) {
                        if ((curr == escapeChar) && (i + 1 < length) && (s[i + 1] == quoteChar))
                        {
                            b.Append(s[i + 1]);
                            i += 2;
                        }
                        else if (curr == quoteChar)
                        {
                            i++;
                            break; // break this loop
                        }
                        else
                        {
                            b.Append(s[i]);
                            i++;
                        }
                    }
                }
                else
                {
                    b.Append(curr);
                    i++;
                }
            }
            // RFC 4180 disallows readonly comma. At any rate, don't produce a field after it unless non-empty
            if (b.Length > 0)
            {
                result.Add(b.ToString());
            }
            return result.ToArray();
        }

        /**
   * Computes the longest common substring of s and t.
   * The longest common substring of a and b is the longest run of
   * characters that appear in order inside both a and b. Both a and b
   * may have other extraneous characters along the way. This is like
   * edit distance but with no substitution and a higher number means
   * more similar. For example, the LCS of "abcD" and "aXbc" is 3 (abc).
   */
        /*public static int longestCommonSubstring(string s, string t) {
    int[][] d; // matrix
    int n; // length of s
    int m; // length of t
    int i; // iterates through s
    int j; // iterates through t
    // int cost; // cost
    // Step 1
    n = s.Length;
    m = t.Length;
    if (n == 0) {
      return 0;
    }
    if (m == 0) {
      return 0;
    }
    d = new int[n + 1][m + 1];
    // Step 2
    for (i = 0; i <= n; i++) {
      d[i][0] = 0;
    }
    for (j = 0; j <= m; j++) {
      d[0][j] = 0;
    }
    // Step 3
    for (i = 1; i <= n; i++) {
      char s_i = s[i - 1]; // ith character of s
// Step 4
      for (j = 1; j <= m; j++) {
        char t_j = t[j - 1]; // jth character of t
// Step 5
        // js: if the chars match, you can get an extra point
        // otherwise you have to skip an insertion or deletion (no subs)
        if (s_i == t_j) {
          d[i][j] = SloppyMath.max(d[i - 1][j], d[i][j - 1], d[i - 1][j - 1] + 1);
        } else {
          d[i][j] = Math.Max(d[i - 1][j], d[i][j - 1]);
        }
      }
    }
    // Step 7
    return d[n][m];
  }*/
        
        /// <summary>
        /// Computes the longest common contiguous substring of s and t.
        /// The LCCS is the longest run of characters that appear consecutively in
        /// both s and t. For instance, the LCCS of "color" and "colour" is 4, because of "colo".
        /// </summary>
        public static int LongestCommonContiguousSubstring(string s, string t)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
            {
                return 0;
            }
            int M = s.Length;
            int N = t.Length;
            var d = new int[M + 1, N + 1];
            for (int j = 0; j <= N; j++)
            {
                d[0, j] = 0;
            }
            for (int i = 0; i <= M; i++)
            {
                d[i, 0] = 0;
            }

            int max = 0;
            for (int i = 1; i <= M; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    if (s[i - 1] == t[j - 1])
                    {
                        d[i, j] = d[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        d[i, j] = 0;
                    }

                    if (d[i, j] > max)
                    {
                        max = d[i, j];
                    }
                }
            }
            return max;
        }

        /**
   * Computes the Levenshtein (edit) distance of the two given Strings.
   * This method doesn't allow transposition, so one character transposed between two strings has a cost of 2 (one insertion, one deletion).
   * The EditDistance class also implements the Levenshtein distance, but does allow transposition.
   */
        /*public static int editDistance(string s, string t) {
    // Step 1
    int n = s.Length; // length of s
    int m = t.Length; // length of t
    if (n == 0) {
      return m;
    }
    if (m == 0) {
      return n;
    }
    int[,] d = new int[n + 1,m + 1]; // matrix
    // Step 2
    for (int i = 0; i <= n; i++) {
      d[i,0] = i;
    }
    for (int j = 0; j <= m; j++) {
      d[0,j] = j;
    }
    // Step 3
    for (int i = 1; i <= n; i++) {
      char s_i = s[i - 1]; // ith character of s
      // Step 4
      for (int j = 1; j <= m; j++) {
        char t_j = t[j - 1]; // jth character of t
        // Step 5
        int cost; // cost
        if (s_i == t_j) {
          cost = 0;
        } else {
          cost = 1;
        }
        // Step 6
        d[i,j] = SloppyMath.min(d[i - 1,j] + 1, d[i,j - 1] + 1, d[i - 1,j - 1] + cost);
      }
    }

    // Step 7
    return d[n,m];
  }*/

        /// <summary>
        /// Computes the WordNet 2.0 POS tag corresponding to the PTB POS tag s
        /// </summary>
        /// <param name="s">a Penn TreeBank POS tag.</param>
        /// <returns></returns>
        public static string PennPosToWordnetPos(string s)
        {
            if (Regex.IsMatch(s, "NN|NNP|NNS|NNPS"))
            {
                return "noun";
            }
            if (Regex.IsMatch(s, "VB|VBD|VBG|VBN|VBZ|VBP|MD"))
            {
                return "verb";
            }
            if (Regex.IsMatch(s, "JJ|JJR|JJS|CD"))
            {
                return "adjective";
            }
            if (Regex.IsMatch(s, "RB|RBR|RBS|RP|WRB"))
            {
                return "adverb";
            }
            return null;
        }

        /**
   * Returns a short class name for an object.
   * This is the class name stripped of any package name.
   *
   * @return The name of the class minus a package name, for example
   *         <code>ArrayList</code>
   */
        /*public static string getShortClassName(Object o) {
    if (o == null) {
      return "null";
    }
    string name = o.getClass().getName();
    int index = name.LastIndexOf('.');
    if (index >= 0) {
      name = name.Substring(index + 1);
    }
    return name;
  }*/


        /**
   * Converts a tab delimited string into an object with given fields
   * Requires the object has setXxx functions for the specified fields
   *
   * @param objClass Class of object to be created
   * @param str string to convert
   * @param delimiterRegex delimiter regular expression
   * @param fieldNames fieldnames
   * @param <T> type to return
   * @return Object created from string
   */
        /*public static <T> T columnStringToObject(Class objClass, string str, string delimiterRegex, string[] fieldNames)
          throws InstantiationException, IllegalAccessException, NoSuchFieldException, NoSuchMethodException, InvocationTargetException
  {
    Pattern delimiterPattern = Pattern.compile(delimiterRegex);
    return StringUtils.<T>columnStringToObject(objClass, str, delimiterPattern, fieldNames);
  }*/

        /**
   * Converts a tab delimited string into an object with given fields
   * Requires the object has public access for the specified fields
   *
   * @param objClass Class of object to be created
   * @param str string to convert
   * @param delimiterPattern delimiter
   * @param fieldNames fieldnames
   * @param <T> type to return
   * @return Object created from string
   */
        /*public static <T> T columnStringToObject(Class<?> objClass, string str, Pattern delimiterPattern, string[] fieldNames)
          throws InstantiationException, IllegalAccessException, NoSuchMethodException, NoSuchFieldException, InvocationTargetException
  {
    string[] fields = delimiterPattern.Split(str);
    T item = ErasureUtils.<T>uncheckedCast(objClass.newInstance());
    for (int i = 0; i < fields.Length; i++) {
      try {
        Field field = objClass.getDeclaredField(fieldNames[i]);
        field.set(item, fields[i]);
      } catch (IllegalAccessException ex) {
        Method method = objClass.getDeclaredMethod("set" + StringUtils.capitalize(fieldNames[i]), string.class);
        method.invoke(item, fields[i]);
      }
    }
    return item;
  }*/

        /**
   * Converts an object into a tab delimited string with given fields
   * Requires the object has public access for the specified fields
   *
   * @param object Object to convert
   * @param delimiter delimiter
   * @param fieldNames fieldnames
   * @return string representing object
   */
        /*public static string objectToColumnString(Object object, string delimiter, string[] fieldNames)
          throws IllegalAccessException, NoSuchFieldException, NoSuchMethodException, InvocationTargetException
  {
    StringBuilder sb = new StringBuilder();
    for (string fieldName : fieldNames) {
      if (sb.Length() > 0) {
        sb.Append(delimiter);
      }
      try {
        Field field = object.getClass().getDeclaredField(fieldName);
        sb.Append(field.get(object));
      } catch (IllegalAccessException ex) {
        Method method = object.getClass().getDeclaredMethod("get" + StringUtils.capitalize(fieldName));
        sb.Append(method.invoke(object));
      }
    }
    return sb.ToString();
  }*/

        /// <summary>
        /// Uppercases the first character of a string.
        /// </summary>
        /// <param name="s">a string to capitalize</param>
        /// <returns>a capitalized version of the string</returns>
        public static string Capitalize(string s)
        {
            if (char.IsLower(s[0]))
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// Check if a string begins with an uppercase.
        /// </summary>
        /// <returns>true if the string is capitalized, false otherwise</returns>
        public static bool IsCapitalized(string s)
        {
            return (char.IsUpper(s[0]));
        }

        public static string SearchAndReplace(string text, string from, string to)
        {
            from = EscapeString(from, new char[] {'.', '[', ']', '\\'}, '\\'); // special chars in regex
            var res = Regex.Replace(text, from, to);
            return res;
        }
        
        /// <summary>
        /// Returns an HTML table containing the matrix of strings passed in.
        /// The first dimension of the matrix should represent the rows, and the second dimension the columns.
        /// </summary>
        public static string MakeHtmlTable(string[][] table, string[] rowLabels, string[] colLabels)
        {
            var buff = new StringBuilder();
            buff.Append("<table class=\"auto\" border=\"1\" cellspacing=\"0\">\n");
            // top row
            buff.Append("<tr>\n");
            buff.Append("<td></td>\n"); // the top left cell
            for (int j = 0; j < table[0].Length; j++)
            {
                // assume table is a rectangular matrix
                buff.Append("<td class=\"label\">").Append(colLabels[j]).Append("</td>\n");
            }
            buff.Append("</tr>\n");
            // all other rows
            for (int i = 0; i < table.Length; i++)
            {
                // one row
                buff.Append("<tr>\n");
                buff.Append("<td class=\"label\">").Append(rowLabels[i]).Append("</td>\n");
                for (int j = 0; j < table[i].Length; j++)
                {
                    buff.Append("<td class=\"data\">");
                    buff.Append(((table[i][j] != null) ? table[i][j] : ""));
                    buff.Append("</td>\n");
                }
                buff.Append("</tr>\n");
            }
            buff.Append("</table>");
            return buff.ToString();
        }

        /// <summary>
        /// Returns a text table containing the matrix of objects passed in.
        /// The first dimension of the matrix should represent the rows, 
        /// and the second dimension the columns. Each object is printed in a cell with ToString().
        /// The printing may be padded with spaces on the left and then on the right 
        /// to ensure that the string form is of length at least padLeft or padRight.
        /// If tsv is true, a tab is put between columns.
        /// </summary>
        /// <returns>A string form of the table</returns>
        public static string MakeTextTable(Object[][] table, Object[] rowLabels, Object[] colLabels, int padLeft,
            int padRight, bool tsv)
        {
            var buff = new StringBuilder();
            // top row
            buff.Append(MakeAsciiTableCell("", padLeft, padRight, tsv)); // the top left cell
            for (int j = 0; j < table[0].Length; j++)
            {
                // assume table is a rectangular matrix
                buff.Append(MakeAsciiTableCell(colLabels[j], padLeft, padRight, (j != table[0].Length - 1) && tsv));
            }
            buff.Append('\n');
            // all other rows
            for (int i = 0; i < table.Length; i++)
            {
                // one row
                buff.Append(MakeAsciiTableCell(rowLabels[i], padLeft, padRight, tsv));
                for (int j = 0; j < table[i].Length; j++)
                {
                    buff.Append(MakeAsciiTableCell(table[i][j], padLeft, padRight, (j != table[0].Length - 1) && tsv));
                }
                buff.Append('\n');
            }
            return buff.ToString();
        }

        /// <summary>
        /// The cell string is the string representation of the object.
        ///  If padLeft is greater than 0, it is padded. Ditto right
        /// </summary>
        private static string MakeAsciiTableCell(Object obj, int padLeft, int padRight, bool tsv)
        {
            string result = obj.ToString();
            if (padLeft > 0)
            {
                result = StringUtils.PadLeft(result, padLeft);
            }
            if (padRight > 0)
            {
                result = Pad(result, padRight);
            }
            if (tsv)
            {
                result = result + '\t';
            }
            return result;
        }
        
        public static string ToAscii(string s)
        {
            var b = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c > 127)
                {
                    string result = "?";
                    if (c >= 0x00c0 && c <= 0x00c5)
                    {
                        result = "A";
                    }
                    else if (c == 0x00c6)
                    {
                        result = "AE";
                    }
                    else if (c == 0x00c7)
                    {
                        result = "C";
                    }
                    else if (c >= 0x00c8 && c <= 0x00cb)
                    {
                        result = "E";
                    }
                    else if (c >= 0x00cc && c <= 0x00cf)
                    {
                        result = "F";
                    }
                    else if (c == 0x00d0)
                    {
                        result = "D";
                    }
                    else if (c == 0x00d1)
                    {
                        result = "N";
                    }
                    else if (c >= 0x00d2 && c <= 0x00d6)
                    {
                        result = "O";
                    }
                    else if (c == 0x00d7)
                    {
                        result = "x";
                    }
                    else if (c == 0x00d8)
                    {
                        result = "O";
                    }
                    else if (c >= 0x00d9 && c <= 0x00dc)
                    {
                        result = "U";
                    }
                    else if (c == 0x00dd)
                    {
                        result = "Y";
                    }
                    else if (c >= 0x00e0 && c <= 0x00e5)
                    {
                        result = "a";
                    }
                    else if (c == 0x00e6)
                    {
                        result = "ae";
                    }
                    else if (c == 0x00e7)
                    {
                        result = "c";
                    }
                    else if (c >= 0x00e8 && c <= 0x00eb)
                    {
                        result = "e";
                    }
                    else if (c >= 0x00ec && c <= 0x00ef)
                    {
                        result = "i";
                    }
                    else if (c == 0x00f1)
                    {
                        result = "n";
                    }
                    else if (c >= 0x00f2 && c <= 0x00f8)
                    {
                        result = "o";
                    }
                    else if (c >= 0x00f9 && c <= 0x00fc)
                    {
                        result = "u";
                    }
                    else if (c >= 0x00fd && c <= 0x00ff)
                    {
                        result = "y";
                    }
                    else if (c >= 0x2018 && c <= 0x2019)
                    {
                        result = "\'";
                    }
                    else if (c >= 0x201c && c <= 0x201e)
                    {
                        result = "\"";
                    }
                    else if (c >= 0x0213 && c <= 0x2014)
                    {
                        result = "-";
                    }
                    else if (c >= 0x00A2 && c <= 0x00A5)
                    {
                        result = "$";
                    }
                    else if (c == 0x2026)
                    {
                        result = ".";
                    }
                    b.Append(result);
                }
                else
                {
                    b.Append(c);
                }
            }
            return b.ToString();
        }


        public static string ToCSVString(string[] fields)
        {
            var b = new StringBuilder();
            foreach (string fld in fields)
            {
                if (b.Length > 0)
                {
                    b.Append(',');
                }
                string field = EscapeString(fld, new char[] {'\"'}, '\"'); // escape quotes with double quotes
                b.Append('\"').Append(field).Append('\"');
            }
            return b.ToString();
        }

        /**
   * Swap any occurrences of any characters in the from string in the input string with
   * the corresponding character from the to string.  As Perl tr, for example,
   * tr("chris", "irs", "mop").equals("chomp"), except it does not
   * support regular expression character ranges.
   * <p>
   * <i>Note:</i> This is now optimized to not allocate any objects if the
   * input is returned unchanged.
   */
        /*public static string tr(string input, string from, string to) {
    assert from.Length() == to.Length();
    StringBuilder sb = null;
    int len = input.Length();
    for (int i = 0; i < len; i++) {
      int ind = from.indexOf(input.charAt(i));
      if (ind >= 0) {
        if (sb == null) {
          sb = new StringBuilder(input);
        }
        sb.setCharAt(i, to.charAt(ind));
      }
    }
    if (sb == null) {
      return input;
    } else {
      return sb.ToString();
    }
  }*/

        /// <summary>
        /// Returns the supplied string with any trailing '\n' removed.
        /// </summary>
        public static string Chomp(string s)
        {
            if (s.Length == 0)
                return s;
            int l_1 = s.Length - 1;
            if (s[l_1] == '\n')
            {
                return s.Substring(0, l_1);
            }
            return s;
        }
        
        /// <summary>
        /// Returns the result of calling ToString() on the supplied Object, but with any trailing '\n' removed.
        /// </summary>
        public static string Chomp(Object o)
        {
            return Chomp(o.ToString());
        }
        
        /*public static string toInvocationString(string cls, string[] args) {
    StringBuilder sb = new StringBuilder();
    sb.Append(cls).Append(" invoked on ").Append(new Date());
    sb.Append(" with arguments:\n  ");
    for (string arg : args) {
      sb.Append(' ').Append(arg);
    }
    return sb.ToString();
  }*/

        /// <summary>
        /// Strip directory from filename.  Like Unix 'basename'. <p/>
        /// Example: <code>getBaseName("/u/wcmac/foo.txt") ==> "foo.txt"</code>
        /// </summary>
        public static string GetBaseName(string fileName)
        {
            return GetBaseName(fileName, "");
        }
        
        /// <summary>
        /// Strip directory and suffix from filename.  Like Unix 'basename'.
        /// Example: <code>getBaseName("/u/wcmac/foo.txt", "") ==> "foo.txt"</code>
        /// Example: <code>getBaseName("/u/wcmac/foo.txt", ".txt") ==> "foo"</code>
        /// Example: <code>getBaseName("/u/wcmac/foo.txt", ".pdf") ==> "foo.txt"</code>
        /// </summary>
        public static string GetBaseName(string fileName, string suffix)
        {
            string[] elts = fileName.Split(new[] {"/"}, StringSplitOptions.None);
            string lastElt = elts[elts.Length - 1];
            if (lastElt.EndsWith(suffix))
            {
                lastElt = lastElt.Substring(0, lastElt.Length - suffix.Length);
            }
            return lastElt;
        }
        
        /// <summary>
        /// Given a string the method uses Regex to check if the string only contains alphabet characters
        /// </summary>
        /// <param name="s">a string to check using regex</param>
        /// <returns>true if the string is valid</returns>
        public static bool IsAlpha(string s)
        {
            /*Pattern p = Pattern.compile("^[\\p{Alpha}\\s]+$");
            Matcher m = p.matcher(s);
            return m.matches();*/
            return Regex.IsMatch(s, "^[\\p{Alpha}\\s]+$");
        }

        /// <summary>
        /// Given a string the method uses Regex to check if the string only contains numeric characters
        /// </summary>
        /// <param name="s">a string to check using regex</param>
        /// <returns>true if the string is valid</returns>
        public static bool IsNumeric(string s)
        {
            /*Pattern p = Pattern.compile("^[\\p{Digit}\\s\\.]+$");
            Matcher m = p.matcher(s);
            return m.matches();*/
            return Regex.IsMatch(s, "^[\\p{Digit}\\s\\.]+$");
        }
        
        /// <summary>
        /// Given a string the method uses Regex to check 
        /// if the string only contains alphanumeric characters
        /// </summary>
        /// <param name="s">a string to check using regex</param>
        /// <returns>true if the string is valid</returns>
        public static bool IsAlphanumeric(string s)
        {
            /*Pattern p = Pattern.compile("^[\\p{Alnum}\\s\\.]+$");
            Matcher m = p.matcher(s);
            return m.matches();*/
            return Regex.IsMatch(s, "^[\\p{Alnum}\\s\\.]+$");
        }

        /// <summary>
        /// Given a string the method uses Regex to check 
        /// if the string only contains punctuation characters
        /// </summary>
        /// <param name="s">a string to check using regex</param>
        /// <returns>true if the string is valid</returns>
        public static bool IsPunct(string s)
        {
            /*Pattern p = Pattern.compile("^[\\p{Punct}]+$");
            Matcher m = p.matcher(s);
            return m.matches();*/
            return Regex.IsMatch(s, "^[\\p{Punct}]+$");
        }

        /// <summary>
        /// Given a string the method uses Regex to check if the string looks like an acronym
        /// </summary>
        /// <param name="s">a string to check using regex</param>
        /// <returns>true if the string is valid</returns>
        public static bool IsAcronym(string s)
        {
            /*Pattern p = Pattern.compile("^[\\p{Upper}]+$");
            Matcher m = p.matcher(s);
            return m.matches();*/
            return Regex.IsMatch(s, "^[\\p{Upper}]+$");
        }

        public static string GetNotNullString(string s)
        {
            if (s == null)
                return "";
            else
                return s;
        }

        /**
   * Resolve variable. If it is the props file, then substitute that variable with
   * the value mentioned in the props file, otherwise look for the variable in the environment variables.
   * If the variable is not found then substitute it for empty string.
   */
        /*public static string resolveVars(string str, Map props) {
    if (str == null)
      return null;
    // ${VAR_NAME} or $VAR_NAME
    Pattern p = Pattern.compile("\\$\\{(\\w+)\\}");
    Matcher m = p.matcher(str);
    StringBuffer sb = new StringBuffer();
    while (m.find()) {
      string varName = null == m.group(1) ? m.group(2) : m.group(1);
      string vrValue;
      //either in the props file
      if (props.containsKey(varName)) {
        vrValue = ((string) props.get(varName));
      } else {
        //or as the environment variable
        vrValue = System.getenv(varName);
      }
      m.AppendReplacement(sb, null == vrValue ? "" : vrValue);
    }
    m.AppendTail(sb);
    return sb.ToString();
  }*/


        /**
   * convert args to properties with variable names resolved. for each value
   * having a ${VAR} or $VAR, its value is first resolved using the variables
   * listed in the props file, and if not found then using the environment
   * variables. if the variable is not found then substitute it for empty string
   */
        /*public static Properties argsToPropertiesWithResolve(string[] args) {
    LinkedHashMap<string, string> result = new LinkedHashMap<string, string>();
    Map<string, string> existingArgs = new LinkedHashMap<string, string>();

    for (int i = 0; i < args.Length; i++) {
      string key = args[i];
      if (key.Length() > 0 && key.charAt(0) == '-') { // found a flag
        if (key.Length() > 1 && key.charAt(1) == '-')
          key = key.Substring(2); // strip off 2 hyphens
        else
          key = key.Substring(1); // strip off the hyphen

        int max = 1;
        int min = 0;
        List<string> flagArgs = new ArrayList<string>();
        // cdm oct 2007: add length check to allow for empty string argument!
        for (int j = 0; j < max && i + 1 < args.Length && (j < min || args[i + 1].Length() == 0 || args[i + 1].charAt(0) != '-'); i++, j++) {
          flagArgs.Add(args[i + 1]);
        }

        if (flagArgs.isEmpty()) {
          existingArgs.put(key, "true");
        } else {

          if (key.equalsIgnoreCase(PROP) || key.equalsIgnoreCase(PROPS) || key.equalsIgnoreCase(PROPERTIES) || key.equalsIgnoreCase(ARGUMENTS) || key.equalsIgnoreCase(ARGS)) {
            for(string flagArg: flagArgs)
              result.putAll(propFileToLinkedHashMap(flagArg, existingArgs));

            existingArgs.clear();
          } else
            existingArgs.put(key, join(flagArgs, " "));
        }
      }
    }
    result.putAll(existingArgs);

    for (Entry<string, string> o : result.entrySet()) {
      string val = resolveVars(o.getValue(), result);
      result.put(o.getKey(), val);
    }
    Properties props = new Properties();
    props.putAll(result);
    return props;
  }*/

        /**
   * This method reads in properties listed in a file in the format prop=value,
   * one property per line. and reads them into a LinkedHashMap (insertion order preserving)
   * Flags not having any arguments is set to "true".
   *
   * @param filename A properties file to read
   * @return The corresponding LinkedHashMap where the ordering is the same as in the
   *         props file
   */
        /*public static LinkedHashMap<string, string> propFileToLinkedHashMap(string filename, Map<string, string> existingArgs) {

    LinkedHashMap<string, string> result = new LinkedHashMap<string, string>();
    result.putAll(existingArgs);
    for (string l : IOUtils.readLines(filename)) {
      l = l.Trim();
      if (l.isEmpty() || l.startsWith("#"))
        continue;
      int index = l.indexOf('=');

      if (index == -1)
        result.put(l, "true");
      else
        result.put(l.Substring(0, index).Trim(), l.Substring(index + 1).Trim());
    }
    return result;
  }*/

        /**
   * n grams for already splitted string. the ngrams are joined with a single space
   */
        /*public static List<string> getNgrams(List<string> words, int minSize, int maxSize){
    List<List<string>> ng = CollectionUtils.getNGrams(words, minSize, maxSize);
    List<string> ngrams = new ArrayList<string>();
    for(List<string> n: ng)
      ngrams.Add(StringUtils.join(n," "));

    return ngrams;
  }*/

        /**
   * n grams for already splitted string. the ngrams are joined with a single space
   */
        /*public static List<string> getNgramsFromTokens(List<CoreLabel> words, int minSize, int maxSize){
    List<string> wordsStr = new ArrayList<string>();
    for(CoreLabel l : words)
      wordsStr.Add(l.word());
    List<List<string>> ng = CollectionUtils.getNGrams(wordsStr, minSize, maxSize);
    List<string> ngrams = new ArrayList<string>();
    for(List<string> n: ng)
      ngrams.Add(StringUtils.join(n," "));

    return ngrams;
  }*/

        /**
   * The string is split on whitespace and the ngrams are joined with a single space
   */
        /*public static List<string> getNgramsString(string s, int minSize, int maxSize){
    return getNgrams(Arrays.asList(s.Split("\\s+")), minSize, maxSize);
  }*/

        /// <summary>
        /// Build a list of character-based ngrams from the given string.
        /// </summary>
        public static List<string> GetCharacterNgrams(string s, int minSize, int maxSize)
        {
            var ngrams = new List<string>();
            int len = s.Length;

            for (int i = 0; i < len; i++)
            {
                for (int ngramSize = minSize;
                    ngramSize > 0 && ngramSize <= maxSize && i + ngramSize <= len;
                    ngramSize++)
                {
                    ngrams.Add(s.Substring(i, i + ngramSize));
                }
            }

            return ngrams;
        }

        //private static Regex diacriticalMarksPattern = new Regex("\\p{InCombiningDiacriticalMarks}", RegexOptions.Compiled);
        /*public static string normalize(string s) {
    // Normalizes string and strips diacritics (map to ascii) by
    // 1. taking the NFKD (compatibility decomposition -
    //   in compatibility equivalence, formatting such as subscripting is lost -
    //   see http://unicode.org/reports/tr15/)
    // 2. Removing diacriticals
    // 3. Recombining into NFKC form (compatibility composition)
    // This process may be slow.
    //
    // The main purpose of the function is to remove diacritics for asciis,
    //  but it may normalize other stuff as well.
    // A more conservative approach is to do explicit folding just for ascii character
    //   (see RuleBasedNameMatcher.normalize)
    string d = Normalizer.normalize(s, Normalizer.Form.NFKD);
    d = diacriticalMarksPattern.matcher(d).replaceAll("");
    return Normalizer.normalize(d, Normalizer.Form.NFKC);
  }*/
    }
}