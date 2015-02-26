using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public class PortraitStage : MonoBehaviour 
	{
		public Canvas portraitCanvas;
		public bool dimPortraits;
		public float fadeDuration;
		public float moveSpeed;
		public LeanTweenType fadeEaseType;
		public LeanTweenType moveEaseType;
		public Vector2 shiftOffset;
		public Image defaultPosition;
		public List<RectTransform> positions;
		public List<Character> charactersOnStage = new List<Character>();

		[HideInInspector]
		static public List<PortraitStage> activePortraitStages = new List<PortraitStage>();

		[ExecuteInEditMode]
		protected virtual void OnEnable()
		{
			if (!activePortraitStages.Contains(this))
			{
				activePortraitStages.Add(this);
			}
		}
		[ExecuteInEditMode]
		protected virtual void OnDisable()
		{
			activePortraitStages.Remove(this);
		}
	}
}

