# LuaBindings component {#lua_bindings}

The LuaBindings component allows you map gameobjects and components in your scenes to Lua variables which you can then access in your Lua scripts. You can bind to any component, including standard Unity components, components from the Unity Asset Store and your own custom scripts.

As well as scene GameObjects and components, you can bind to any Unity object in your project. This includes things like Prefabs, Materials, TextAssets, Textures, ScriptableObjects, etc. 

![LuaBinding](images/lua_bindings.png)

# Adding LuaBindings

To setup LuaBindings in your scene:

1. Create a LuaBindings object (Tools > Fungus > Create > LuaBindings)
2. Drag the Unity object you want to access to the Object field in the Object Bindings list.
3. The Key field is automatically populated based on the object name. This will be the variable name you use to access the bound object from Lua script. You can change this key to whatever string you prefer.
4. If the bound object is a GameObject, you can optionally select a component within it to bind to.

# Using a global table

The bindings specified in a LuaBindings component are automatically registered as global variables in all LuaEnvironments in the scene at startup. 

Registering as global variables is convenient when writing short scripts, but for more complex scripts it could cause problems if you accidentally define another variable with the same name as a binding. To avoid this problem, you can use the Table Name property to register bindings in a global table to add a degree of namespace safety.

For example, if your binding is called 'camera' and you've set Table Name to "myobjects", you would access the camera object like this:
```lua
myobjects.camera
```

Note that by default the LuaBindings component will register its bindings with all LuaEnvironments in the scene. If you don't want this behaviour, deselect the 'All Environments' option and select the specific LuaEnvironment you want to use instead.

# Finding member info

The Member Info dropdown box lets you to quickly lookup properties and methods for any bound object. When you select a member, a description of the member is displayed together with the Lua script needed to access it. When binding to standard Unity objects, you can also check the API docs to find out more about the supported methods and properties.

# Register Types option

In order to access a C# type from Lua, that type has to be registered with MoonSharp. When the Register Types option is selected, LuaBindings will automatically register the types of bound objects and all public properties & methods that the type uses.


