using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	// Manages the global state information for the game
	// This is implemented as a separate class to support saving and loading game state easily.
	public class GameState 
	{		
		protected Dictionary<string, bool> flags = new Dictionary<string, bool>();
		
		protected Dictionary<string, int> counters = new Dictionary<string, int>();
		
		protected Dictionary<string, int> inventory = new Dictionary<string, int>();

		public GameState DeepClone()
		{
			GameState clone = new GameState();

			foreach (string key in flags.Keys)
				clone.flags[key] = flags[key];
			foreach (string key in counters.Keys)
				clone.counters[key] = counters[key];
			foreach (string key in inventory.Keys)
				clone.inventory[key] = inventory[key];

			return clone;
		}
		
		public void ClearFlags()
		{
			flags.Clear();
		}
		
		public bool GetFlag(string key)
		{
			if (flags.ContainsKey(key))
			{
				return flags[key];
			}
			return false;
		}
		
		public void SetFlag(string key, bool value)
		{
			flags[key] = value;
		}
		
		public void ClearCounters()
		{
			counters.Clear();
		}
		
		public int GetCounter(string key)
		{
			if (counters.ContainsKey(key))
			{
				return counters[key];
			}
			return 0;
		}
		
		public void SetCounter(string key, int value)
		{
			counters[key] = value;
		}
		
		public void ClearInventory()
		{
			inventory.Clear();
		}
		
		public int GetInventory(string key)
		{
			if (inventory.ContainsKey(key))
			{
				return inventory[key];
			}
			return 0;
		}
		
		public void SetInventory(string key, int value)
		{
			inventory[key] = value;
		}
	}
}