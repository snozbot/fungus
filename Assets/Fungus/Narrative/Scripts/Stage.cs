using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Stage : MonoBehaviour 
	{
		public Canvas portraitCanvas;
		public bool dimPortraits;
		public float fadeDuration = 0.5f;
		public float moveDuration = 1f;
		public LeanTweenType fadeEaseType;
		public LeanTweenType moveEaseType;
		public Vector2 shiftOffset;
		public Image defaultPosition;
		public List<RectTransform> positions;
		public List<Character> charactersOnStage = new List<Character>();

		[HideInInspector]
		static public List<Stage> activeStages = new List<Stage>();

		protected virtual void OnEnable()
		{
			if (!activeStages.Contains(this))
			{
				activeStages.Add(this);
			}
		}

		protected virtual void OnDisable()
		{
			activeStages.Remove(this);
		}

		protected virtual void Start()
		{
			if (Application.isPlaying &&
			    portraitCanvas != null)
			{
				// Ensure the stage canvas is active
				portraitCanvas.gameObject.SetActive(true);
			}
		}
	}
}

