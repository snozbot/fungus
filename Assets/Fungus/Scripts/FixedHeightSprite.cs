using UnityEngine;
using System.Collections;

namespace Fungus
{
	// Adjusts the scale of a sprite to fit into a fixed number of vertical world units
	// This helps to keep room sprites neatly organised in the editor.
	[ExecuteInEditMode]
	public class FixedHeightSprite : MonoBehaviour 
	{
		public float height = 2f;

		public void Update()
		{
			if (!Application.isPlaying)
			{
				SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
				if (!spriteRenderer || !spriteRenderer.sprite)
				{
					return;
				}

				transform.position = new Vector3(transform.position.x, transform.position.y, 0);
				transform.rotation = Quaternion.identity;
				
				float spriteHeight = spriteRenderer.sprite.bounds.extents.y * 2;

				float scale = height / spriteHeight;
				
				transform.localScale = new Vector3(scale, scale, 1f);
			}
		}
	}
}