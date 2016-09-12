namespace Fungus
{
    /// <summary>
    /// Create custom tags for use in Say text.
    /// </summary>
    public interface ICustomTag
    {
        /// <summary>
        /// String that defines the start of the tag.
        /// </summary>
        string TagStartSymbol { get; }

        /// <summary>
        /// String that defines the end of the tag.
        /// </summary>
        string TagEndSymbol { get; }

        /// <summary>
        /// String to replace the start tag with.
        /// </summary>
        string ReplaceTagStartWith { get; }

        /// <summary>
        /// String to replace the end tag with.
        /// </summary>
        string ReplaceTagEndWith { get; }
    }
}
