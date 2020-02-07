// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

#if UNITY_2018_1_OR_NEWER

namespace Fungus.Examples
{
    /// <summary>
    /// Used in TMPro Link Anim Demo, adds a number of sample animation styles. Serves as
    /// an example of how you might configure these effects and variations of them in
    /// your projects
    /// </summary>
    public class TMProLinkStylingDemo : MonoBehaviour
    {
        private void Awake()
        {
            //force clearing and adding our own effects here
            TMProLinkAnimLookup.RemoveAll();

            TMProLinkAnimLookup.AddHelper("shake", new TMProLinkAnimEffects.ShakeEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerCharacter,
                offsetScale = Vector2.one * 2,
                rotScale = 15
            });
            TMProLinkAnimLookup.AddHelper("wiggle", new TMProLinkAnimEffects.WiggleEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerSection,
                offsetScale = Vector2.one * 5
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
                totalStep = 10,
            });
            TMProLinkAnimLookup.AddHelper("pulse", new TMProLinkAnimEffects.PulseEffect()
            {
                mode = TMProLinkAnimEffects.TMPLinkAnimatorMode.PerWord,
                speed = 3,
                HSVIntensityScale = 0.15f,
                hueScale = 0,
                saturationScale = 0,
                scale = new Vector3(0.05f, 0.05f, 0),
            });
        }
    }
}

#endif