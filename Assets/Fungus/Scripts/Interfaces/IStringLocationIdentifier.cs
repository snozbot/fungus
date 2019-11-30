namespace Fungus
{
    /// <summary>
    /// Interface for providing a human readable path to an element, used in editor code to determine where 
    /// something exists elsewhere in the scene.
    /// </summary>
    public interface IStringLocationIdentifier
    {
        string GetLocationIdentifier();
    }
}