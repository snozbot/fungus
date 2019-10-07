using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Fungus.TMProLinkAnimEffects;

#if UNITY_2018_1_OR_NEWER
namespace Fungus.Examples
{
    public class TMProLinkStylingDemo : MonoBehaviour
    {
        void Awake()
        {
            TMProLinkAnimLookup.AddHelper("shake", new TMProLinkAnimEffects.ShakeEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerLine,
                offsetScale = 2,
                rotScale = 15
            });
            TMProLinkAnimLookup.AddHelper("wiggle", new TMProLinkAnimEffects.WiggleEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerSection,
                scale = 5
            });
            TMProLinkAnimLookup.AddHelper("wave", new TMProLinkAnimEffects.WaveEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                speed = 10,
                indexStep = 0.3f,
                scale = 2
            });
            TMProLinkAnimLookup.AddHelper("pivot", new TMProLinkAnimEffects.PivotEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerWord,
                speed = 10,
                degScale = 15
            });
            TMProLinkAnimLookup.AddHelper("rainbow", new TMProLinkAnimEffects.RainbowEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                speed = 2,
                indexStep = 0.1f,
                s = 0.8f,
                v = 0.8f
            });
            TMProLinkAnimLookup.AddHelper("ascend", new TMProLinkAnimEffects.AscendEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                totalStep = 10
            });
        }
    }
}
#endif