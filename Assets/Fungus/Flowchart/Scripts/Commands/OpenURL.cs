using UnityEngine;
using System.Collections;
using Fungus;

namespace Fungus
{

	[CommandInfo("Scripting",
				 "Open URL",
				 "Opens the specified URL in the browser.")]
	public class LinkToWebsite : Command
	{
		[Tooltip("URL to open in the browser")]
		public string url;

		public override void OnEnter()
		{
			Application.OpenURL(url);

			Continue();
		}

		public override string GetSummary()
		{
			return url;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}