# LuaUtils # {#lua_utils}
[TOC]

LuaUtils is a component that extends the Lua environment with some commonly used functionality. 

It can be accessed from Lua scripts via the 'luautils' global variable. This component mostly does a lot of setup work in the background, but it also provides some handy functions for instantiating, finding and destroying gameobjects in the scene.

# Example # {#lua_utils_example}

Here's an example of the kind of thing you can do:

```lua
local go = luautils.Find("MyObject") -- Find a game object by name
luautils.Destroy(go) -- Destroy it
```

# GameObject Functions # {#gameobject_functions}

This is the list of GameObject functions provided in luautils.

```lua
-- Find a game object by name and returns it.
GameObject Find(string name)

-- Returns one active GameObject tagged tag. Returns null if no GameObject was found.
GameObject FindWithTag(string tag)

-- Returns a list of active GameObjects tagged tag. Returns empty array if no GameObject was found.
GameObject[] FindGameObjectsWithTag(string tag)
			
-- Create a copy of a GameObject.
-- Can be used to instantiate prefabs.
GameObject Instantiate(GameObject go)

-- Destroys an instance of a GameObject.
Destroy(GameObject go)

-- Spawns an instance of a named prefab resource.
-- The prefab must exist in a Resources folder in the project.
GameObject Spawn(string resourceName)
```

# Registering C# Types # {#registering_types}

The most important function of the LuaUtils component is registering C# types so that instances of those types can be accessed from Lua scripts. 

In order to access the members of a C# type from Lua, the type first has to be registered with MoonSharp. Note that for objects added using the LuaBindings component, the relevant types are registered automatically.

In some cases however, you will need to register a type explicitly. The easiest way to do this is by adding the type's name to the FungusTypes.txt or UnityTypes.txt JSON files referenced by the LuaUtils component. You can also create your own JSON files to register additional types. Note that types that are not contained in the main application DLL will need to use the [namespace qualified type name] in the JSON file.

# Example JSON Type File # {#example_json_file}

Example of a types JSON file:
```json
{
    "registerTypes" : [
        "Fungus.Block"
    ],
    "extensionTypes" : [
        "Fungus.LuaExtensions"
    ]
}
```

# Registering Types Directly # {#register_types_directly}

If you need to register types directly from C#, or do a more complex type of registration, you can use the MoonSharp UserData class to do this. See the MoonSharp documentation for a list of supported registration methods. A good place to register C# types is in the Awake method of a custom component.

# Other Utilities # {#lua_utils_other}

LuaUtils creates bindings for several useful C# classes and components so that you can access them from Lua script.

| Binding name 		| Description |
| ----------------- | ----------- |
| time 				| The [Unity Time class]. e.g. 'time.deltaTime' returns the delta time for this frame |
| playerprefs		| The @ref lua_preferences "Lua Preferences" class. Used for saving data to disk. |
| prefs 			| The @ref lua_preferences "Fungus Prefs" class, our own wrapper around PlayerPrefs that adds a slots systems. |
| factory 			| The @ref lua_utils "PODTypeFactory" class for creating common plain-old-data types |
| luaenvironment 	| The LuaEnvironment component used to execute Lua scripts |
| luautils 			| A reference to the LuaUtils component itself |
| test 				| Support for @ref lua_unity_test_tools (if installed) |
| stringtable 		| The FungusLua localisation @ref lua_string_table |

# PODFactory # {#pod_factory}

Due to limitations in C# / Mono, MoonSharp has limited support for working with Plain-Old-Data (struct) types like [Vector3](http://docs.unity3d.com/ScriptReference/Vector3.html), [Color](http://docs.unity3d.com/ScriptReference/Color.html), etc. 

The best approach here is to treat POD properties as immutable objects, and never try to modify a POD variable that has been acquired from a C# object. Instead, you should construct a new POD object, populate it with the required values and then pass that object in calls to C# code. The LuaUtils PODFactory class helps do this for common Unity types.

```lua
-- Returns a new Color object
local c = luautils.factory.color(1,1,1,1)

-- Returns a new Vector2 object
local v2 = luautils.factory.vector2(1, 2)

-- Returns a new Vector3 object
local v3 = luautils.factory.vector3(1, 2, 3)

-- Returns a new Vector4 object
local v4 = luautils.factory.vector4(1, 2, 3, 4)

-- Returns a new Quaternion object
local q = luautils.factory.quaternion(float x, float y, float z) -- Rotation in euler angles
			
-- Returns a new Rect object
local r = luautils.factory.rect(float x, float y, float width, float height)
```

[namespace qualified type name]: https://msdn.microsoft.com/en-us/library/system.type.assemblyqualifiedname(v=vs.110).aspx
[Unity Time class]: http://docs.unity3d.com/ScriptReference/Time.html