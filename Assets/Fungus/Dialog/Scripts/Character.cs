using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[ExecuteInEditMode]
	public class Character : MonoBehaviour 
	{
		public string characterName; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")
		public Sprite characterImage;
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