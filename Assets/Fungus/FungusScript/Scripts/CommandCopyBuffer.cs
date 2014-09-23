using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class CommandCopyBuffer : Sequence 
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
				if (go != null)
				{
					instance = go.GetComponent<CommandCopyBuffer>();
				}
				else
				{
					go = new GameObject("_CommandCopyBuffer");
					instance = go.AddComponent<CommandCopyBuffer>();
					go.hideFlags = HideFlags.HideInHierarchy;
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