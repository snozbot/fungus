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
    /// Loads a new Unity scene and adds it to the hierarchy. This is useful for
    /// splitting a large game across multiple scene files and loading e.g. levels
    /// on the fly.
    /// The scene to be loaded must be added to the scene list in Build Settings.")]
    /// </summary>
    [CommandInfo("Flow", 
                 "Scene Load Additive", 
                 "Loads a new Unity scene and adds it to the hierarchy. This is useful for " +
                 "splitting a large game across multiple scene files and loading e.g. levels " +
                 "on the fly." +
                 "The scene to be loaded must be added to the scene list in Build Settings.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SceneLoadAdditive : Command
    {
        public AsyncOperation asyncLoad;

        [Tooltip("Name of the scene to load. The scene must also be added to the build settings.")]
        [SerializeField] protected StringData _sceneName = new StringData("");

        [Tooltip("Display the scene, when it has finished loading")]
        [SerializeField] protected bool showSceneWhenLoaded;

        [Tooltip("Flowchart which contains the block to execute. If none is specified then the current Flowchart is used.")]
        [SerializeField] protected Flowchart targetFlowchart;

        [Tooltip("Block to start executing")]
        [SerializeField] protected string targetBlock;

        #region Public members

        public override void OnEnter()
        {
            StartCoroutine(LoadYourAsyncScene());

            if (targetBlock != null)
            {
                // get current Flowchart - or parameter
                var flowchart = GetFlowchart();
                if (flowchart == null)
                {
                    flowchart = targetFlowchart;
                }

                flowchart.ExecuteIfHasBlock(targetBlock);
                Continue();
            }
            else
            {
                Continue();
            }
        }

        IEnumerator LoadYourAsyncScene()
        {
            asyncLoad = SceneManager.LoadSceneAsync(_sceneName.Value, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = showSceneWhenLoaded;

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
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