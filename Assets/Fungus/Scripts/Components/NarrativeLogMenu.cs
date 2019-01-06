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
        [Tooltip("Show the Narrative Log Menu")]
        [SerializeField] protected bool showLog = true;

        [Tooltip("Show previous lines instead of previous and current")]
        [SerializeField] protected bool previousLines = true;

        [Tooltip("A scrollable text field used for displaying conversation history.")]
        [SerializeField] protected ScrollRect narrativeLogView;

        protected TextAdapter narLogViewtextAdapter = new TextAdapter();
        
        [Tooltip("The CanvasGroup containing the save menu buttons")]
        [SerializeField] protected CanvasGroup narrativeLogMenuGroup;

        protected static bool narrativeLogActive = false;
        
        protected AudioSource clickAudioSource;

        protected LTDescr fadeTween;

        protected static NarrativeLogMenu instance;

        protected virtual void Awake()
        {
            if (showLog)
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
            else
            {
                GameObject logView = GameObject.Find("NarrativeLogView");
                logView.SetActive(false);
                this.enabled = false;
            }

            narLogViewtextAdapter.InitFromGameObject(narrativeLogView.gameObject, true);
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
            BlockSignals.OnBlockEnd += OnBlockEnd;
            NarrativeLog.OnNarrativeAdded += OnNarrativeAdded;
        }
                
        protected virtual void OnDisable()
        {
            WriterSignals.OnWriterState -= OnWriterState;
            SaveManagerSignals.OnSavePointLoaded -= OnSavePointLoaded;
            SaveManagerSignals.OnSaveReset -= OnSaveReset;
            BlockSignals.OnBlockEnd -= OnBlockEnd;
            NarrativeLog.OnNarrativeAdded -= OnNarrativeAdded;
        }

        protected virtual void OnNarrativeAdded()
        {
            UpdateNarrativeLogText();
        }

        protected virtual void OnWriterState(Writer writer, WriterState writerState)
        {
            if (writerState == WriterState.Start)
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

        protected virtual void OnBlockEnd (Block block)
        {
            // At block end update to get the last line of the block
            bool defaultPreviousLines = previousLines;
            previousLines = false;
            UpdateNarrativeLogText();
            previousLines = defaultPreviousLines;
        }

        protected void UpdateNarrativeLogText()
        {
            if (narrativeLogView.enabled)
            {
                narLogViewtextAdapter.Text = FungusManager.Instance.NarrativeLog.GetPrettyHistory();
                
                Canvas.ForceUpdateCanvases();
                narrativeLogView.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();
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
                LeanTween.value(narrativeLogMenuGroup.gameObject, narrativeLogMenuGroup.alpha, 0f, .2f)
                    .setEase(LeanTweenType.easeOutQuint)
                    .setOnUpdate((t) => {
                    narrativeLogMenuGroup.alpha = t;
                }).setOnComplete(() => {
                    narrativeLogMenuGroup.alpha = 0f;
                });
                
            }
            else
            {
                // Switch menu on
                LeanTween.value(narrativeLogMenuGroup.gameObject, narrativeLogMenuGroup.alpha, 1f, .2f)
                    .setEase(LeanTweenType.easeOutQuint)
                    .setOnUpdate((t) => {
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