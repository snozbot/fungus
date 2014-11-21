using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("", 
	             "Comment", 
	             "Use comments to record design notes and reminders about your game.")]
	public class Comment : Command
	{	
		[Tooltip("Text to display for this comment")]
		[TextArea(2,4)]
		public string commentText = "";

		public override void OnEnter()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return commentText;
		}

		public override Color GetButtonColor()
		{
			return new Color32(220, 220, 220, 255);
		}
	}

}