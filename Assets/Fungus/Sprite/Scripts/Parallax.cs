using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Attach this component to a sprite object to apply a simple parallax scrolling effect.
	 * The horizontal and vertical parallax offset is calculated based on the distance from the camera to the position of the background sprite.
	 * The scale parallax is calculated based on the ratio of the camera size to the size of the background sprite. This gives a 'dolly zoom' effect.
	 * Accelerometer based parallax is also applied on devices that support it.
	 */
	public class Parallax : MonoBehaviour 
	{
		/**
		 * The background sprite which this sprite is layered on top of.
		 * The position of this sprite is used to calculate the parallax offset.
		 */
		public SpriteRenderer backgroundSprite;

		/**
		 * Scale factor for calculating the parallax offset.
		 */
		public Vector2 parallaxScale = new Vector2(0.25f, 0f);

		/**
		 * Scale factor for calculating parallax offset based on device accelerometer tilt angle.
		 * Set this to 0 to disable the accelerometer parallax effect.
		 */
		public float accelerometerScale = 0.5f;

		protected Vector3 startPosition;
		protected Vector3 acceleration;
		protected Vector3 velocity;

		protected virtual void Start () 
		{
			// Store the starting position and scale of the sprite object
			startPosition = transform.position;

			// Default to using parent sprite as background
			if (backgroundSprite == null)
			{
				backgroundSprite = gameObject.GetComponentInParent<SpriteRenderer>();
			}
		}

		protected virtual void Update () 
		{
			if (backgroundSprite == null)
			{
				return;
			}

			Vector3 translation = Vector3.zero;

			// Apply parallax translation based on camera position relative to background sprite
			{
				Vector3 a = backgroundSprite.bounds.center;
				Vector3 b = Camera.main.transform.position;
				translation = (a - b);
				translation.x *= parallaxScale.x;
				translation.y *= parallaxScale.y;
				translation.z = 0;

				// TODO: Limit parallax offset to just outside the bounds of the background sprite
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
		}
	}
}
