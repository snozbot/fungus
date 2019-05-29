// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-05-29 - www.ducks-on-the-water.com

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;

namespace Fungus
{
    /// <summary>
    /// Shows an advertisment and calls cancel or success. If no ad is available, calls error.
    /// AdPrepare has to be called before.
    /// It's failsafe to call, even if there is no advertisment available.
    /// </summary>
    [CommandInfo("Ads",
                 "Show Ad",
                 "Show an ad and call the appropriate block on success or user cancel. If no ad is available, it calls the block in error. Please check twice, if you checked Testmode in Ad: Prepare!! This should only be off, if you deploy in stores.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class AdShow : Command
    {
        [Tooltip("Placement ID: e.g. video")]
        [SerializeField] protected StringData placementId = new StringData();

        [Tooltip("Block to execute on success")]
        [SerializeField] protected Block targetBlockSuccess;

        [Tooltip("Block to execute on user cancel")]
        [SerializeField] protected Block targetBlockCancel;

        [Tooltip("Block to execute on error")]
        [SerializeField] protected Block targetBlockError;

        #region Public members

        public override void OnEnter()
        {
#if UNITY_ADS
#if !UNITY_WEBGL
            if (Advertisement.IsReady(placementId))
            {
                StartCoroutine(WaitForAd());
            }
            else
            {
                Debug.Log("Ad error. I swear this has never happened to me before");
                GetComponent<Flowchart>().ExecuteIfHasBlock(targetBlockError.BlockName);
            }
#endif
#endif
            Continue();
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlockSuccess != null)
            {
                connectedBlocks.Add(targetBlockSuccess);
            }
        }

        public override string GetSummary()
        {
            if (targetBlockSuccess == null)
            {
                return "Error: No target block selected";
            }

#if !UNITY_ADS
            return "Error: Activate Advertising in Unity Services";
#else
            return "with Id: '" + placementId.Value + "'";
#endif

        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

#if UNITY_ADS
#if !UNITY_WEBGL
        IEnumerator WaitForAd()
        {
            yield return null;
            /*            while (!Advertisement.IsReady(placementId))
                        {
                            yield return null;
                        }*/

            ShowOptions options = new ShowOptions();
            options.resultCallback = AdCallbackhandler;

            Advertisement.Show(options);
        }

        void AdCallbackhandler(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Ad Finished. Rewarding player...");
                    GetComponent<Flowchart>().ExecuteIfHasBlock(targetBlockSuccess.BlockName);
                    break;
                case ShowResult.Skipped:
                    Debug.Log("Ad skipped. Son, I am dissapointed in you");
                    GetComponent<Flowchart>().ExecuteIfHasBlock(targetBlockCancel.BlockName);
                    break;
                case ShowResult.Failed:
                    Debug.Log("Ad error. I swear this has never happened to me before");
                    GetComponent<Flowchart>().ExecuteIfHasBlock(targetBlockError.BlockName);
                    break;
            }
        }
#endif
#endif

        #endregion

    }
}