namespace Fungus
{
    /// <summary>
    /// Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
    /// The Game Object must have a Collider or Collider2D component attached.
    /// Use in conjunction with the ObjectClicked Flowchart event handler.
    /// </summary>
    public interface IClickable2D
    {
        /// <summary>
        /// Is object clicking enabled.
        /// </summary>
        bool ClickEnabled { set; }
    }
}