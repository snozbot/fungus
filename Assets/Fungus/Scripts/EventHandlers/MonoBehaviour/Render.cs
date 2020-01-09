// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired Rendering related message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Render",
                      "The block will execute when the desired Rendering related message for the monobehaviour is received.")]
    [AddComponentMenu("")]
    public class Render : EventHandler
    {

        [System.Flags]
        public enum RenderMessageFlags
        {
            OnPostRender = 1 << 0,
            OnPreCull = 1 << 1,
            OnPreRender = 1 << 2,
            //OnRenderImage = 1 << 3,
            OnRenderObject = 1 << 4,
            OnWillRenderObject = 1 << 5,
            OnBecameInvisible = 1 << 6,
            OnBecameVisible = 1 << 7,
        }

        [Tooltip("Which of the Rendering messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected RenderMessageFlags FireOn = RenderMessageFlags.OnWillRenderObject;

        private void OnPostRender()
        {
            if((FireOn & RenderMessageFlags.OnPostRender) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnPreCull()
        {
            if ((FireOn & RenderMessageFlags.OnPreCull) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnPreRender()
        {
            if ((FireOn & RenderMessageFlags.OnPreRender) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            //TODO
        }

        private void OnRenderObject()
        {
            if ((FireOn & RenderMessageFlags.OnRenderObject) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnWillRenderObject()
        {
            if ((FireOn & RenderMessageFlags.OnWillRenderObject) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnBecameInvisible()
        {
            if ((FireOn & RenderMessageFlags.OnBecameInvisible) != 0)
            {
                ExecuteBlock();
            }
        }

        private void OnBecameVisible()
        {
            if ((FireOn & RenderMessageFlags.OnBecameVisible) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}
