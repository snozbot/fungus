using UnityEngine;
using System.Collections;

namespace Fungus
{
	/** 
	 * Defines a camera view point.
	 * The position and rotation are specified using the game object's transform, so this class
	 * only needs to specify the ortographic view size.
	 */
	[ExecuteInEditMode]
	public class View : MonoBehaviour 
	{
		/**
		 * Orthographic size of the camera view in world units.
		 */
		[Tooltip("Orthographic size of the camera view in world units.")]
		public float viewSize = 0.5f;

		/**
		 * Aspect ratio of the primary view rectangle.
		 * e.g. a 4:3 aspect ratio = 1.333
		 */
		[Tooltip("Aspect ratio of the primary view rectangle. (e.g. 4:3 aspect ratio = 1.333)")]
		public float primaryAspectRatio = (4f / 3f);

		/**
		 * Color of the primary view rectangle.
		 */
		[Tooltip("Color of the primary view rectangle.")]
		public Color primaryColor = Color.green;

		/**
		 * Aspect ratio of the secondary view rectangle.
		 * e.g. a 2:1 aspect ratio = 2/1 = 2.0
		 */
		[Tooltip("Aspect ratio of the secondary view rectangle. (e.g. 2:1 aspect ratio = 2.0)")]
		public float secondaryAspectRatio = (2f / 1f);

		/**
		 * Color of the secondary view rectangle.
		 */
		[Tooltip("Color of the secondary view rectangle.")]
		public Color secondaryColor = Color.grey;

		void Update()
		{
			// Disable scaling to avoid complicating the orthographic size calculations
			transform.localScale = new Vector3(1,1,1);
		}
	}
}