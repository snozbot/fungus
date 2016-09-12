namespace Fungus
{
    /// <summary>
    /// Input handler for say dialogues.
    /// </summary>
    public interface IDialogInput
    {
        /// <summary>
        /// Trigger next line input event from script.
        /// </summary>
        void SetNextLineFlag();

        /// <summary>
        /// Set the dialog clicked flag (usually from an Event Trigger component in the dialog UI).
        /// </summary>
        void SetDialogClickedFlag();

        /// <summary>
        /// Sets the button clicked flag.
        /// </summary>
        void SetButtonClickedFlag();
    }

    /// <summary>
    /// Interface for listening for dialogue input events.
    /// </summary>
    public interface IDialogInputListener
    {
        void OnNextLineEvent();
    }
}