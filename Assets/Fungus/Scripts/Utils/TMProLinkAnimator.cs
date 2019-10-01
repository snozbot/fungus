using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_2018_1_OR_NEWER
namespace Fungus
{
    //Component that is automatically added to all tmpro texts that contain links,
    //  caches local data for that TMProText and uses the TMProLinkAnimLookup to
    //  to the actual animation
    [DisallowMultipleComponent]
    public class TMProLinkAnimator : MonoBehaviour
    {
        #region Auto Add Component

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterAutoAddTMPLinkAnim()
        {
            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Add(AutoAddTMPLinkAnim);
        }

        public static void AutoAddTMPLinkAnim(object obj)
        {
            if (Application.isPlaying)
            {
                var tmp = (obj as TMPro.TMP_Text);
                if (forcedUpdater == null && tmp.textInfo.linkCount > 0)
                {
                    var tmpa = tmp.GetComponent<TMProLinkAnimator>();
                    if (tmpa == null)
                    {
                        tmpa = tmp.gameObject.AddComponent<TMProLinkAnimator>();
                        tmpa.TMProComponent = tmp;
                    }

                    tmpa.SetDirty();
                    tmpa.UpdateAnimation();
                }
            }
        }

        protected static TMProLinkAnimator forcedUpdater;

        #endregion Auto Add Component

        public TMPro.TMP_Text TMProComponent { get; protected set; }
        public bool dirty = true;
        protected bool needsToForceMeshUpdate = true;

        public TMPro.TMP_MeshInfo[] CachedMeshInfo { get; protected set; }

        public void SetDirty()
        {
            dirty = true;
        }

        protected void Awake()
        {
            if (TMProComponent == null)
            {
                TMProComponent = GetComponent<TMPro.TMP_Text>();
            }
        }

        protected void Start()
        {
            SetDirty();
        }

        protected void Update()
        {
            UpdateAnimation();
        }

        protected void UpdateAnimation()
        {
            //could we anim
            if (TMProComponent != null && enabled)
            {
                bool requiresVertexDataUpdate = false;

                //for all found links
                for (int i = 0; i < TMProComponent.textInfo.linkCount; i++)
                {
                    var curLink = TMProComponent.textInfo.linkInfo[i];

                    //if a static lookup exists, ask it to run its animation with us as the context
                    if(TMProLinkAnimLookup.LinkHashToEffect.TryGetValue(curLink.hashCode, out TMProLinkAnimLookup.TMProAnimFunc animFunc))
                    {
                        //only update caches if we actually need it
                        HandleDirty();

                        animFunc(this, curLink.linkTextfirstCharacterIndex, curLink.linkTextLength);

                        requiresVertexDataUpdate = true;
                    }
                }

                // Push changes if we actually found a matching effect
                if (requiresVertexDataUpdate)
                {
                    TMProComponent.UpdateVertexData();
                }
            }
        }

        protected void HandleDirty()
        {
            //update internal cache if underlying data has changed 
            if (dirty)
            {
                if (needsToForceMeshUpdate)
                {
                    forcedUpdater = this;
                    TMProComponent.ForceMeshUpdate();
                    forcedUpdater = null;
                }
                CachedMeshInfo = TMProComponent.textInfo.CopyMeshInfoVertexData();
                dirty = false;
            }
        }
    }
}
#endif