// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Use comments to record design notes and reminders about your game.
    /// </summary>
    [CommandInfo("", 
                 "Comment", 
                 "Use comments to record design notes and reminders about your game.")]
    [AddComponentMenu("")]
    public class Comment : Command
    {   
        [Tooltip("Name of Commenter")]
        [SerializeField] protected string commenterName = "";

        [Tooltip("Text to display for this comment")]
        [TextArea(2,4)]
        [SerializeField] protected string commentText = "";

        #region Public members

        public override void OnEnter()
        {
            Continue();
        }

        public override string GetSummary()
        {
            if (commenterName != "")
            {
                return commenterName + ": " + commentText;
            }
            return commentText;
        }

        public override Color GetButtonColor()
        {
            return new Color32(220, 220, 220, 255);
        }

        #endregion
    }
}