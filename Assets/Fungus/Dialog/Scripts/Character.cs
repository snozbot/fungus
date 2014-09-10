using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[ExecuteInEditMode]
	public class Character : MonoBehaviour 
	{
		public string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")
		public Color nameColor = Color.white;
		public Sprite profileSprite;

		[TextArea(5,10)]
		public string notes;

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