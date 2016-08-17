/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{

    [AddComponentMenu("")]
    public class CommandCopyBuffer : Block 
    {
        protected static CommandCopyBuffer instance;
        
        /**
         * Returns the CommandCopyBuffer singleton instance.
         * Will create a CommandCopyBuffer game object if none currently exists.
         */
        static public CommandCopyBuffer GetInstance()
        {
            if (instance == null)
            {
                // Static variables are not serialized (e.g. when playing in the editor)
                // We need to reaquire the static reference to the game object in this case
                GameObject go = GameObject.Find("_CommandCopyBuffer");
                if (go == null)
                {
                    go = new GameObject("_CommandCopyBuffer");
                    go.hideFlags = HideFlags.HideAndDontSave;
                }

                instance = go.GetComponent<CommandCopyBuffer>();
                if (instance == null)
                {
                    instance = go.AddComponent<CommandCopyBuffer>();
                }
            }
            
            return instance;
        }

        protected virtual void Start()
        {
            if (Application.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }

        public virtual bool HasCommands()
        {
            return GetCommands().Length > 0;
        }

        public virtual Command[] GetCommands()
        {
            return GetComponents<Command>();
        }

        public virtual void Clear()
        {
            foreach (Command command in GetCommands())
            {
                DestroyImmediate(command);
            }
        }
    }

}