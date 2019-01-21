// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Fades a sprite to a target color over a period of time.
    /// </summary>
    [CommandInfo("Sprite", 
                 "Fade Sprite", 
                 "Fades a sprite to a target color over a period of time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class FadeSprite : Command
    {
        [Tooltip("Sprite object to be faded")]
        [SerializeField] protected SpriteRenderer spriteRenderer;

        [Tooltip("Length of time to perform the fade")]
        [SerializeField] protected FloatData _duration = new FloatData(1f);

        [Tooltip("Target color to fade to. To only fade transparency level, set the color to white and set the alpha to required transparency.")]
        [SerializeField] protected ColorData _targetColor = new ColorData(Color.white);

        [Tooltip("Wait until the fade has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        #region Public members

        public override void OnEnter()
        {
            if (spriteRenderer == null)
            {
                Continue();
                return;
            }

            SpriteFader.FadeSprite(spriteRenderer, _targetColor.Value, _duration.Value, Vector2.zero, delegate {
                if (waitUntilFinished)
                {
                    Continue();
                }
            });

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (spriteRenderer == null)
            {
                return "Error: No sprite renderer selected";
            }

            return spriteRenderer.name + " to " + _targetColor.Value.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(221, 184, 169, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _duration.floatRef == variable || _targetColor.colorRef == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;
        [HideInInspector] [FormerlySerializedAs("targetColor")] public Color targetColorOLD;

        protected virtual void OnEnable()
        {
            if (durationOLD != default(float))
            {
                _duration.Value = durationOLD;
                durationOLD = default(float);
            }
            if (targetColorOLD != default(Color))
            {
                _targetColor.Value = targetColorOLD;
                targetColorOLD = default(Color);
            }
        }

        #endregion
    }
}