using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Flowchart nodes.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Returns the display rect.
        /// </summary>
        Rect _NodeRect { get; set; }
    }
}