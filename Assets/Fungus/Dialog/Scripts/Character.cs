using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Character : MonoBehaviour 
	{
		public string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")
		public Color nameColor = Color.white;
		public Sprite profileSprite;
		public AudioClip soundEffect;

		[TextArea(5,10)]
		public string notes;

		static public List<Character> activeCharacters = new List<Character>();

		protected virtual void OnEnable()
		{
			if (!activeCharacters.Contains(this))
			{
				activeCharacters.Add(this);
			}
		}
		
		protected virtual void OnDisable()
		{
			activeCharacters.Remove(this);
		}
	}

}