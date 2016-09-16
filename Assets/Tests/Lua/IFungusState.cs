using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Used by the Flowchart window to serialize the currently active Flowchart object
    /// so that the same Flowchart can be displayed while editing & playing.
    /// </summary>
    public interface IFungusState
    {
        /// <summary>
        /// The currently selected Flowchart.
        /// </summary>
        Flowchart SelectedFlowchart { get; set; }
    }
}