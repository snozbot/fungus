using UnityEngine;
using System.Collections;
using Fungus;

public class MenusRoom : Room
{
	public Room pagesRoom;
	public Room viewsRoom;
	public Room spritesRoom;
	public Room audioRoom;

	void OnEnter() 
	{
		AddOption("1. Writing a story with Pages", MoveToWriting);
		AddOption("2. Controlling the camera with Views", MoveToViews);
		AddOption("3. Using Sprites and Animations", MoveToAnimations);
		AddOption("4. Playing music and sound effects", MoveToAudio);
		Choose("Choose an example");
	}

	void MoveToWriting()
	{
		MoveToRoom(pagesRoom);
	}

	void MoveToViews()
	{
		MoveToRoom(viewsRoom);
	}

	void MoveToAnimations()
	{
		MoveToRoom(spritesRoom);
	}

	void MoveToAudio()
	{
		MoveToRoom(audioRoom);
	}
}
