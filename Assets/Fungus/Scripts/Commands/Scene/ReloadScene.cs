// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Reload the current scene
    /// </summary>
    [CommandInfo("Scene",
                 "Reload",
                 "Reload the current scene")]
    [AddComponentMenu("")]
    public class ReloadScene : Command
    {
        [Tooltip("Image to display while loading the scene")]
        [SerializeField]
        protected Texture2D loadingImage;

        public override void OnEnter()
        {
            SceneLoader.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, loadingImage);

            Continue();
        }

        public override string GetSummary()
        {
            return "";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}