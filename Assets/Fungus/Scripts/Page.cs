using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Defines a user editable screen aligned rect for setting Page bounds.
	 */
	public class Page : MonoBehaviour 
	{
		/// Rectangular bounds used to display page text.
		public Bounds pageBounds = new Bounds(Vector3.zero, new Vector3(0.25f, 0.25f, 0f));

		/// Layout style to use for Page
		public PageController.Layout layout = PageController.Layout.FullSize;

		/**
		 * Modifies the PageController to use a rect defined by the bounds and the current camera transform
		 */
		public void UpdatePageRect()
		{
			// Y increases down the screen in GUI space, so top left is rect origin
			Vector3 topLeft = transform.position + pageBounds.center;
			topLeft.x -= pageBounds.extents.x;
			topLeft.y += pageBounds.extents.y;
			
			Vector3 bottomRight = transform.position + pageBounds.center;
			bottomRight.x += pageBounds.extents.x;
			bottomRight.y -= pageBounds.extents.y;
			
			Vector2 tl = Camera.main.WorldToScreenPoint(topLeft);
			Vector2 br = Camera.main.WorldToScreenPoint(bottomRight);

			PageController.ScreenRect screenRect = new PageController.ScreenRect();
			screenRect.x1 = (tl.x / Screen.width);
			screenRect.y1 = 1f - (tl.y / Screen.height);
			screenRect.x2 = (br.x / Screen.width);
			screenRect.y2 = 1f - (br.y / Screen.height);

			PageController page = Game.GetInstance().pageController;
			page.pageRect = PageController.CalcPageRect(screenRect);
			page.layout = layout;
		}
	}
}