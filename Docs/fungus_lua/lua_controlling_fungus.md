# Controlling %Fungus # {#lua_controlling_fungus}
[TOC]

The %Fungus module provides several functions for working with the standard %Fungus narrative features and flowcharts.

You can control Say and Menu dialogs in much the same way you use Say and Menu commands in a normal %Fungus flowchart.

When you use the menu() function, you supply another Lua function to call when that menu option is selected. Make sure to define the function higher up in the file before referencing it in a menu() call. If you don't explicitly set a SayDialog or MenuDialog object to use default ones are created automatically.

# Narrative example # {#narrative_example}

This example Lua script demonstrates some of the Say and Menu dialog functions. To try it out, add a Lua object to the scene (Tools > %Fungus > Create > Lua) and copy this script into the Lua Script text box. You may also need to add an EventSystem object in the scene (GameObject > UI > Event System) so that the menu buttons will respond to user input.

```lua
-- Display text in a SayDialog
say("Hi there")
say "This syntax also works for say commands"

-- Display a list of options in a MenuDialog
-- (Note the curly braces here!)
local choice = choose{ "Go left", "Go right" }

if choice == 1 then
    say("You chose left")
elseif choice == 2 then
    say("You chose right")
end
```

Note: The curly braces syntax means that the list of options is passed as a single table parameter to the choose() function. It's a shortcut for writing this:
```lua
local choice = choose( {"Go left", "Go right"} )
```

# Say Dialog functions # {#say_dialog_functions}

To use a custom SayDialog:

1. Add as SayDialog to the scene (Tools > %Fungus > Create > SayDialog)
2. Select the Lua object in the hierarchy and find the LuaBindings component.
3. Add a binding to the SayDialog game object, and select the SayDialog component. N.B. Make sure to select the correct component!
4. In Lua script, you can now activate this SayDialog using the setsaydialog() function, by passing the key of the SayDialog binding.

To change the behaviour of the say() function, e.g. to not wait for input when done, do this:
```lua
sayoptions.waitforinput = false
```

You can bind Character objects in a similar fashion, and set the speaking character using the setcharacter() function.

This is the list of available functions for controlling SayDialogs.

```lua
-- Options for configuring Say Dialog behaviour
sayoptions.clearprevious = true | false
sayoptions.waitforinput = true | false
sayoptions.fadewhendone = true | false
sayoptions.stopvoiceover = true | false

-- Set the active saydialog to use with the say function
-- saydialog: A binding to a SayDialog component
setsaydialog(saydialog)

-- Gets the active say dialog, or creates one if none exists yet
getsaydialog()

-- Set the active character on the Say Dialog
-- character: A Fungus.Character component
-- portrait: The name of a sprite in the character's portrait list
setcharacter(character, portrait)

-- Write text to the active Say Dialog
-- text: A string to write to the say dialog
-- voice: A voiceover audioclip to play
say(text, voiceclip)
```

# Menu Dialog functions # {#menu_dialog_functions}

You setup custom MenuDialogs in the same manner as SayDialogs, use the setmenudialog() function to set the active MenuDialog.

The easiest way to display a list of options is using the choose() function. Remember that in Lua array indices start at 1 instead of 0 like in most other languages.

You can set an option to be displayed but not selectable by prepending it with the ~ character.

```lua
local choice = choose { "Option 1", "Option 2", "~Option 3" } 

if choice == 1 then 
	say "Chose option 1"
elseif choice == 2 then 
	say "Chose option 2"
end
-- Option 3 is displayed but can't be selected

say "End options"
```

A useful pattern is to use choose() together with Lua's goto statement and labels. This can be handy for 'flattening out' nested menu options. The [goto statement] doesn't support jumping into the scope of a local variable, but it's easy to work around this by declaring the local variable in the outer scope. You could also use a global variable (by not using the local keyword).

```lua
local choice = 0

choice = choose { "Option A", "Option B" } 
if choice == 1 then goto optionA end
if choice == 2 then goto optionB end

::optionA::
say "Chose option A"
goto endoptions

::optionB::
choice = choose { "Option C", "Option D" } 
if choice == 1 then goto optionC end
if choice == 2 then goto optionD end
goto endoptions

::optionC::
say "Chose option C"
goto endoptions

::optionD::
say "Chose option D"
goto endoptions

::endoptions::
say "End options"
```

The menu() and menutimer() functions provide an alternative way to use the MenuDialog. These functions return immediately, and a callback function is called when the player selects an option from the menu.

This is the list of available MenuDialog functions.

