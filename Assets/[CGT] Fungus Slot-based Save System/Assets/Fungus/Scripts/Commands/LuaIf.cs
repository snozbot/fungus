// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the test expression is true, execute the following command block.
    /// </summary>
	[CommandInfo("Flow", 
	             "Lua If", 
	             "If the test expression is true, execute the following command block.")]
    [AddComponentMenu("")]
    public class LuaIf : LuaCondition
    {
        #region Public members
        
        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}