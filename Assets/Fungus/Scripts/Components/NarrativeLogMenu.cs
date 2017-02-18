// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fungus
{
    /// <summary>
    /// A singleton game object which displays a simple UI for the Narrative Log.
    /// </summary>
    public class NarrativeLogMenu : MonoBehaviour 
    {
        [Tooltip("The button that shows conversation history.")]
        [SerializeField] protected Button narrativeLogButton;

        [Tooltip("A scrollable text field used for displaying conversation history.")]
        [SerializeField] protected ScrollRect narrativeLogView;
        
        [Tooltip("The CanvasGroup containing the save menu buttons")]
        [SerializeField] protected CanvasGroup narrativeLogMenuGroup;

        protected static bool narrativeLogActive = false;
        
        protected AudioSource clickAudioSource;

        protected LTDescr fadeTween;

        protected static NarrativeLogMenu instance;

        protected virtual void Awake()
        {
            // Only one instance of NarrativeLogMenu may exist
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            GameObject.DontDestroyOnLoad(this);

            clickAudioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            if (!narrativeLogActive)
            {
                narrativeLogMenuGroup.alpha = 0f;
            }

            //Clear up the lorem ipsum
            UpdateNarrativeLogText();

        }

        protected virtual void OnEnable()
        {
            WriterSignals.OnWriterState += OnWriterState;
            SaveManagerSignals.OnSavePointLoaded += OnSavePointLoaded;
            SaveManagerSignals.OnSaveReset += OnSaveReset;
        }
                
        protected virtual void OnDisable()
        {
            WriterSignals.OnWriterState -= OnWriterState;
            SaveManagerSignals.OnSavePointLoaded -= OnSavePointLoaded;
            SaveManagerSignals.OnSaveReset -= OnSaveReset;
        }

        protected virtual void OnWriterState(Writer writer, WriterState writerState)
        {
            if (writerState == WriterState.End || writerState == WriterState.Start)
            {
                UpdateNarrativeLogText();
            }
        }

        protected virtual void OnSavePointLoaded(string savePointKey)
        {
            UpdateNarrativeLogText();
        }

        protected virtual void OnSaveReset()
        {
            FungusManager.Instance.NarrativeLog.Clear();
            UpdateNarrativeLogText();
        }

        protected void UpdateNarrativeLogText()
        {
            if (narrativeLogView.enabled)
            {
                Debug.Log("Update NarrativeLog");
                var historyText = narrativeLogView.GetComponentInChildren<Text>();
                if (historyText != null)
                {
                    historyText.text = FungusManager.Instance.NarrativeLog.GetPrettyHistory();
                }
            }
        }

        protected void PlayClickSound()
        {
            if (clickAudioSource != null)
            {
                clickAudioSource.Play();
            }
        }

        #region Public methods

        public virtual void ToggleNarrativeLogView()
        {
            if (fadeTween != null)
            {
                LeanTween.cancel(fadeTween.id, true);
                fadeTween = null;
            }

            if (narrativeLogActive)
            {
                // Switch menu off
                LeanTween.value(narrativeLogMenuGroup.gameObject, narrativeLogMenuGroup.alpha, 0f, 0.2f).setOnUpdate((t) => {
                    narrativeLogMenuGroup.alpha = t;
                }).setOnComplete(() => {
                    narrativeLogMenuGroup.alpha = 0f;
                });
            }
            else
            {
                // Switch menu on
                LeanTween.value(narrativeLogMenuGroup.gameObject, narrativeLogMenuGroup.alpha, 1f, 0.2f).setOnUpdate((t) => {
                    narrativeLogMenuGroup.alpha = t;
                }).setOnComplete(() => {
                    narrativeLogMenuGroup.alpha = 1f;
                });
            }

            narrativeLogActive = !narrativeLogActive;
        }
    
        #endregion
    }
}

#endif