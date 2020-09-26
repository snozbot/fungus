// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;


namespace Fungus
{

    public enum CharStageDisplayType
    {
        /// <summary> No operation </summary>
        None,
        /// <summary> Dim all non-speaking portraits on the stage. </summary>
        Wobble,

        /// <summary> Applies Happy animation to character. </summary>
        Shock,

        /// <summary> Applies Happy animation to character. </summary>
        Happy,

        /// <summary> Applies Panic animation expression to character. </summary>
        Panic,
        /// <summary> Applies Panic animation expression to character. </summary>
        Mad        
    }

    /// <summary>
    /// Controls the stage on which character portraits are displayed.
    /// </summary>
    [CommandInfo("Animation", 
                 "Character Expression",
                 "Applies animation to both speaking/non-speaking character on stage. The active state valid until next character sprite change OR it should stop manually, both use cases are safe.")]
             
    public class CharacterActiveState : ControlWithDisplay<CharStageDisplayType> 
    {
        [Tooltip("Applies animation to active/non-active charracters")]
        [SerializeField] protected Character character;
        [SerializeField] protected bool AllChar = false;
        [HideInInspector] protected Stage stage;
        [HideInInspector] protected bool waitUntilFinished = false;
        [SerializeField] protected bool overideStagePro = true;
        protected static Character speakingCharacter;
        protected virtual void MadSpeakingPortraits(Stage stage) 
            {
                var prevSpeakingCharacter = speakingCharacter;
                speakingCharacter = character;
                
                var charactersOnStage = stage.CharactersOnStage;
                for (int j = 0; j < charactersOnStage.Count; j++)               
                   { 
                        var c = charactersOnStage[j];
                        if (c != null && c.Equals(speakingCharacter))
                            {   
                                LeanTween.alpha(c.State.portraitImage.rectTransform, 0f, 0.1f).setDelay(0.1f);
                                LeanTween.scale(c.State.portraitImage.rectTransform, Vector3.zero, 0.2f).setEase(LeanTweenType.punch).setLoopPingPong(-1);                                    
                            }

                        if(AllChar)                                    
                            {

                                if (c != null && !c.Equals(speakingCharacter))
                                {                        
                            
                                    for (int i = 0; i < charactersOnStage.Count; i++)
                                    {
                                        LeanTween.alpha(c.State.portraitImage.rectTransform, 0f, 0.1f).setDelay(0.1f);
                                        LeanTween.scale(c.State.portraitImage.rectTransform, Vector3.zero, 0.2f).setEase(LeanTweenType.punch).setLoopPingPong(-1);      

                                    }
                                }
                                    
                            }                                       
                        if (!waitUntilFinished)
                            {
                                Continue();                                
                            }
                        if (overideStagePro)
                            {
                                stage.SetDimmed(c, false);
                            }                                     
                    }                
                    
            }
        protected virtual void WobbleSpeakingPortraits(Stage stage) 
            {
                var prevSpeakingCharacter = speakingCharacter;
                speakingCharacter = character;
                
                var charactersOnStage = stage.CharactersOnStage;
                for (int j = 0; j < charactersOnStage.Count; j++)               
                { 
                    var c = charactersOnStage[j];
                    if (c != null && c.Equals(speakingCharacter))
                        {   

                            LeanTween.scale(c.State.portraitImage.rectTransform, Vector3.zero, 1f).setEase(LeanTweenType.punch).setLoopPingPong(-1);
                                          
                        }
                    if(AllChar)                                    
                        {

                            if (c != null && !c.Equals(speakingCharacter))
                            {                        
                           
                                for (int i = 0; i < charactersOnStage.Count; i++)
                                {
                                    LeanTween.scale(c.State.portraitImage.rectTransform, Vector3.zero, 1f).setEase(LeanTweenType.punch).setLoopPingPong(-1);

                                }
                            }
                                
                        }                                    
                    if (!waitUntilFinished)
                        {
                            Continue();                                
                        }
                    if (overideStagePro)
                        {
                            stage.SetDimmed(c, false);
                        }                                           
                }
            }

        // Happy expression
        protected virtual void HappySpeakingPortraits(Stage stage) 
        {
                var prevSpeakingCharacter = speakingCharacter;
                speakingCharacter = character;
                
                var charactersOnStage = stage.CharactersOnStage;
                for (int j = 0; j < charactersOnStage.Count; j++)               
                   { 
                        var c = charactersOnStage[j];
                        if (c != null && c.Equals(speakingCharacter))
                            {   
                                LeanTween.moveY(c.State.portraitImage.rectTransform, 100f, 0.2f).setEase(LeanTweenType.easeInQuad).setDelay(0.1f);
                                LeanTween.moveY(c.State.portraitImage.rectTransform, -100f, 0.2f).setEase(LeanTweenType.easeInQuad).setLoopPingPong(-1);                             
                            }

                        if(AllChar)                                    
                            {

                                if (c != null && !c.Equals(speakingCharacter))
                                {                        
                            
                                    for (int i = 0; i < charactersOnStage.Count; i++)
                                    {
                                        LeanTween.moveY(c.State.portraitImage.rectTransform, 100f, 0.2f).setEase(LeanTweenType.easeInQuad).setDelay(0.1f);
                                        LeanTween.moveY(c.State.portraitImage.rectTransform, -100f, 0.2f).setEase(LeanTweenType.easeInQuad).setLoopPingPong(-1); 
                                    
                                    }
                                }
                                    
                            }                                        
                        if (!waitUntilFinished)
                            {
                                Continue();                                
                            }
                        if (overideStagePro)
                            {
                                stage.SetDimmed(c, false);
                            }                                         
                    }        
        }

