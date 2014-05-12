using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
	public class ButtonRoom : Room 
	{
		public Fungus.Room menuRoom;

		public AudioClip effectClip;

		public Button homeButton;
		public Button soundButton;
		public Button questionButton;

		void OnEnter()
		{
			// Show button, always visible (because autoHide is set to false)
			ShowButton(homeButton, OnHomeClicked);

			// Show buttons, auto hides when text is displayed (because autoHide is set to true)
			ShowButton(soundButton, OnMusicClicked);
			ShowButton(questionButton, OnQuestionClicked);
		
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

			// The music button has been configured to automatically hide when this value is set
			SetValue("PlayedSound");
		}

		void OnQuestionClicked()
		{
			// Set the Button.autoHide property to automatically hide buttons when displaying page text/options or waiting
			// The Question and Sound buttons have the Auto Hide property set, but the Home button does not.

			Say("What book was he reading?");
			Say("Sadly we will never know for sure.");
		}
	}
}
