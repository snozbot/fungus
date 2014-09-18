using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Run Script", 
	             "Start executing another Fungus Script.")]
	public class RunScript : Command
	{	
		public FungusScript targetFungusScript;

		public bool stopCurrentScript = true;
	
		public override void OnEnter()
		{
			if (targetFungusScript != null)
			{
				if (stopCurrentScript)
				{
					Stop();
				}

				targetFungusScript.Execute();

				if (!stopCurrentScript)
				{
					Continue();
				}
			}
			else
			{		
				Continue();
			}
		}

		public override string GetSummary()
		{
			if (targetFungusScript == null)
			{
				return "<Continue>";
			}

			return targetFungusScript.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}