        protected virtual void ShockSpeakingPortraits(Stage stage) 
        {
                var prevSpeakingCharacter = speakingCharacter;
                speakingCharacter = character;
                
                var charactersOnStage = stage.CharactersOnStage;
                for (int j = 0; j < charactersOnStage.Count; j++)               
                   { 
                        var c = charactersOnStage[j];
                        if (c != null && c.Equals(speakingCharacter))
                            {   
                                LeanTween.rotateAround(c.State.portraitImage.rectTransform, Vector3.forward, -12f, 0.2f);
                                LeanTween.rotateAround(c.State.portraitImage.rectTransform, Vector3.forward, 0f, 0.2f);
                                LeanTween.rotateAround(c.State.portraitImage.rectTransform, Vector3.forward, 12f, 0.2f).setLoopPingPong(-1);                      
                            }

                        if(AllChar)                                    
                            {

                                if (c != null && !c.Equals(speakingCharacter))
                                {                        
                            
                                    for (int i = 0; i < charactersOnStage.Count; i++)
                                    {
                                        LeanTween.rotateAround(c.State.portraitImage.rectTransform, Vector3.forward, -12f, 0.2f);
                                        LeanTween.rotateAround(c.State.portraitImage.rectTransform, Vector3.forward, 0f, 0.2f);
                                        LeanTween.rotateAround(c.State.portraitImage.rectTransform, Vector3.forward, 12f, 0.2f).setLoopPingPong(-1); 

                                    }
                                }
                                    
                            }                                    
                        if (!waitUntilFinished)
                            {
                                Continue();                                
                            }
                        if (overideStagePro)
                            {
                                stage.SetDimmed(c, false);
                            }           
                    }    
        }

        protected virtual void PanicSpeakingPortraits(Stage stage) 
        {
                var prevSpeakingCharacter = speakingCharacter;
                speakingCharacter = character;
                
                var charactersOnStage = stage.CharactersOnStage;
                for (int j = 0; j < charactersOnStage.Count; j++)               
                   { 
                    var c = charactersOnStage[j];
                    if (c != null && c.Equals(speakingCharacter))
                        {   
                            LeanTween.moveX(c.State.portraitImage.rectTransform, 100f, 0.2f).setEase(LeanTweenType.easeInQuad).setDelay(0.1f);
                            LeanTween.moveX(c.State.portraitImage.rectTransform, -100f, 0.2f).setEase(LeanTweenType.easeInQuad).setLoopPingPong(-1);                             
                        }

                    if(AllChar)                                    
                        {

                            if (c != null && !c.Equals(speakingCharacter))
                            {                        
                           
                                for (int i = 0; i < charactersOnStage.Count; i++)
                                {
                                    LeanTween.moveX(c.State.portraitImage.rectTransform, 100f, 0.2f).setEase(LeanTweenType.easeInQuad).setDelay(0.1f);
                                    LeanTween.moveX(c.State.portraitImage.rectTransform, -100f, 0.2f).setEase(LeanTweenType.easeInQuad).setLoopPingPong(-1);  

                                }
                            }
                                
                        }                                     
                    if (!waitUntilFinished)
                        {
                            Continue();                                
                        }
                    if (overideStagePro)
                        {
                            stage.SetDimmed(c, false);
                        }         
                    }    
        }
        protected virtual void OnComplete() 
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }    

        public virtual Character _Character { get { return character; } }
        public Character SpeakingCharacter { get { return speakingCharacter; } }

        #region Public members        
        public override void OnEnter()
        {

            if(AllChar)
            {
                var activeCharacters = Character.ActiveCharacters;
                for (int i = 0; i < activeCharacters.Count; i++)
                {
                    var c = activeCharacters[i];
                    if (c.State.portraitImage != null)
                    {
                        if (LeanTween.isTweening(c.State.portraitImage.gameObject))
                        {
                            LeanTween.cancel(c.State.portraitImage.gameObject, true);
                            Continue();
                        }
                    }
                }
            }

            // If NONE selected, means stop all tweens
            if (IsDisplayNone(display))
            {
                var activeCharacters = Character.ActiveCharacters;
                for (int i = 0; i < activeCharacters.Count; i++)
                {
                    var c = activeCharacters[i];
                    if (c.State.portraitImage != null)
                    {
                        if (LeanTween.isTweening(c.State.portraitImage.gameObject))
                        {
                            LeanTween.cancel(c.State.portraitImage.gameObject, true);
                            PortraitController.SetRectTransform(c.State.portraitImage.rectTransform, c.State.position);                            
                            Continue();
                        }
                    }
                }
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
            // Use default settings
            switch(display)
            {            
            case (CharStageDisplayType.Wobble):
                WobbleSpeakingPortraits(stage);
                break;
            case (CharStageDisplayType.Shock):
                ShockSpeakingPortraits(stage);
                break;
            case (CharStageDisplayType.Happy):
                HappySpeakingPortraits(stage);
                break;
            case (CharStageDisplayType.Panic):
                PanicSpeakingPortraits(stage);
                break;
            case (CharStageDisplayType.Mad):
                MadSpeakingPortraits(stage);
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
            if (character != null)
            {
                displaySummary = StringFormatter.SplitCamelCase(character.ToString());
            }
            else
            {
                return "";
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
            display = CharStageDisplayType.Happy;
        }
        
    }
    #endregion
    
}