using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.International.Morph
{
    public class MorphoFeatures
    {
        private static readonly long serialVersionUID = -3893316324305154940L;

        public static readonly String KEY_VAL_DELIM = ":";

        protected readonly Dictionary<MorphoFeatureSpecification.MorphoFeatureType, String> fSpec;
        protected String altTag;

        public MorphoFeatures()
        {
            fSpec = new Dictionary<MorphoFeatureSpecification.MorphoFeatureType, string>();
        }

        public MorphoFeatures(MorphoFeatures other) : this()
        {
            foreach (var entry in other.fSpec)
                this.fSpec.Add(entry.Key, entry.Value);
            this.altTag = other.altTag;
        }

        public void addFeature(MorphoFeatureSpecification.MorphoFeatureType feat, String val)
        {
            fSpec.Add(feat, val);
        }

        public bool hasFeature(MorphoFeatureSpecification.MorphoFeatureType feat)
        {
            return fSpec.ContainsKey(feat);
        }

        public String getValue(MorphoFeatureSpecification.MorphoFeatureType feat)
        {
            return hasFeature(feat) ? fSpec[feat] : "";
        }

        public int numFeatureMatches(MorphoFeatures other)
        {
            int nMatches = 0;
            foreach (var fPair in fSpec)
            {
                if (other.hasFeature(fPair.Key) && other.getValue(fPair.Key).Equals(fPair.Value))
                    nMatches++;
            }

            return nMatches;
        }

        public int numActiveFeatures()
        {
            return fSpec.Count;
        }

        /**
   * Build a POS tag consisting of a base category plus inflectional features.
   * 
   * @param baseTag
   * @return the tag
   */

        public String getTag(String baseTag)
        {
            return baseTag + ToString();
        }

        public void setAltTag(String tag)
        {
            altTag = tag;
        }


        /**
   * An alternate tag form than the one produced by getTag(). Subclasses
   * may want to use this form to implement someone else's tagset (e.g., CC, ERTS, etc.)
   * 
   * @return the tag
   */

        public String getAltTag()
        {
            return altTag;
        }

        /**
   * Assumes that the tag string has been formed using a call to getTag(). As such,
   * it removes the basic category from the feature string.
   * <p>
   * Note that this method returns a <b>new</b> MorphoFeatures object. As a result, it
   * behaves like a static method, but is non-static so that subclasses can override
   * this method.
   * 
   * @param str
   */

        public MorphoFeatures fromTagString(String str)
        {
            //List<String> feats = str.Split(new string[]{"\\-"}, StringSplitOptions.None).ToList();
            List<String> feats = str.Split('-').ToList();
            MorphoFeatures mFeats = new MorphoFeatures();
            foreach (String fPair in feats)
            {
                String[] keyValue = Regex.Split(fPair, KEY_VAL_DELIM);
                if (keyValue.Length != 2) //Manual state split annotations
                    continue;
                MorphoFeatureSpecification.MorphoFeatureType fName;
                var success = MorphoFeatureSpecification.MorphoFeatureType.TryParse(keyValue[0].Trim(), out fName);
                if (success)
                {
                    mFeats.addFeature(fName, keyValue[1].Trim());
                }
                else
                {
                    // TODO:add warning
                }
            }

            return mFeats;
        }

        /**
   * values() returns the values in the order in which they are declared. Thus we will not have
   * the case where two feature types can yield two strings:
   *    -feat1:A-feat2:B
   *    -feat2:B-feat1:A
   */
        //@Override
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in fSpec)
            {
                sb.Append(String.Format("-{0}{1}{2}", entry.Key.ToString(), KEY_VAL_DELIM, entry.Value));
            }
            /*foreach(MorphoFeatureSpecification.MorphoFeatureType feat in MorphoFeatureSpecification.MorphoFeatureType.values()) {
      if(fSpec.ContainsKey(feat)) {
        sb.Append(String.Format("-{0}{1}{2}",feat.ToString(),KEY_VAL_DELIM,fSpec[feat]));
      }
    }*/
            return sb.ToString();
        }
    }
}