using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    /** A class that takes care of the stuff necessary for variable strings.
 *
 *  @author Roger Levy (rog@nlp.stanford.edu)
 */

    public class VariableStrings
    {
        private readonly Dictionary<String, String> varsToStrings;
        //private readonly IntCounter<String> numVarsSet;
        private readonly Dictionary<String, int> numVarsSet;

        public VariableStrings()
        {
            varsToStrings = new Dictionary<string, string>();
            //numVarsSet = new IntCounter<String>(MapFactory.<String, MutableInteger>arrayMapFactory());
            numVarsSet = new Dictionary<string, int>();
        }

        public void reset()
        {
            numVarsSet.Clear();
            varsToStrings.Clear();
        }

        public bool isSet(String o)
        {
            return numVarsSet.ContainsKey(o) && numVarsSet[o] >= 1;
            //return numVarsSet.getCount(o) >= 1;
        }

        public void setVar(String var, String string1)
        {
            String oldString = null;
            //var success = varsToStrings.TryGetValue(var,string1);
            if (varsToStrings.ContainsKey(var))
            {
                oldString = varsToStrings[var];
                varsToStrings[var] = string1;
            }
            else
            {
                varsToStrings.Add(var, string1);
            }
            if (oldString != null && ! oldString.Equals(string1))
                throw new SystemException("Error -- can't setVar to a different string -- old: " + oldString + " new: " +
                                          string1);
            //numVarsSet.incrementCount(var);
            if (numVarsSet.ContainsKey(var))
            {
                numVarsSet[var] = numVarsSet[var] + 1;
            }
            else
            {
                numVarsSet[var] = 1;
            }
        }

        public void unsetVar(String var)
        {
            /*if(numVarsSet.getCount(var) > 0)
      numVarsSet.decrementCount(var);
    if(numVarsSet.getCount(var)==0)
      varsToStrings.Add(var,null);*/
            if (numVarsSet.ContainsKey(var) && numVarsSet[var] > 0)
            {
                numVarsSet[var] = numVarsSet[var] - 1;
            }
            if (numVarsSet.ContainsKey(var) && numVarsSet[var] == 0)
            {
                varsToStrings.Add(var, null);
            }
        }

        public String getString(String var)
        {
            return varsToStrings[var];
        }

        //@Override
        public override String ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("{");
            bool appended = false;
            foreach (String key in varsToStrings.Keys)
            {
                if (appended)
                {
                    s.Append(",");
                }
                else
                {
                    appended = true;
                }
                s.Append(key);
                s.Append("=(");
                s.Append(varsToStrings[key]);
                s.Append(":");
                s.Append(numVarsSet[key]);
                s.Append(")");
            }
            s.Append("}");
            return s.ToString();
        }
    }
}