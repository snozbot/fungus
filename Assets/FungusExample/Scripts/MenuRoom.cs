using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
	public class MenuRoom : Room
	{
		public Room pageRoom;
		public Room viewRoom;
		public Room spriteRoom;
		public Room parallaxRoom;
		public Room buttonRoom;
		public Room audioRoom;

		void OnEnter() 
		{
			SetPageMiddle();

			AddOption("Writing a story with Pages", MoveToWritingRoom);
			AddOption("Controlling the camera with Views", MoveToViewRoom);
			AddOption("Sprites and Animations", MoveToSpriteRoom);
			AddOption("Swipe panning and parallax", MoveToParallaxRoom);
			AddOption("Using Buttons", MoveToButtonsRoom);
			AddOption("Playing music and sound effects", MoveToAudioRoom);
			Choose("Choose an example");
		}

		void MoveToWritingRoom()
		{
			MoveToRoom(pageRoom);
		}

		void MoveToViewRoom()
		{
			MoveToRoom(viewRoom);
		}

		void MoveToSpriteRoom()
		{
			MoveToRoom(spriteRoom);
		}

		void MoveToParallaxRoom()
		{
			MoveToRoom(parallaxRoom);
		}

		void MoveToButtonsRoom()
		{
			MoveToRoom(buttonRoom);
		}

		void MoveToAudioRoom()
		{
			MoveToRoom(audioRoom);
		}
	}
}
