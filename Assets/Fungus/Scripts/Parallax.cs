using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Attach this component to a sprite object to apply a simple parallax scrolling effect.
	 * The horizontal and vertical parallax offset is calculated based on the distance from the camera to the position of the parent Room.
	 * The scale parallax is calculated based on the ratio of the camera size to the size of the Room. This gives a 'dolly zoom' effect.
	 * Accelerometer based parallax may also be applied on devices that support it.
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

		/**
		 * Scale factor for calculating parallax offset based on device accelerometer tilt angle.
		 * Set this to 0 to disable the accelerometer parallax effect.
		 */
		public float accelerometerScale = 0.5f;

		Vector3 startPosition;
		Vector3 startScale;

		Vector3 acceleration;
		Vector3 velocity;

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

			Vector3 translation = Vector3.zero;

			// Apply parallax translation based on camera position relative to Room
			{
				Vector3 a = parentRoom.transform.position;
				Vector3 b = Camera.main.transform.position;
				translation = (a - b);
				translation.x *= parallaxScale.x;
				translation.y *= parallaxScale.y;
			}

			// Apply parallax translation based on device accelerometer
			if (SystemInfo.supportsAccelerometer)
			{
				float maxParallaxScale = Mathf.Max(parallaxScale.x, parallaxScale.y); 
				// The accelerometer data is quite noisy, so we apply smoothing to even it out.
				acceleration = Vector3.SmoothDamp(acceleration, Input.acceleration, ref velocity, 0.1f);
				// Assuming a 45 degree "neutral position" when holding a mobile device
				Vector3 accelerometerOffset = Quaternion.Euler(45, 0, 0) * acceleration * maxParallaxScale * accelerometerScale;
				translation += accelerometerOffset;
			}

			transform.position = startPosition + translation;

			// Set new scale for sprite
			float roomSize = parentRoom.renderer.bounds.extents.y;
			float t = Camera.main.orthographicSize / roomSize ;
			float scale = Mathf.Lerp (zoomedInScale, zoomedOutScale, t);
			transform.localScale = startScale * scale;
		}
	}
}
