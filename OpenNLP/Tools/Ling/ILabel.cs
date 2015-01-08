using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// Something that implements the <code>Label</code> interface can act as a
    /// constituent, node, or word label with linguistic attributes.
    /// A <code>Label</code> is required to have a "primary" <code>string</code>
    /// <code>value()</code> (although this may be null).  This is referred to as
    /// its <code>value</code>.
    /// 
    /// Implementations of Label split into two groups with
    /// respect to equality. Classes that extend ValueLabel define equality
    /// solely in terms of string equality of its value (secondary facets may be
    /// present but are ignored for purposes of equality), and have equals and
    /// compareTo defined across all subclasses of ValueLabel. This behavior
    /// should not be changed. Other classes that implement Label define equality only
    /// with their own type and require all fields of the type to be equal.
    /// 
    /// A subclass that extends another Label class <i>should</i> override
    /// the definition of <code>labelFactory()</code>, since the contract for
    /// this method is that it should return a factory for labels of the
    /// exact same object type.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ILabel
    {
        /// <summary>
        /// Return a string representation of just the "main" value of this label.
        /// </summary>
        /// <returns>the "value" of the label</returns>
        string Value();

        /// <summary>
        /// Set the value for the label (if one is stored).
        /// </summary>
        /// <param name="value">the value for the label</param>
        void SetValue(string value);

        /// <summary>
        /// Return a string representation of the label.  For a multipart label,
        /// this will return all parts.  The <code>ToString()</code> method
        /// causes a label to spill its guts.  It should always return an
        /// empty string rather than <code>null</code> if there is no value.
        /// </summary>
        /// <returns>a text representation of the full label contents</returns>
        string ToString();

        /// <summary>
        /// Set the contents of this label to this <code>string</code> representing the
        /// complete contents of the label.  A class implementing label may 
        /// throw an <code>UnsupportedOperationException</code> for this method (only).
        /// Typically, this method would do some appropriate decoding 
        /// of the string in a way that sets multiple fields 
        /// in an inverse of the <code>ToString()</code> method.
        /// </summary>
        /// <param name="labelStr">the string that translates into the content of the label</param>
        void SetFromString(string labelStr);

        /// <summary>
        /// Returns a factory that makes labels of the exact same type as this one.
        /// May return <code>null</code> if no appropriate factory is known.
        /// </summary>
        /// <returns>the LabelFactory for this kind of label</returns>
        ILabelFactory LabelFactory();
    }
}