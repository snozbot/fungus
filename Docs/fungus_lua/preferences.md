# PlayerPrefs {#lua_preferences}

The [Unity PlayerPrefs](http://docs.unity3d.com/ScriptReference/PlayerPrefs.html) system stores and accesses player preferences between game sessions.

Here's an example of using PlayerPrefs from Lua.

```lua
-- Saving a value to preferences
playerprefs.SetInt("SaveName", 1)
playerprefs.Save()

-- Using a value from preferences
local v = playerprefs.GetInt("SaveName")
print(v) -- Will print out 1
```

# FungusPrefs

The FungusPrefs class is a wrapper around PlayerPrefs that adds support for save slots. 

Basically, if you want to store simple values use PlayerPrefs. If you want to store values using multiple player profiles, you should use FungusPrefs. The slot variable is an integer [0..] and key is a string.

```lua
-- Deletes all saved values for all slots.
prefs.DeleteAll()

-- Removes key and its value from this save slot.
prefs.DeleteKey(slot, key)

-- Returns the float value associated with this key in this save slot, it it exists.
prefs.GetFloat(slot, key, defaultValue)

-- Returns the int value associated with this key in this save slot, it it exists.
prefs.GetInt(slot, key, defaultValue)

-- Returns the string value associated with this key in this save slot, it it exists.
prefs.GetString(slot, key, defaultValue)

-- Returns true if the key exists in this save slot.
prefs.HasKey(slot, key)

-- Writes all modified prefences to disk.
prefs.Save()

-- Sets the value of the preference identified by key for this save slot.
prefs.SetFloat(slot, key, value)

-- Sets the value of the preference identified by key for this save slot.
prefs.SetInt(slot, key, value)

-- Sets the value of the preference identified by key for this save slot.
prefs.SetString(slot, key, value)

-- Returns the combined key used to identify a key within a save slot.
prefs.GetSlotKey(slot, key)
```