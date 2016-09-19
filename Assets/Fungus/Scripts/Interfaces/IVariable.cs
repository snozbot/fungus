// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Scope types for Variables.
    /// </summary>
    public enum VariableScope
    {
        Private,
        Public
    }

    /// <summary>
    /// A Fungus variable that can be used with Commands.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Visibility scope for the variable.
        /// </summary>
        VariableScope Scope { get; }

        /// <summary>
        /// String identifier for the variable.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Callback to reset the variable if the Flowchart is reset.
        /// </summary>
        void OnReset();
    }
}
