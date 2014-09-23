using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Fungus
{

	public class ClickableSprite : MonoBehaviour 
	{
		public UnityEvent onSpriteClick;

		void OnMouseDown()
		{
			onSpriteClick.Invoke();
		}
	}

}