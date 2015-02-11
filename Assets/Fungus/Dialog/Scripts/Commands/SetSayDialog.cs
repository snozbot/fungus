using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Set Say Dialog", 
	             "Sets a custom say dialog to use when displaying story text")]
	[AddComponentMenu("")]
	public class SetSayDialog : Command 
	{
		public static SayDialog activeDialog;

		public SayDialog sayDialog;

		public static SayDialog GetActiveSayDialog()
		{
			if (activeDialog == null)
			{
				activeDialog = GameObject.FindObjectOfType<SayDialog>();
			}

			if (activeDialog == null)
			{
				// Auto spawn a say dialog from the prefab
				GameObject go = Resources.Load<GameObject>("FungusSayDialog");
				if (go != null)
				{
					GameObject spawnedGO = Instantiate(go) as GameObject;
					spawnedGO.name = "SayDialog";
					spawnedGO.SetActive(false);
					activeDialog = spawnedGO.GetComponent<SayDialog>();
				}
			}

			return activeDialog;
		}

		public override void OnEnter()
		{
			if (sayDialog != null)
			{
				activeDialog = sayDialog;
			}

			// Populate the static cached dialog
			GetActiveSayDialog();

			Continue();
		}

		public override string GetSummary()
		{
			if (sayDialog == null)
			{
				return "Error: No say dialog selected";
			}

			return sayDialog.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}