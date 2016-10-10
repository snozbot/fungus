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
    /// <summary>
    /// Presents multiple choice buttons to the players.
    /// </summary>
    public class MenuDialog : MonoBehaviour
    {
        [Tooltip("Automatically select the first interactable button when the menu is shown.")]
        [SerializeField] protected bool autoSelectFirstButton = false;

        protected Button[] cachedButtons;

        protected Slider cachedSlider;

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

        protected IEnumerator CallBlock(Block block)
        {
            yield return new WaitForEndOfFrame();
            block.StartExecution();
        }

        protected IEnumerator CallLuaClosure(LuaEnvironment luaEnv, Closure callback)
        {
            yield return new WaitForEndOfFrame();
            if (callback != null)
            {
                luaEnv.RunLuaFunction(callback, true);
            }
        }

        #region Public members

        /// <summary>
        /// Currently active Menu Dialog used to display Menu options
        /// </summary>
        public static MenuDialog ActiveMenuDialog { get; set; }

        /// <summary>
        /// Returns a menu dialog by searching for one in the scene or creating one if none exists.
        /// </summary>
        public static MenuDialog GetMenuDialog()
        {
            if (ActiveMenuDialog == null)
            {
                // Use first Menu Dialog found in the scene (if any)
                var md = GameObject.FindObjectOfType<MenuDialog>();
                if (md != null)
                {
                    ActiveMenuDialog = md;
                }

                if (ActiveMenuDialog == null)
                {
                    // Auto spawn a menu dialog object from the prefab
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/MenuDialog");
                    if (prefab != null)
                    {
                        GameObject go = Instantiate(prefab) as GameObject;
                        go.SetActive(false);
                        go.name = "MenuDialog";
                        ActiveMenuDialog = go.GetComponent<MenuDialog>();
                    }
                }
            }

            return ActiveMenuDialog;
        }

        /// <summary>
        /// A cached list of button objects in the menu dialog.
        /// </summary>
        /// <value>The cached buttons.</value>
        public virtual Button[] CachedButtons { get { return cachedButtons; } }

        /// <summary>
        /// A cached slider object used for the timer in the menu dialog.
        /// </summary>
        /// <value>The cached slider.</value>
        public virtual Slider CachedSlider { get { return cachedSlider; } }

        /// <summary>
        /// Sets the active state of the Menu Dialog gameobject.
        /// </summary>
        public virtual void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        /// <summary>
        /// Clear all displayed options in the Menu Dialog.
        /// </summary>
        public virtual void Clear()
        {
            StopAllCoroutines();

            var optionButtons = GetComponentsInChildren<Button>();
            for (int i = 0; i < optionButtons.Length; i++)
            {
                var button = optionButtons[i];
                button.onClick.RemoveAllListeners();
            }

            for (int i = 0; i < optionButtons.Length; i++)
            {
                var button = optionButtons[i];
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

        /// <summary>
        /// Hides any currently displayed Say Dialog.
        /// </summary>
        public virtual void HideSayDialog()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog != null)
            {
                sayDialog.FadeWhenDone = true;
            }
        }

        /// <summary>
        /// Adds the option to the list of displayed options. Calls a Block when selected.
        /// Will cause the Menu dialog to become visible if it is not already visible.
        /// </summary>
        /// <returns><c>true</c>, if the option was added successfully.</returns>
        /// <param name="text">The option text to display on the button.</param>
        /// <param name="interactable">If false, the option is displayed but is not selectable.</param>
        /// <param name="targetBlock">Block to execute when the option is selected.</param>
        public virtual bool AddOption(string text, bool interactable, Block targetBlock)
        {
            bool addedOption = false;
            for (int i = 0; i < cachedButtons.Length; i++)
            {
                var button = cachedButtons[i];
                if (!button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);
                    button.interactable = interactable;
                    if (interactable && autoSelectFirstButton && !cachedButtons.Select(x => x.gameObject).Contains(EventSystem.current.currentSelectedGameObject))
                    {
                        EventSystem.current.SetSelectedGameObject(button.gameObject);
                    }
                    Text textComponent = button.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = text;
                    }
                    var block = targetBlock;
                    button.onClick.AddListener(delegate
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        StopAllCoroutines();
                        // Stop timeout
                        Clear();
                        HideSayDialog();
                        if (block != null)
                        {
                            var flowchart = block.GetFlowchart();
                            #if UNITY_EDITOR
                            // Select the new target block in the Flowchart window
                            flowchart.SelectedBlock = block;
                            #endif
                            gameObject.SetActive(false);
                            // Use a coroutine to call the block on the next frame
                            // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                            flowchart.StartCoroutine(CallBlock(block));
                        }
                    });
                    addedOption = true;
                    break;
                }
            }

            return addedOption;
        }

        /// <summary>
        /// Adds the option to the list of displayed options, calls a Lua function when selected.
        /// Will cause the Menu dialog to become visible if it is not already visible.
        /// </summary>
        /// <returns><c>true</c>, if the option was added successfully.</returns>
        public virtual bool AddOption(string text, bool interactable, LuaEnvironment luaEnv, Closure callBack)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            bool addedOption = false;
            for (int i = 0; i < CachedButtons.Length; i++)
            {
                var button = CachedButtons[i];
                if (!button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(true);
                    button.interactable = interactable;
                    var textComponent = button.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = text;
                    }

                    // Copy to local variables 
                    LuaEnvironment env = luaEnv;
                    Closure call = callBack;
                    button.onClick.AddListener(delegate
                    {
                        StopAllCoroutines();
                        // Stop timeout
                        Clear();
                        HideSayDialog();
                        // Use a coroutine to call the callback on the next frame
                        StartCoroutine(CallLuaClosure(env, call));
                    });
                    
                    addedOption = true;
                    break;
                }
            }

            return addedOption;
        }

        /// <summary>
        /// Show a timer during which the player can select an option. Calls a Block when the timer expires.
        /// </summary>
        /// <param name="duration">The duration during which the player can select an option.</param>
        /// <param name="targetBlock">Block to execute if the player does not select an option in time.</param>
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

        /// <summary>
        /// Show a timer during which the player can select an option. Calls a Lua function when the timer expires.
        /// </summary>
        public virtual IEnumerator ShowTimer(float duration, LuaEnvironment luaEnv, Closure callBack)
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

        /// <summary>
        /// Returns true if the Menu Dialog is currently displayed.
        /// </summary>
        public virtual bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Returns the number of currently displayed options.
        /// </summary>
        public virtual int DisplayedOptionsCount
        {
            get {
                int count = 0;
                for (int i = 0; i < cachedButtons.Length; i++)
                {
                    var button = cachedButtons[i];
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
