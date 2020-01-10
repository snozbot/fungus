// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    namespace Examples
    {
        public class SpriteColorChangeFromFungusMouseEvent : MonoBehaviour
        {
            private SpriteRenderer rend;

            private void Start()
            {
                rend = GetComponent<SpriteRenderer>();
            }

            void OnMouseEventFromFungus()
            {
                rend.color = Color.HSVToRGB(Random.value, Random.Range(0.7f, 0.9f), Random.Range(0.7f, 0.9f));
            }
        }
    }
}