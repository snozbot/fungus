using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class AddOptionCommand : FungusCommand
	{
		public enum Condition
		{
			AlwaysShow,
			HideOnVisited,
			ShowOnBoolean,
			HideOnBoolean
		}
		
		public string text;
		public Sequence sequence;
		public Condition condition;
		public string booleanVariableKey;

		public override void OnEnter()
		{
			Dialog dialog = Game.GetInstance().dialog;
			if (dialog != null &&
			    sequence != null)
			{
				dialog.AddOption(text, () => {
					Stop();
					parentFungusScript.ExecuteSequence(sequence); 
				});
			}
			Continue();
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (sequence != null)
			{
				connectedSequences.Add(sequence);
			}
		}

	}
	
}