// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

#if UNITY_2018_1_OR_NEWER

namespace Fungus
{
    /// <summary>
    /// Component that is automatically added to all tmpro texts that contain links. Caches
    /// local data for that TMProText and uses the TMProLinkAnimLookup to the actual animation.
    /// </summary>
    [DisallowMultipleComponent]
    public class TMProLinkAnimator : MonoBehaviour
    {
        #region Auto Add Component

        /// <summary>
        /// Ensure we are being notified of TMPro changes.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void RegisterAutoAddTMPLinkAnim()
        {
            TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Add(AutoAddTMPLinkAnim);
        }

        /// <summary>
        /// Adds a suite of default link text animations. These can be removed via the
        /// TMProLinkAnimLookup if desired.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RegisterDefaultTMPLinkAnims()
        {
            TMProLinkAnimLookup.AddHelper("jitter", new TMProLinkAnimEffects.ShakeEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                offsetScale = new Vector2(1, 4),
                rotScale = 10
            });
            TMProLinkAnimLookup.AddHelper("angry", new TMProLinkAnimEffects.ShakeEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerWord,
                offsetScale = new Vector2(1, 8),
                rotScale = 4
            });
            TMProLinkAnimLookup.AddHelper("spooky", new TMProLinkAnimEffects.WiggleEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerSection,
                offsetScale = new Vector2(6, 10),
                speed = 1.5f,
            });
            TMProLinkAnimLookup.AddHelper("unknowable", new TMProLinkAnimEffects.WiggleEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                offsetScale = new Vector2(4, 8),
                speed = 1f,
            });
            TMProLinkAnimLookup.AddHelper("wave", new TMProLinkAnimEffects.WaveEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                speed = 10,
                indexStep = 0.3f,
                scale = 2
            });
            TMProLinkAnimLookup.AddHelper("swing", new TMProLinkAnimEffects.PivotEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerWord,
                speed = 10,
                degScale = 15
            });
            TMProLinkAnimLookup.AddHelper("bounce", new TMProLinkAnimEffects.BounceEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerWord,
                speed = 4,
                scale = 5,
            });
            TMProLinkAnimLookup.AddHelper("excited", new TMProLinkAnimEffects.BounceEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                speed = 7,
                scale = 2,
                indexStep = 11.0f / 3.0f,
            });
            TMProLinkAnimLookup.AddHelper("glow", new TMProLinkAnimEffects.PulseEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerWord,
                speed = 4,
                HSVIntensityScale = 0.15f,
                hueScale = 0,
                saturationScale = 0.1f,
                scale = new Vector3(0.06f, 0.06f, 0),
            });
        }

        /// <summary>
        /// Called by TMPro when a text is changed, ensuring link animator is there and
        /// that data is ready for it to use.
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        /// Cache of the TMProLinkAnimator that just forced an update of the TMProText, used to
        /// prevent cyclic updates of TMPro mesh content.
        /// </summary>
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

        protected void Update()
        {
            UpdateAnimation();
        }

        /// <summary>
        /// If there is TMPro and a link to potentially animate then ask the AnimLookup for it
        /// </summary>
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
                    if (TMProLinkAnimLookup.LinkHashToEffect.TryGetValue(curLink.hashCode, out TMProLinkAnimLookup.TMProAnimFunc animFunc))
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