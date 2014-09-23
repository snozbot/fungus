using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Note", 
	             "Records design notes and reminders about your script.")]
	public class Note : Command
	{	
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