using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public enum StageDisplayType
	{
		None,
		Show,
		Hide,
		Swap,
		MoveToFront,
		UndimAllPortraits,
		DimNonSpeakingPortraits
	}

	[CommandInfo("Narrative", 
	             "Control Stage",
	             "Controls the stage on which character portraits are displayed.")]
	public class ControlStage : Command 
	{	
		[Tooltip("Display type")]
		public StageDisplayType display;

		[Tooltip("Stage to display characters on")]
		public Stage stage;

		[Tooltip("Stage to swap with")]
		public Stage replacedStage;

		[Tooltip("Use Default Settings")]
		public bool useDefaultSettings = true;

		[Tooltip("Fade Duration")]
		public float fadeDuration;
		
		[Tooltip("Wait until the tween has finished before executing the next command")]
		public bool waitUntilFinished = false;
		
		public override void OnEnter()
		{
			// If no display specified, do nothing
			if (display == StageDisplayType.None)
			{
				Continue();
				return;
			}
			// Selected "use default Portrait Stage"
			if (stage == null)            // Default portrait stage selected
			{
				if (stage == null)        // If no default specified, try to get any portrait stage in the scene
				{
					stage = GameObject.FindObjectOfType<Stage>();
				}
			}
			// If portrait stage does not exist, do nothing
			if (stage == null)
			{
				Continue();
				return;
			}
			// Selected "use default Portrait Stage"
			if (display == StageDisplayType.Swap)            // Default portrait stage selected
			{
				if (replacedStage == null)        // If no default specified, try to get any portrait stage in the scene
				{
					replacedStage = GameObject.FindObjectOfType<Stage>();
				}
				// If portrait stage does not exist, do nothing
				if (replacedStage == null)
				{
					Continue();
					return;
				}
			}
			// Use default settings
			if (useDefaultSettings)
			{
				fadeDuration = stage.fadeDuration;
			}
			switch(display)
			{
			case (StageDisplayType.Show):
				Show(stage);
				break;
			case (StageDisplayType.Hide):
				Hide(stage);
				break;
			case (StageDisplayType.Swap):
				Show(stage);
				Hide(replacedStage);
				break;
			case (StageDisplayType.MoveToFront):
				MoveToFront(stage);
				break;
			case (StageDisplayType.UndimAllPortraits):
				UndimAllPortraits(stage);
				break;
			case (StageDisplayType.DimNonSpeakingPortraits):
				DimNonSpeakingPortraits(stage);
				break;
			}

			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		protected void Show(Stage stage) 
		{
			if (fadeDuration == 0)
			{
				fadeDuration = float.Epsilon;
			}

			LeanTween.value(gameObject,0,1,fadeDuration).setOnUpdate(
				(float fadeAmount)=>{
				foreach ( Character c in stage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",fadeAmount);
				}
			}
			).setOnComplete(
				()=>{
				foreach ( Character c in stage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",1);
				}
				OnComplete();
			}
			);
		}

		protected void Hide(Stage stage)
		{
			if (fadeDuration == 0) 
			{
				fadeDuration = float.Epsilon;
			}

			LeanTween.value(gameObject,1,0,fadeDuration).setOnUpdate(
				(float fadeAmount)=>{
				foreach ( Character c in stage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",fadeAmount);
				}
			}
			).setOnComplete(
				()=>{
				foreach ( Character c in stage.charactersOnStage)
				{
					c.state.portraitImage.material.SetFloat("_Alpha",0);
				}
				OnComplete();
			}
			);
		}

		protected void MoveToFront(Stage stage)
		{
			foreach (Stage s in Stage.activeStages)
			{
				if (s == stage)
				{
					s.portraitCanvas.sortingOrder = 1;
				}
				else
				{
					s.portraitCanvas.sortingOrder = 0;
				}
			}
		}

		protected void UndimAllPortraits(Stage stage) 
		{
			stage.dimPortraits = false;
			foreach (Character character in stage.charactersOnStage)
			{
				Portrait.Undim(character, stage);
			}
		}

		protected void DimNonSpeakingPortraits(Stage stage) 
		{
			stage.dimPortraits = true;
		}

		protected void OnComplete() 
		{
			if (waitUntilFinished)
			{
				Continue();
			}
		}

		public override string GetSummary()
		{
			string displaySummary = "";
			if (display != StageDisplayType.None)
			{
				displaySummary = StringFormatter.SplitCamelCase(display.ToString());
			}
			else
			{
				return "Error: No display selected";
			}
			string stageSummary = "";
			if (stage != null)
			{
				stageSummary = " \"" + stage.name + "\"";
			}
			return displaySummary + stageSummary;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(230, 200, 250, 255);
		}

		public override void OnCommandAdded(Block parentBlock)
		{
			//Default to display type: show
			display = StageDisplayType.Show;
		}
	}
}