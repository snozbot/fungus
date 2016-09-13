using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Detects drag and drop interactions on a Game Object, and sends events to all Flowchart event handlers in the scene.
    /// The Game Object must have Collider2D & RigidBody components attached. 
    /// The Collider2D must have the Is Trigger property set to true.
    /// The RigidBody would typically have the Is Kinematic property set to true, unless you want the object to move around using physics.
    /// Use in conjunction with the Drag Started, Drag Completed, Drag Cancelled, Drag Entered & Drag Exited event handlers.
    /// </summary>
    public interface IDraggable2D
    {
        /// <summary>
        /// Is object drag and drop enabled.
        /// </summary>
        /// <value><c>true</c> if drag enabled; otherwise, <c>false</c>.</value>
        bool DragEnabled { get; set; }
    }
}
