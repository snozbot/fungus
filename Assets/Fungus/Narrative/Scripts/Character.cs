using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Character : MonoBehaviour, ILocalizable
	{
		public string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")
		public Color nameColor = Color.white;
		public AudioClip soundEffect;
		public Sprite profileSprite;
		public List<Sprite> portraits;
		public FacingDirection portraitsFace;
		public PortraitState state;

		[Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
		public SayDialog setSayDialog;

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

		//
		// ILocalizable implementation
		//

		public virtual string GetStandardText()
		{
			return nameText;
		}

		public virtual void SetStandardText(string standardText)
		{
			nameText = standardText;
		}

		public virtual string GetDescription()
		{
			return description;
		}

		public virtual string GetStringId()
		{
			// String id for character names is CHARACTER.<Character Name>
			return "CHARACTER." + nameText;
		}
	}

}