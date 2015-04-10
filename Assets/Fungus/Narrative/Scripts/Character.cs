using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Character : MonoBehaviour
	{
		public string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")
		public Color nameColor = Color.white;
		public AudioClip soundEffect;
		public Sprite profileSprite;
		public List<Sprite> portraits;
		public FacingDirection portraitsFace;	
		public PortraitState state;		

		[FormerlySerializedAs("notes")]
		[TextArea(5,10)]
		public string description;

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