using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
	public class DialogRoom : Room 
	{
		// This is a reference to the menu room so we can transition back to the menu using MoveToRoom()
		public Room menuRoom;

		// The OnEnter() method is called whenever the player enters the room
		void OnEnter()
		{
			// Each Say() command writes one line of text, followed by a continue button
			Say("One day in the deep dark forest, a mushroom grew.");

			SetCharacter("Mushroom");

			Say("What am I doing here?");
			Say("I think I will wait for a while and see if something happens.");

			// Wait for a few seconds
			Wait(2);

			Say("Hmmm. Nothing seems to be happening.");

			// Add a some user options, you can add as many as you like.
			// The first parameter is the option text
			// The second parameter is the method to call if the user selects the option
			AddOption("Go to sleep", GoToSleep);
			AddOption("Produce spores", ProduceSpores);

			// Write some story text. 
			// The previously added options will be displayed as buttons.
			Say("Whatever will I do?");
		}

		void GoToSleep()
		{
			// Check to see if a game value has been set
			if (HasValue("spawned"))
			{
				Say("I am feeling rather sleepy after all that spawning!");
				Say("Yawn! Good night world!");

				// Leave the current room and enter the menu room
				MoveToRoom(menuRoom);
			}
			else
			{
				Say("I'm not feeling tired. I'm a fresh mushroom!");
				Say("Maybe I should spawn some spores?");

				// Use Call() to call another method whenever you want.
				Call(ProduceSpores);
			}
		}

		void ProduceSpores()
		{
			Say("Yeah! I feel like doing some sporing!");
			Say("Wow - look at all these spores! COOL!");

			// Sets a global value flag which we check above in GoToSleep
			SetValue("spawned");

			AddOption("So tired. I sleep now.", GoToSleep);
			AddOption("No way! More spores!", ProduceSpores);

			Say("What will I do now?");
		}
	}
}
