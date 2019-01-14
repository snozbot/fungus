// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-01-14 - www.ducks-on-the-water.com

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace Fungus
{
    /// <summary>
    /// Removes an additively loaded scene from the hierarchy.
    /// The scene to be loaded must be added to the scene list in Build Settings.")]
    /// </summary>
    [CommandInfo("Scene",
                 "Remove Additive", 
                 "Removes an additively loaded scene from the hierarchy." +
                 "The scene to be removed must have been added to the scene list in Build Settings.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SceneRemoveAdditive : Command
    {
        [Tooltip("Name of the scene to remove. The scene must also be added to the build settings.")]
        [SerializeField] protected StringData _sceneName = new StringData("");

        #region Public members

        public override void OnEnter()
        {
            if (SceneManager.GetSceneByName(_sceneName.Value).IsValid())
            {
                SceneManager.UnloadSceneAsync(_sceneName.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_sceneName.Value.Length == 0)
            {
                return "Error: No scene name selected";
            }

            return _sceneName.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion

    }
}