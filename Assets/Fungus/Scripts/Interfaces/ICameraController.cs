// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Controller for main camera.Supports several types of camera transition including snap, pan & fade.
    /// </summary>
    public interface ICameraController
    {
        /// <summary>
        /// Full screen texture used for screen fade effect.
        /// </summary>
        /// <value>The screen fade texture.</value>
        Texture2D ScreenFadeTexture { set; }

        /// <summary>
        /// Perform a fullscreen fade over a duration.
        /// </summary>
        void Fade(float targetAlpha, float fadeDuration, System.Action fadeAction);

        /// <summary>
        /// Fade out, move camera to view and then fade back in.
        /// </summary>
        void FadeToView(Camera camera, View view, float fadeDuration, bool fadeOut, System.Action fadeAction);

        /// <summary>
        /// Stop all camera tweening.
        /// </summary>
        void Stop();

        /// <summary>
        /// Moves camera from current position to a target position over a period of time.
        /// </summary>
        void PanToPosition(Camera camera, Vector3 targetPosition, Quaternion targetRotation, float targetSize, float duration, System.Action arriveAction);

        /// <summary>
        /// Activates swipe panning mode. The player can pan the camera within the area between viewA & viewB.
        /// </summary>
        void StartSwipePan(Camera camera, View viewA, View viewB, float duration, float speedMultiplier, System.Action arriveAction);

        /// <summary>
        /// Deactivates swipe panning mode.
        /// </summary>
        void StopSwipePan();
    }
}