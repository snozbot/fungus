namespace Fungus
{
    /// <summary>
    /// Interface to allow the VariableEditor to check that a given fungus variable is compatibile with the object it resides with 
    /// within the object, such as in the collection base command
    /// </summary>
    public interface ICollectionCompatible
    {
        bool IsCompat(Fungus.Variable variableInQuestion, string compatibleWith);
    }
}