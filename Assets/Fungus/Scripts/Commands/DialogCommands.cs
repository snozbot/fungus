using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 *  Command classes have their own namespace to prevent them popping up in code completion.
	 */
	namespace Command
	{
		/** 
		 * Writes story text to the page.
		 * A continue icon is displayed when the text has fully appeared.
		 */
		public class Say : CommandQueue.Command
		{
			string storyText;
			
			public Say(string _storyText)
			{
				storyText = _storyText;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				IDialog sayDialog = Game.GetInstance().GetDialog();
				sayDialog.Say(storyText, onComplete);
			}
		}
		
		/** 
		 * Adds an option button to the current list of options.
		 * Use the Choose command to display added options.
		 */
		public class AddOption : CommandQueue.Command
		{
			string optionText;
			Action optionAction;
			
			public AddOption(string _optionText, Action _optionAction)
			{
				optionText = _optionText;
				optionAction = _optionAction;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				IDialog characterDialog = Game.GetInstance().GetDialog();
				characterDialog.AddOption(optionText, optionAction);
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}		
	}
}
