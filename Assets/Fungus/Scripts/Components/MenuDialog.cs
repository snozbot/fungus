// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using MoonSharp.Interpreter;

namespace Fungus
{
    public class MenuDialog : MonoBehaviour, IMenuDialog
    {
        [Tooltip("Automatically select the first interactable button when the menu is shown.")]
        [SerializeField] protected bool autoSelectFirstButton = false;

        protected Button[] cachedButtons;

        protected Slider cachedSlider;

        // Currently active Menu Dialog used to display Menu options
        public static IMenuDialog activeMenuDialog;

        public static IMenuDialog GetMenuDialog()
        {
            if (activeMenuDialog == null)
            {
                // Use first Menu Dialog found in the scene (if any)
                IMenuDialog md = GameObject.FindObjectOfType<MenuDialog>();
                if (md != null)
                {
                    activeMenuDialog = md;
                }
                
                if (activeMenuDialog == null)
                {
                    // Auto spawn a menu dialog object from the prefab
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/MenuDialog");
                    if (prefab != null)
                    {
                        GameObject go = Instantiate(prefab) as GameObject;
                        go.SetActive(false);
                        go.name = "MenuDialog";
                        activeMenuDialog = go.GetComponent<IMenuDialog>();
                    }
                }
            }

            return activeMenuDialog;
        }

        protected virtual void Awake()
        {
            Button[] optionButtons = GetComponentsInChildren<Button>();
            cachedButtons = optionButtons;

            Slider timeoutSlider = GetComponentInChildren<Slider>();
            cachedSlider = timeoutSlider;

            if (Application.isPlaying)
            {
                // Don't auto disable buttons in the editor
                Clear();
            }
        }

        protected virtual void OnEnable()
        {
            // The canvas may fail to update if the menu dialog is enabled in the first game frame.
            // To fix this we just need to force a canvas update when the object is enabled.
            Canvas.ForceUpdateCanvases();
        }

        protected virtual IEnumerator WaitForTimeout(float timeoutDuration, Block targetBlock)
        {
            float elapsedTime = 0;
            
            Slider timeoutSlider = GetComponentInChildren<Slider>();
            
            while (elapsedTime < timeoutDuration)
            {
                if (timeoutSlider != null)
                {
                    float t = 1f - elapsedTime / timeoutDuration;
                    timeoutSlider.value = t;
                }
                
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            Clear();
            gameObject.SetActive(false);

            HideSayDialog();

            if (targetBlock != null)
            {
                targetBlock.StartExecution();
            }
        }

        #region IMenuDialog implementation

        public virtual Button[] CachedButtons { get { return cachedButtons; } }

        public virtual Slider CachedSlider { get { return cachedSlider; } }

        public virtual void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public virtual void Clear()
        {
            StopAllCoroutines();

            Button[] optionButtons = GetComponentsInChildren<Button>();                     
            foreach (UnityEngine.UI.Button button in optionButtons)
            {
                button.onClick.RemoveAllListeners();
            }

            foreach (UnityEngine.UI.Button button in optionButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }

            Slider timeoutSlider = GetComponentInChildren<Slider>();
            if (timeoutSlider != null)
            {
                timeoutSlider.gameObject.SetActive(false);
            }
        }

        public virtual void HideSayDialog()
        {
            ISayDialog sayDialog = SayDialog.GetSayDialog();
            if (sayDialog != null)
            {
                sayDialog.FadeWhenDone = true;
            }
        }
            
        public virtual bool AddOption(string text, bool interactable, Block targetBlock)
        {
            bool addedOption = false;
            foreach (Button button in cachedButtons)
            {
                if (!button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);

                    button.interactable = interactable;

                    if (interactable && autoSelectFirstButton && !cachedButtons.Select((x) => x.gameObject).Contains(EventSystem.current.currentSelectedGameObject))
                    {
                        EventSystem.current.SetSelectedGameObject(button.gameObject);
                    }

                    Text textComponent = button.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = text;
                    }

                    var block = targetBlock;

                    button.onClick.AddListener(delegate {

                        EventSystem.current.SetSelectedGameObject(null);

                        StopAllCoroutines(); // Stop timeout
                        Clear();

                        HideSayDialog();

                        if (block != null)
                        {
                            #if UNITY_EDITOR
                            // Select the new target block in the Flowchart window
                            var flowchart = block.GetFlowchart();
                            flowchart.SelectedBlock = block;
                            #endif

                            gameObject.SetActive(false);

                            block.StartExecution();
                        }
                    });

                    addedOption = true;
                    break;
                }
            }

            return addedOption;
        }

        public bool AddOption(string text, bool interactable, ILuaEnvironment luaEnv, Closure callBack)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            bool addedOption = false;
            foreach (Button button in CachedButtons)
            {
                if (!button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);

                    button.interactable = interactable;

                    Text textComponent = button.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = text;
                    }

                    button.onClick.AddListener(delegate {

                        StopAllCoroutines(); // Stop timeout
                        Clear();
                        HideSayDialog();

                        if (callBack != null)
                        {
                            luaEnv.RunLuaFunction(callBack, true);
                        }
                    });

                    addedOption = true;
                    break;
                }
            }

            return addedOption;
        }

        public virtual void ShowTimer(float duration, Block targetBlock)
        {
            if (cachedSlider != null)
            {
                cachedSlider.gameObject.SetActive(true);
                gameObject.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(WaitForTimeout(duration, targetBlock));
            }
        }

        public IEnumerator ShowTimer(float duration, ILuaEnvironment luaEnv, Closure callBack)
        {
            if (CachedSlider == null ||
                duration <= 0f)
            {
                yield break;
            }

            CachedSlider.gameObject.SetActive(true);
            StopAllCoroutines();

            float elapsedTime = 0;
            Slider timeoutSlider = GetComponentInChildren<Slider>();

            while (elapsedTime < duration)
            {
                if (timeoutSlider != null)
                {
                    float t = 1f - elapsedTime / duration;
                    timeoutSlider.value = t;
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            Clear();
            gameObject.SetActive(false);
            HideSayDialog();

            if (callBack != null)
            {
                luaEnv.RunLuaFunction(callBack, true);
            }
        }

        public virtual bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public virtual int DisplayedOptionsCount
        {
            get {
                int count = 0;
                foreach (Button button in cachedButtons)
                {
                    if (button.gameObject.activeSelf)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        #endregion
    }    
}
