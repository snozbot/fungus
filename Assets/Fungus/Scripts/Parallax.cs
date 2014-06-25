using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Attach this component to a sprite object to apply a simple parallax scrolling effect.
	 * The horizontal and vertical parallax offset is calculated based on the distance from the camera to the position of the parent Room.
	 * The scale parallax is calculated based on the ratio of the camera size to the size of the Room. This gives a 'dolly zoom' effect.
	 */
	public class Parallax : MonoBehaviour 
	{
		/**
		 * Scale factor for calculating the parallax offset.
		 */
		public Vector2 parallaxScale = new Vector2(0.25f, 0f);

		/**
		 * Scale factor when camera is zoomed out to show the full Room.
		 * This will typically be set to 1 to show the sprite at normal size.
		 */
		public float zoomedOutScale = 1f;

		/**
		 * Scale factor when camera is fully zoomed in on Room.
		 * Setting this to a value greater than 1 will give a 'dolly zoom' effect when zooming in.
		 */
		public float zoomedInScale = 1f;

		Vector3 startPosition;
		Vector3 startScale;

		Room parentRoom;

		void Start () 
		{
			// Store the starting position and scale of the sprite object
			startPosition = transform.position;
			startScale = transform.localScale;

			// Store a reference to the parent Room object
			Transform ancestor = transform.parent;
			while (ancestor != null)
			{
				Room room = ancestor.GetComponent<Room>();
				if (room != null)
				{
					parentRoom = room;
					break;
				}
				ancestor = ancestor.transform.parent;
			}
		}

		void Update () 
		{
			if (parentRoom == null)
			{
				// Don't apply offset if the sprite is not a child of a Room
				return;
			}

			if (Game.GetInstance().activeRoom != parentRoom)
			{
				// Early out if this sprite is not in the currently active Room
				return;
			}

			// Set new position for sprite
			Vector3 a = parentRoom.transform.position;
			Vector3 b = Camera.main.transform.position;
			Vector3 offset = (a - b);
			offset.x *= parallaxScale.x;
			offset.y *= parallaxScale.y;
			transform.position = startPosition + offset;

			// Set new scale for sprite
			float roomSize = parentRoom.renderer.bounds.extents.y;
			float t = Camera.main.orthographicSize / roomSize ;
			float scale = Mathf.Lerp (zoomedInScale, zoomedOutScale, t);
			transform.localScale = startScale * scale;
		}
	}
}
