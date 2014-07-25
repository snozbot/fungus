using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class SayCommand : FungusCommand 
	{
		public string text;

		public List<Sequence> options = new List<Sequence>();

		public override void OnExecute()
		{
			Dialog dialog = Game.GetInstance().dialog;

			foreach (Sequence sequence in options)
			{
				Sequence s = sequence;
			
				dialog.AddOption(sequence.titleText, () => { 
					ExecuteSequence(s); 
				});
			}

			dialog.Say (text, delegate {
				ExecuteNextCommand();
			});
		}

		public void OnDrawGizmos()
		{
			errorMessage = "";
			int i = 0;
			foreach (Sequence sequence in options)
			{
				if (sequence == null)
				{
					errorMessage = "Please select a Sequence for option " + i;
					break;
				}
			}
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			foreach (Sequence sequence in options)
			{
				if (sequence != null)
				{
					connectedSequences.Add(sequence);
				}
			}
		}
	}

}