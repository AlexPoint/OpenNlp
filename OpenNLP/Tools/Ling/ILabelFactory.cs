using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// A <code>LabelFactory</code> object acts as a factory for creating
    /// objects of class <code>Label</code>, or some descendant class.
    /// It can also make Labels from Strings, optionally with options.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ILabelFactory
    {
        
        /// <summary>
        /// Make a new label with this <code>string</code> as the <code>value</code>.
        /// Any other fields of the label would normally be <code>null</code>.
        /// </summary>
        /// <param name="labelStr">The string that will be used for value</param>
        /// <returns>The new Label</returns>
        ILabel NewLabel(string labelStr);

        /// <summary>
        /// Make a new label with this <code>string</code> as the value, and
        /// the type determined in an implementation-dependent way from the options value.
        /// </summary>
        /// <param name="labelStr">The string that will be used for value</param>
        /// <param name="options">May determine what kind of label is created</param>
        /// <returns>The new Label</returns>
        ILabel NewLabel(string labelStr, int options);

        /// <summary>
        /// Make a new label.  The string argument will be decomposed into
        /// multiple fields in an implementing class-specific way, in
        /// accordance with the class's setFromString() method.
        /// </summary>
        /// <param name="encodedLabelStr">
        /// The string that will be used for labelling the object (by decoding it into parts)
        /// </param>
        /// <returns>The new Label</returns>
        ILabel NewLabelFromString(string encodedLabelStr);

        /// <summary>
        /// Create a new <code>Label</code>, where the label is formed from
        /// the <code>Label</code> object passed in.
        /// The new Label is guaranteed to at least copy the <code>value()</code> of the
        /// source label (if non-null); it may also copy other components
        /// (this is implementation-specific).  However, if oldLabel is of
        /// the same type as is produced by the factory, then the whole
        /// label should be cloned, so that the returnedLabel.equals(oldLabel).
        /// <i>Implementation note:</i> That last sentence isn't true of all
        /// current implementations (e.g., WordTag), but we should make it so that it is true!
        /// </summary>
        /// <param name="oldLabel">The Label that the new label is being created from</param>
        /// <returns>The new label of a particular type</returns>
        ILabel NewLabel(ILabel oldLabel);
    }
}