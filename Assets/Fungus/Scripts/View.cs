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
		public float viewSize = 0.5f;

		// An empty Start() method is needed to display enable checkbox in editor
		void Start()
		{}
	}
}