using UnityEngine;
using System.Collections;
using Fungus;

public class PageRoom : Room 
{
	// This is a reference to the menu room so we can transition back to the menu using MoveToRoom()
	public Room menuRoom;

	// References to PageStyle prefab assets
	// Use these with SetPageStyle() to change the Page rendering style
	public PageStyle defaultStyle;
	public PageStyle alternateStyle;

	// The OnEnter() method is called whenever the player enters the room
	// You can also use the OnLeave() method to handle when the player leaves the room.
	void OnEnter()
	{
		// Sets the header text on the page
		Header("The Mushroom");

		// Each Say() command writes one line of text, followed by a continue button
		SetPageTop();
		Say("One day in the forest, a mushroom grew.");

		SetPageMiddle();
		Say("What am I doing here he wondered?");

		SetPageBottom();
		Say("I think I will wait for a while and see if something happens.");

		// Wait for a few seconds
		Wait(3);

		// Set the header text to the empty string to remove the page title
		Header("");

		Say("...");
		Say("Hmmm. Nothing seems to be happening.");

		// Add a couple of user options
		// The first parameter is the option text
		// The second parameter is the method to call if the user selects the option
		// You can add as many options as you like
		AddOption("Go to sleep", GoToSleep);
		AddOption("Produce spores", ProduceSpores);

		// Display all the previously added options, with a text prompt
		Choose("Whatever will I do?");
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
		// Set a PageStyle with no background box texture
		SetPageStyle(alternateStyle);

		Say("Yeah! I feel like doing some sporing!");
		Say("Wow - look at all these spores! COOL!");

		// Set the default style with background box texture
		SetPageStyle(defaultStyle);

		// Sets a game flag which we check above in GoToSleep
		SetValue("spawned");

		AddOption("So tired. I sleep now.", GoToSleep);
		AddOption("No way! More spores!", ProduceSpores);

		Choose("What will I do now?");
	}
}
