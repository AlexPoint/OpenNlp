using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.International.Morph
{
    /// <summary>
    /// Holds a set of morphosyntactic features for a given surface form.
    /// 
    /// @author Spence Green
    /// </summary>
    public class MorphoFeatures
    {
        public static readonly string KeyValDelim = ":";

        protected readonly Dictionary<MorphoFeatureSpecification.MorphoFeatureType, string> fSpec;
        protected string altTag;

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

        public void AddFeature(MorphoFeatureSpecification.MorphoFeatureType feat, string val)
        {
            fSpec.Add(feat, val);
        }

        public bool HasFeature(MorphoFeatureSpecification.MorphoFeatureType feat)
        {
            return fSpec.ContainsKey(feat);
        }

        public string GetValue(MorphoFeatureSpecification.MorphoFeatureType feat)
        {
            return HasFeature(feat) ? fSpec[feat] : "";
        }

        public int NumFeatureMatches(MorphoFeatures other)
        {
            int nMatches = 0;
            foreach (var fPair in fSpec)
            {
                if (other.HasFeature(fPair.Key) && other.GetValue(fPair.Key).Equals(fPair.Value))
                    nMatches++;
            }

            return nMatches;
        }

        public int NumActiveFeatures()
        {
            return fSpec.Count;
        }

        /// <summary>
        /// Build a POS tag consisting of a base category plus inflectional features.
        /// </summary>
        public string GetTag(string baseTag)
        {
            return baseTag + ToString();
        }

        public void SetAltTag(string tag)
        {
            altTag = tag;
        }

        /// <summary>
        /// An alternate tag form than the one produced by getTag().
        /// Subclasses may want to use this form to implement someone else's tagset (e.g., CC, ERTS, etc.)
        /// </summary>
        public string GetAltTag()
        {
            return altTag;
        }

        /// <summary>
        /// Assumes that the tag string has been formed using a call to getTag().
        /// As such, it removes the basic category from the feature string.
        /// 
        /// Note that this method returns a <b>new</b> MorphoFeatures object.
        /// As a result, it behaves like a static method, but is non-static so that subclasses can override
        /// this method.
        /// </summary>
        public MorphoFeatures FromTagString(string str)
        {
            //List<string> feats = str.Split(new string[]{"\\-"}, StringSplitOptions.None).ToList();
            List<string> feats = str.Split('-').ToList();
            var mFeats = new MorphoFeatures();
            foreach (string fPair in feats)
            {
                string[] keyValue = Regex.Split(fPair, KeyValDelim);
                if (keyValue.Length != 2) //Manual state split annotations
                    continue;
                MorphoFeatureSpecification.MorphoFeatureType fName;
                var success = MorphoFeatureSpecification.MorphoFeatureType.TryParse(keyValue[0].Trim(), out fName);
                if (success)
                {
                    mFeats.AddFeature(fName, keyValue[1].Trim());
                }
                else
                {
                    // TODO:add warning
                }
            }

            return mFeats;
        }

        /// <summary>
        /// values() returns the values in the order in which they are declared.
        /// Thus we will not have the case where two feature types can yield two strings:
        /// -feat1:A-feat2:B
        /// -feat2:B-feat1:A
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var entry in fSpec)
            {
                sb.Append(String.Format("-{0}{1}{2}", entry.Key.ToString(), KeyValDelim, entry.Value));
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