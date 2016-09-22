# ExecuteLua Command # {#lua_execute_lua}
[TOC]

This command allows you to embed a Lua script to be executed as part of a command sequence in a Block. The Lua script to be executed can be specified in the inspector or in a text file, in the same way as the @ref lua_script component.

You can provide an optional LuaEnvironment to use for the execution. If none is provided then a LuaEnvironment will be selected / created automatically. If a LuaEnvironment has been set on the parent Flowchart then that environment will be used by default. 

The parent Flowchart will be bound to a global Lua variable prior to executing the Lua script. By default the binding name is 'flowchart', but you can change it to anything you want via the Lua Binding Name property of the parent Flowchart.

You can also store the return value from the Lua script in a Flowchart variable.

# Evaluating expressions # {#evaluating}

The %Fungus If command can only compare 2 variables at a time. For more complex expressions involving multiple variables or [math functions], you can use Lua to evaluate the expression and store the result in a Flowchart variable.

1. Add a Flowchart object (Tools > %Fungus > Create > Flowchart). Add some variables to the Flowchart.
2. Add a LuaBindings object (Tools > %Fungus > Create > LuaBindings)
3. Add a binding to the Flowchart gameobject, and select the Flowchart component.
4. In the Flowchart, add an ExecuteLua command in a block to evaluate the complex expression. Store the return value in a Boolean Flowchart variable.
5. Add an If command which checks the value of the Boolean variable.

In the Execute Lua command, you can use the getvar() function to get any Flowchart variables to be used in the expression. 
Note: getvar() returns a reference to the %Fungus variable object. To access the value of this variable use the .value property.

# Example code # {#example_code}

Here's an example Lua script which evaluates a complex expression involving 3 integer variables defined on a bound Flowchart.

```lua
local v1 = getvar(flowchart, "Var1")
local v2 = getvar(flowchart, "Var2")
local v3 = getvar(flowchart, "Var3")

return (v1.value == v2.value or v3.value == 5) 
```

Don't forget to use .value to access the value stored in the variable object, rather than the variable object itself!

[math functions]: http://lua-users.org/wiki/MathLibraryTutorial