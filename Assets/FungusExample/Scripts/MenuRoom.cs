using UnityEngine;
using System.Collections;
using Fungus;

public class MenuRoom : Room
{
	public Room writingRoom;
	public Room viewRoom;
	public Room animationRoom;

	void OnEnter() 
	{
		AddOption("1. Writing a story with Pages", MoveToWriting);
		AddOption("2. Controlling the camera with Views", MoveToViews);
		AddOption("3. Using sprites and animations", MoveToAnimations);
		Choose("Choose an example");
	}

	void MoveToWriting()
	{
		MoveToRoom(writingRoom);
	}

	void MoveToViews()
	{
		MoveToRoom(viewRoom);
	}

	void MoveToAnimations()
	{
		MoveToRoom(animationRoom);
	}
}
