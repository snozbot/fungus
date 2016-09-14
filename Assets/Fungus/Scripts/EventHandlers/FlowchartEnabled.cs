// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the Flowchart game object is enabled.
    /// </summary>
    [EventHandlerInfo("",
                      "Flowchart Enabled",
                      "The block will execute when the Flowchart game object is enabled.")]
    [AddComponentMenu("")]
    public class FlowchartEnabled : EventHandler
    {   
        protected virtual void OnEnable()
        {
            // Blocks use coroutines to schedule command execution, but Unity's coroutines are
            // sometimes unreliable when enabling / disabling objects.
            // To workaround this we execute the block on the next frame.
            Invoke("DoEvent", 0);
        }

        protected virtual void DoEvent()
        {
            ExecuteBlock();
        }
    }
}
