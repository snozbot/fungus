using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Set Menu Dialog", 
	             "Sets a custom menu dialog to use when displaying multiple choice menus")]
	[AddComponentMenu("")]
	public class SetMenuDialog : Command 
	{
		public static MenuDialog activeMenuDialog;

		public MenuDialog menuDialog;

		public static MenuDialog GetActiveMenuDialog()
		{
			if (activeMenuDialog == null)
			{
				activeMenuDialog = GameObject.FindObjectOfType<MenuDialog>();
			}

			if (activeMenuDialog == null)
			{
				// Auto spawn a menu object from the prefab
				GameObject go = Resources.Load<GameObject>("FungusMenuDialog");
				if (go != null)
				{
					GameObject spawnedGO = Instantiate(go) as GameObject;
					spawnedGO.name = "MenuDialog";
					spawnedGO.SetActive(false);
					activeMenuDialog = spawnedGO.GetComponent<MenuDialog>();
				}
			}

			return activeMenuDialog;
		}

		public override void OnEnter()
		{
			if (menuDialog != null)
			{
				activeMenuDialog = menuDialog;
			}

			// Populate the static cached dialog
			GetActiveMenuDialog();

			Continue();
		}

		public override string GetSummary()
		{
			if (menuDialog == null)
			{
				return "Error: No menu dialog selected";
			}

			return menuDialog.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(170, 204, 169, 255);
		}
	}

}