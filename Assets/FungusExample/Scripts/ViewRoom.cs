using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
	public class ViewRoom : Room 
	{
		public Room menuRoom;

		public View mainView;
		public View logoView;
		public View toadstoolView;

		void OnEnter()
		{
			SetView(mainView);

			AddOption("Lets look at the logo", LookLogo);
			AddOption("That's a nice toadstool over there", LookToadstool);
			AddOption("Back to menu", MoveToMenu);

			Say("Wanna move the camera?");
		}

		void MoveToMenu()
		{
			MoveToRoom(menuRoom);
		}

		void LookLogo()
		{
			PanToView(logoView, 2f);
			Wait(2);
			PanToView(mainView, 2f, false); // Don't wait for pan to finish before executing next command
			Say("Wow - nice logo!");
			Call(OnEnter);
		}

		void LookToadstool()
		{
			FadeToView(toadstoolView, 2f);
			Say("Now that is a pretty mushroom");
			Say("Hey - let's go look at that logo");
			Call(LookLogo);
		}
	}
}