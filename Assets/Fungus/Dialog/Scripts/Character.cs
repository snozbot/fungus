using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[ExecuteInEditMode]
	public class Character : MonoBehaviour 
	{
		public string characterName;
		public Sprite characterImage;
		public SayDialog.DialogSide dialogSide;
		public Color characterColor;

		static public List<Character> activeCharacters = new List<Character>();

		void OnEnable()
		{
			if (!activeCharacters.Contains(this))
			{
				activeCharacters.Add(this);
			}
		}
		
		void OnDisable()
		{
			activeCharacters.Remove(this);
		}
	}

}