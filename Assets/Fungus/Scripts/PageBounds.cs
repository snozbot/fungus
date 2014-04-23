using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Defines a screen aligned rectangular area for setting Page bounds
	 */
	public class PageBounds : MonoBehaviour 
	{
		/// Rectangular bounds used to display page text.
		public Bounds bounds = new Bounds(Vector3.zero, new Vector3(0.25f, 0.25f, 0f));

		/// Layout style to use for Page
		public Page.Layout layout = Page.Layout.FullSize;

		/**
		 * Modifies the active Page to use a rect defined by the bounds and the current camera transform
		 */
		public void UpdatePageRect()
		{
			// Y increases down the screen in GUI space, so top left is rect origin
			Vector3 topLeft = transform.position + bounds.center;
			topLeft.x -= bounds.extents.x;
			topLeft.y += bounds.extents.y;
			
			Vector3 bottomRight = transform.position + bounds.center;
			bottomRight.x += bounds.extents.x;
			bottomRight.y -= bounds.extents.y;
			
			Vector2 tl = Camera.main.WorldToScreenPoint(topLeft);
			Vector2 br = Camera.main.WorldToScreenPoint(bottomRight);

			Page.ScreenRect screenRect = new Page.ScreenRect();
			screenRect.x1 = (tl.x / Screen.width);
			screenRect.y1 = 1f - (tl.y / Screen.height);
			screenRect.x2 = (br.x / Screen.width);
			screenRect.y2 = 1f - (br.y / Screen.height);

			Page page = Game.GetInstance().activePage;
			page.pageRect = Page.CalcPageRect(screenRect);
			page.layout = layout;
		}
	}
}