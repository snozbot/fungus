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
		public StringData url = new StringData();

		public override void OnEnter()
		{
			Application.OpenURL(url.Value);

			Continue();
		}

		public override string GetSummary()
		{
			return url.Value;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}