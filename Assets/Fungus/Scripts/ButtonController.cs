using UnityEngine;
using System.Collections;

namespace Fungus
{
	public class ButtonController : MonoBehaviour 
	{
		public float autoButtonAlpha;

		void Update()
		{
			// Poll if active page has no content and game is not waiting
			bool showAutoButtons = false;
			Page page = Game.GetInstance().activePage;
			if (page != null &&
			    page.mode == Page.Mode.Idle &&
			    !Game.GetInstance().waiting)
			{
				showAutoButtons = true;
			}

			float targetAlpha = 0f;
			if (showAutoButtons)
			{
				targetAlpha = 1f;
			}

			float fadeSpeed = (1f / Game.GetInstance().buttonFadeDuration);
			autoButtonAlpha = Mathf.MoveTowards(autoButtonAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

			SetAutoButtonAlpha(autoButtonAlpha);
		}

		void SetAutoButtonAlpha(float alpha)
		{
			Room room = Game.GetInstance().activeRoom;
			if (room == null)
			{
				return;
			}
			
			Button[] buttons = room.gameObject.GetComponentsInChildren<Button>();
			if (buttons == null)
			{
				return;
			}
			
			foreach (Button button in buttons)
			{
				if (button.autoDisplay)
				{
					Color color = button.spriteRenderer.color;
					color.a = alpha;
					button.spriteRenderer.color = color;
				}
			}
		}
	}
}