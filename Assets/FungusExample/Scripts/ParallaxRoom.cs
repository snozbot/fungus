using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
	// The parallax effect is achieved by attaching a Parallax script to each sprite that requires a
	// parallax offset. The offset is then applied automatically whenever the camera moves around the active Room.
	// There is a handy parallax sprite prefab in Fungus/Prefabs/ParallaxSprite.prefab

	public class ParallaxRoom : Room 
	{
		public View viewA;
		public View viewB;

		public Button menuButton;

		public Room menuRoom;

		void OnEnter()
		{
			SetView(viewA);

			Say("Let's move the camera!");
			PanToView(viewB, 2);
			Say("Oooh! Nice parallax!");
			PanToView(viewA, 2);
			Say("Now you have a go!");
			Say("Swipe the screen to pan around.");

			ShowButton(menuButton, OnHomeButtonClicked);

			StartSwipePan(viewA, viewB, 0f);
		}

		void OnHomeButtonClicked()
		{
			MoveToRoom(menuRoom);
		}
	}
}