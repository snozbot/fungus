// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// A Block may have an associated Event Handler which starts executing commands when
    /// a specific event occurs. 
    /// To create a custom Event Handler, simply subclass EventHandler and call the ExecuteBlock() method
    /// when the event occurs. 
    /// Add an EventHandlerInfo attibute and your new EventHandler class will automatically appear in the
    /// 'Execute On Event' dropdown menu when a block is selected.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// The parent Block which owns this Event Handler.
        /// </summary>
        Block ParentBlock { get; set; }

        /// <summary>
        /// The Event Handler should call this method when the event is detected to start executing the Block.
        /// </summary>
        bool ExecuteBlock();

        /// <summary>
        /// Returns custom summary text for the event handler.
        /// </summary>
        string GetSummary();
    }
}