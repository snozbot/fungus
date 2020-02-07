// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Interface for indicating that the class holds a reference to a fungus variable, used primarily in editor.
    /// </summary>
    public interface IVariableReference : IStringLocationIdentifier
    {
        bool HasReference(Variable variable);
    }
}