```lua
-- Set the active menudialog to use with the menu function
setmenudialog(menudialog)

-- Gets the active menu dialog, or creates one if none exists yet
getmenudialog()

-- Display a list of menu options and wait for user to choose one.
-- When an option starts with the ~ character it will be displayed but not be selectable.
-- Returns the index of the selected option.
-- Returns 0 if options list is empty. Note: Lua array indices start at 1, not 0).
-- options: an array of option strings. e.g. { "Option 1", "Option 2" }
choose(options)

-- Display a list of menu options and wait for user to choose one, or for a timer to expire.
-- When an option starts with the ~ character it will be displayed but not be selectable.
-- Returns the index of the selected option, or the defaultoption if the timer expires.
-- Returns 0 if options list is empty. Note: Lua array indices start at 1, not 0).
-- options: an array of option strings. e.g. { "Option 1", "Option 2" }
-- duration: Time player has to pick an option.
-- defaultoption: Option index to return if the timer expires.
choosetimer(options, duration, defaultoption)

-- Display a menu button
-- text: text to display on the button
-- callback: function to call when this option is selected
-- interactive (optional): if false, displays the option as disabled
menu(text, callback, interactive)

-- Display a timer during which the player has to choose an option.
-- duration: The length of time to display the timer.
-- callback: Function to call if the timer expires before an option is selected.
menutimer(duration, callback)

-- Clear all currently displayed menu options
clearmenu()
```

# Portrait functions # {#portrait_functions}

Stage portraits can be controlled by using the stage.

First, add your characters and stage to the LuaBindings list.

![Lua Stage Binding]

Then, in a lua script, use the stage commands show, showPortrait and hide to control the portraits on stage.

```lua
-- Show a character at this stage position
stage.show(character, "left")

-- show a character with a specific portrait and move it
-- from one stage position to another.
stage.show(character, "happy", "offscreen right", "right")

-- show a specific portrait
stage.showPortrait(character, "amused")

-- hide a character
stage.hide(character)

-- Hide a character fading out to a position
stage.hide(character, "offscreen left")
```
You can also specify any Portrait option available by using named arguments.

```lua
stage.show{character=character, fromPosition="left", toPosition="right"}

stage.show{character=character, portrait="angry"}

stage.hide{character=character}
```

# Conversation function # {#conversation_function}

The conversation() function allows you to perform long dialogue exchanges with a single function call. Lua's multiline string syntax [[ ]] is handy here. As the conversation() function takes a single string parameter you can also omit the usual function parentheses.

```text
conversation [[
john: Hello!
sherlock: Greetings.
]]
```

See the docs for the @ref conversation "Conversation System".

# Flowchart functions # {#flowchart_functions}

We've added special functions for say() and menu() because these are so common in %Fungus games. To execute any other commands in %Fungus from Lua, you must do it in conjunction with a Flowchart & Block, like this:

1. Add a Flowchart and a Block (e.g. "MyBlock") in the scene.
2. Add the %Fungus commands you want to execute from Lua in the Block. (e.g Play Sound)
3. Add a Lua object to the scene (Tools > %Fungus > Create > Lua)
4. In the LuaBindings component, add a binding to the Flowchart gameobject, and select the Flowchart component.
5. In the LuaScript component, use the runblock() function to execute the Block, passing the bound flowchart and name of the block as parameters.

```lua
runblock(flowchart, "MyBlock")
```

You can also access any Flowchart variable from Lua via the getvar() function.

```lua
-- Assume the 'flowchart' variable is bound to a Flowchart component in LuaBindings
-- MyVar is a string variable defined on the Flowchart

local myvar = getvar(flowchart, "MyVar")

print(myvar.value)

myvar.value = "New value for string"
```

This is the list of available functions for controlling Flowcharts.

```lua
-- Returns the specified Variable in a Flowchart.
-- To access the value of the variable, use its .value property. e.g.
--  v = getvar(flowchart, "FloatVar")
--  v.value = 10    -- Sets the value of the variable to 10
--  f = v.value     -- f now contains 10
-- flowchart: The Fungus Flowchart containing the Block to run.
-- varname: The name of the Variable to get.
getvar(flowchart, varname)

-- Runs the specified Block in a Flowchart
-- flowchart: The Fungus Flowchart containing the Block to run.
-- blockname: The name of the Block to run.
-- commandindex: Index of the command to start execution at
-- nowait: If false, will yield until the Block finishes execution. If true will continue immediately.
runblock(flowchart, blockname, commandindex, nowait)
```

[goto statement]: http://lua-users.org/wiki/GotoStatement

[Lua Stage Binding]: fungus_lua/lua_stage_binding.png

