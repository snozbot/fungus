// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Supported display operations for Stage.
    /// </summary>
    public enum StageDisplayType
    {
        /// <summary> No operation </summary>
        None,
        /// <summary> Show the stage and all portraits. </summary>
        Show,
        /// <summary> Hide the stage and all portraits. </summary>
        Hide,
        /// <summary> Swap the stage and all portraits with another stage. </summary>
        Swap,
        /// <summary> Move stage to the front. </summary>
        MoveToFront,
        /// <summary> Undim all portraits on the stage. </summary>
        UndimAllPortraits,
        /// <summary> Dim all non-speaking portraits on the stage. </summary>
        DimNonSpeakingPortraits
    }

    /// <summary>
    /// Controls the stage on which character portraits are displayed.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Control Stage",
                 "Controls the stage on which character portraits are displayed.")]
    public class ControlStage : ControlWithDisplay<StageDisplayType> 
    {
        [Tooltip("Stage to display characters on")]
        [SerializeField] protected Stage stage;
        public virtual Stage _Stage { get { return stage; } }

        [Tooltip("Stage to swap with")]
        [SerializeField] protected Stage replacedStage;

        [Tooltip("Use Default Settings")]
        [SerializeField] protected bool useDefaultSettings = true;
        public virtual bool UseDefaultSettings { get { return useDefaultSettings; } }

        [Tooltip("Fade Duration")]
        [SerializeField] protected float fadeDuration;
        
        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = false;

        protected virtual void Show(Stage stage, bool visible) 
        {
            float duration = (fadeDuration == 0) ? float.Epsilon : fadeDuration;
            float targetAlpha = visible ? 1f : 0f;

            CanvasGroup canvasGroup = stage.GetComponentInChildren<CanvasGroup>();
            if (canvasGroup == null)
            {
                Continue();
                return;
            }

            LeanTween.value(canvasGroup.gameObject, canvasGroup.alpha, targetAlpha, duration).setOnUpdate( (float alpha) => {
                canvasGroup.alpha = alpha;
            }).setOnComplete( () => {
                OnComplete();
            });
        }

        protected virtual void MoveToFront(Stage stage)
        {
            var activeStages = Stage.ActiveStages;
            for (int i = 0; i < activeStages.Count; i++)
            {
                var s = activeStages[i];
                if (s == stage)
                {
                    s.PortraitCanvas.sortingOrder = 1;
                }
                else
                {
                    s.PortraitCanvas.sortingOrder = 0;
                }
            }
        }

        protected virtual void UndimAllPortraits(Stage stage) 
        {
            stage.DimPortraits = false;
            var charactersOnStage = stage.CharactersOnStage;
            for (int i = 0; i < charactersOnStage.Count; i++)
            {
                var character = charactersOnStage[i];
                stage.SetDimmed(character, false);
            }
        }

        protected virtual void DimNonSpeakingPortraits(Stage stage) 
        {
            stage.DimPortraits = true;
        }

        protected virtual void OnComplete() 
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }

        #region Public members

        public override void OnEnter()
        {
            // If no display specified, do nothing
            if (IsDisplayNone(display))
            {
                Continue();
                return;
            }

            // Selected "use default Portrait Stage"
            if (stage == null)           
            {
                // If no default specified, try to get any portrait stage in the scene
                stage = FindObjectOfType<Stage>();

                // If portrait stage does not exist, do nothing
                if (stage == null)
                {
                    Continue();
                    return;
                }
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
                fadeDuration = stage.FadeDuration;
            }
            switch(display)
            {
            case (StageDisplayType.Show):
                Show(stage, true);
                break;
            case (StageDisplayType.Hide):
                Show(stage, false);
                break;
            case (StageDisplayType.Swap):
                Show(stage, true);
                Show(replacedStage, false);
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

        #endregion
    }
}