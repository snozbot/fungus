// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Fades the camera out and in again at a position specified by a View object.
    /// </summary>
    [CommandInfo("Camera", 
                 "Fade To View", 
                 "Fades the camera out and in again at a position specified by a View object.")]
    [AddComponentMenu("")]
    public class FadeToView : Command 
    {
        [Tooltip("Time for fade effect to complete")]
        [SerializeField] protected float duration = 1f;

        [Tooltip("Fade from fully visible to opaque at start of fade")]
        [SerializeField] protected bool fadeOut = true;

        [Tooltip("View to transition to when Fade is complete")]
        [SerializeField] protected View targetView;

        [Tooltip("Wait until the fade has finished before executing next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        [Tooltip("Color to render fullscreen fade texture with when screen is obscured.")]
        [SerializeField] protected Color fadeColor = Color.black;

        [Tooltip("Optional texture to use when rendering the fullscreen fade effect.")]
        [SerializeField] protected Texture2D fadeTexture;

        [Tooltip("Camera to use for the fade. Will use main camera if set to none.")]
        [SerializeField] protected Camera targetCamera;

        [SerializeField] protected LeanTweenType fadeTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] protected LeanTweenType orthoSizeTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] protected LeanTweenType posTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] protected LeanTweenType rotTweenType = LeanTweenType.easeInOutQuad;

        protected virtual void Start()
        {
            AcquireCamera();
        }

        protected virtual void AcquireCamera()
        {
            if (targetCamera != null)
            {
                return;
            }

            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = GameObject.FindObjectOfType<Camera>();
            }
        }

        #region Public members

        /// <summary>
        /// View to transition to when Fade is complete
        /// </summary>
        public virtual View TargetView { get { return targetView; } }

        public override void OnEnter()
        {
            AcquireCamera();
            if (targetCamera == null ||
                targetView == null)
            {
                Continue();
                return;
            }

            var cameraManager = FungusManager.Instance.CameraManager;

            if (fadeTexture)
            {
                cameraManager.ScreenFadeTexture = fadeTexture;
            }
            else
            {
                cameraManager.ScreenFadeTexture = CameraManager.CreateColorTexture(fadeColor, 32, 32);
            }

            cameraManager.FadeToView(targetCamera, targetView, duration, fadeOut, delegate { 
                if (waitUntilFinished)
                {
                    Continue();
                }
            }, fadeTweenType, orthoSizeTweenType, posTweenType, rotTweenType);

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnStopExecuting()
        {
            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.Stop();
        }

        public override string GetSummary()
        {
            if (targetView == null)
            {
                return "Error: No view selected";
            }
            else
            {
                return targetView.name;
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }
}