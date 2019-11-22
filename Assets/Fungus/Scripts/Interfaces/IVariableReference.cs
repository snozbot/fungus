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