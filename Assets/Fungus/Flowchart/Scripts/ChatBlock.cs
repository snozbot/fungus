using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fungus
{
	public class ChatBlock : MonoBehaviour
	{
		public Image avatar;

		public Text characterName;

		public GameObject chatText;
		
		public void setCharacter(Character character)
		{
			characterName.text = character.nameText;
			avatar.sprite = character.GetPortrait("avatar");
		}

		public double getHeight()
		{
			return 20.0;
		}
	}
}
