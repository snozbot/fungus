# Variables # {#variables}
[TOC]

%Fungus Variables live on flowcharts. Acting like a blackboard, they allow Commands, Blocks and Flowcharts to set and share data.

<!-- **************************************************** -->
# What is a Variable? # {#variable}

If you have scripted or programmed before you are most likely familiar with the concept. At it's most simple, a variable is a named place or location to keep data. Since variables in %fungus live on Flowcharts they exist for the life of the flowchart. 
They are commonly used to keep track of progress or choices made by the player. They are also commonly used to pass data from one command to another.

Variables do not automatically save and restore themselves between runs of the application. The saving mechanisms can, however, be used to load and save variables to disk.

<!-- **************************************************** -->
# The Variable list # {#variable_list_window}

they are in the flowchart window
![variable window]

also on the flowchart inspector
![variable inspector]

The large variable button toggles showing the variable list. Hiding it can be useful when you have a large number of variables on a flowchart and need the extra screen real estate to see the flowchart blocks within the window or other components on the GameObject you want to be able to see.

This is where fungus variables are added (created) and removed. Clicking the plus button shows a menu to select the type of variable to add. Pressing the minus button will remove the currently selected variable from the list.
![variable add button]

Variables will highlight when they are being referenced by the currently selected %Commmand.
![variable highlight]

Each variable needs a name, this is how it is shown to you in command drop downs and how it is referred to when using variable substitution within strings. You can set the value of variables directly in the variable list for most variable types. In all cases you will want to make sure that the starting value makes sense. For example, an Integer variable called Lives, might want to start at 3.

<!-- **************************************************** -->
# What is a Variable Type? # {#variable_type}

Mimicing c#, %Fungus Variables declare the type of data they are going to contain. 
![variable data type]

Simple applications will most commonly use Booleans, variables that hold either true or false, and Strings, Unicode characters, such as words, sentences and paragraphs.

<!-- **************************************************** -->
# What is Variable Scope? # {#variable_scope}

The scope of the variable determines how the variable is used by fungus and how it is exposed to other parts of the system.
![variable scope]

Presently these can be set to, private, public or global.

Private is not directly available to other flowcharts. Indicating that the variable is only relevant and only to be used by the flowchart it is declared on. This does not limit its ability to be used by commands.

Public indicates that the variable can be found and is intended to be used or modified by other flowcharts. Those other flowcharts will need to be able to directly access the flowchart the variable is declared on to modify it. Public also makes the variable available to other flowcharts during variable substitution.

Global allow for sharing state among all flowcharts, without direct access to the other flowcharts. It also allows for the value of the variable to outlive the flowchart that declared it. All flowcharts that have a variable of the same type and name and are global access have the same underlying value, stored on the FungusManager.

During variable substitution, the flowchart looks for name matches on itself. Then public variables on all active flowcharts.

<!-- **************************************************** -->
# What is a Fungus Variable Data? # {#variable_data}

Commands within blocks may use either a Variable reference directly or a Variable Data. Variable Data is a mechanism for %Fungus to use either a Fungus Variable or a manually entered value of the same type. This allows for more generic commands.
![variable data]



[variable window]: ./variables/variable_window.png "Variable section of the flowchart window"
[variable inspector]: ./variables/variable_inspector.png "Variable section of the flowchart inspector"
[variable add button]: ./variables/variable_add_button.png "Plus button shows selection of variable types to add."
[variable highlight]: ./variables/variable_highlight.png "The variable(s) being referenced by the current command are highlighted in the list."
[variable data type]: ./variables/variable_data_type.png "Fungus supports a number of types of variables"
[variable scope]: ./variables/variable_scope.png "Scope determines where and how the variable is accessable."
[variable data]: ./variables/variable_data.png "Data with a manual value entered. The drop down shows the compatible variable types on this flowchart that could be used instead of the manually entered value."


<!--
-adding your own variables to fungus
-variableinfo
-variableproperty
-accessing variable in c# code
-->