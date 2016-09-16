using UnityEngine;

namespace Fungus
{
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
