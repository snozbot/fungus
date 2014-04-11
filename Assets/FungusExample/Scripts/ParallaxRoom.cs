using UnityEngine;
using System.Collections;
using Fungus;

// The parallax effect is achieved by attaching a Parallax script to each sprite that requires a
// parallax offset. The offset is then applied automatically whenever the camera moves around the active Room.
// There is a handy parallax sprite prefab in Fungus/Prefabs/ParallaxSprite.prefab

public class ParallaxRoom : Room 
{
	public View mainView;
	public View zoomView;

	public Room menuRoom;

	void OnEnter()
	{
		SetView(mainView);

		Say("Let's zoom in!");
		PanToView(zoomView, 2);
		Say("Oooh! Nice parallax!");
		PanToView(mainView, 2);
		Say("Mmmm... purdy!");

		MoveToRoom(menuRoom);
	}
}
