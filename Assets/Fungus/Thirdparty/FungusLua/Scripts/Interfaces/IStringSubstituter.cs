// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Text;

namespace Fungus
{
    /// <summary>
    /// Replaces special tokens in a string with substituted values (typically variables or localisation strings).
    /// </summary>
    public interface IStringSubstituter
    {
        /// <summary>
        /// The internal StringBuilder object used to perform string substitution.
        /// This is exposed publicly to allow for optimized string manipulation in client code.
        /// </summary>
        StringBuilder _StringBuilder { get; }

        /// <summary>
        /// Returns a new string that has been processed by all substitution handlers in the scene.
        /// </summary>
        string SubstituteStrings(string input);

        /// <summary>
        /// Returns a new string that has been processed by all substitution handlers in the scene.
        /// </summary>
        bool SubstituteStrings(StringBuilder input);
    }

    /// <summary>
    /// Interface for components that support substituting strings.
    /// </summary>
    public interface ISubstitutionHandler
    {
        /// <summary>
        /// Modifies a StringBuilder so that tokens are replaced by subsituted values.
        /// It's up to clients how to implement substitution but the convention looks like:
        /// "Hi {$VarName}" => "Hi John" where VarName == "John"
        /// </summary>
        /// <returns>True if the input was modified</returns>
        bool SubstituteStrings(StringBuilder input);
    }

}