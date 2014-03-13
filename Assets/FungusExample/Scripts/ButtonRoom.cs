using UnityEngine;
using System.Collections;
using Fungus;

public class ButtonRoom : Room 
{
	public Room menuRoom;

	public AudioClip effectClip;

	public Button homeButton;
	public Button soundButton;
	public Button questionButton;
	
	void OnEnter()
	{
		// Normal button, always visible
		ShowButton(homeButton, OnHomeClicked);

		// Auto hide buttons (hidden when story/options are being displayed)
		ShowButton(soundButton, OnMusicClicked);
		ShowButton(questionButton, OnQuestionClicked);
	
		// NOTE: Add auto buttons before first Say() command to ensure they start hidden

		Say("The Mushroom read his book with great interest.");
		Say("After turning the last page, he considered his options.");

		// Uncomment this line to make the player tap the screen before showing the buttons
		// WaitForInput();

		// Once the last Say command executes the page will dissappear because there's no more content to show.
		// At that point, the game will automatically fade in all Auto Buttons in the room
	}

	void OnHomeClicked()
	{
		MoveToRoom(menuRoom);
	}

	void OnMusicClicked()
	{
		PlaySound(effectClip);

		// The music button has been set to hide if this flag is set
		SetFlag("PlayedSound");
	}

	void OnQuestionClicked()
	{
		// Set the Button.autoHide property to automatically hide buttons when displaying page text/options or waiting
		// The Question and Sound buttons have the Auto Hide property set, but the Home button does not.

		Say("What book was he reading?");
		Say("Sadly we will never know for sure.");
	}
}
