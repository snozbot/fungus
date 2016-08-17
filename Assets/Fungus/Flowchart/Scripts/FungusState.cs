/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{

    // Used by the Flowchart window to serialize the currently active Flowchart object
    // so that the same Flowchart can be displayed while editing & playing.
    [AddComponentMenu("")]
    public class FungusState : MonoBehaviour 
    {
        public Flowchart selectedFlowchart;
    }

}