// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for Flowchart nodes.
    /// </summary>
    [AddComponentMenu("")]
    public class Node : MonoBehaviour
    {
        [SerializeField] protected Rect nodeRect = new Rect(0, 0, 120, 30);

        #region Public members

        public virtual Rect _NodeRect { get { return nodeRect; } set { nodeRect = value; } }

        #endregion
    }
}