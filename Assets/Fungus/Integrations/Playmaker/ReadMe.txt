Playmaker
============
Playmaker is a Finite State Machine plugin for Unity that allows developers to create games using a visual scripting methodology that does away with writing code in a manner similar to Fungus.
The FSM allows the creation of various states and events for a given object that allows for the wuick and easy creation of object behaviours and AI for NPCs and enemies.
By combining the Playmaker FSM with Fungus' ability to create engaging, character driven story based games, it is hoped that developers will be able to create games that combine the best features of both.

Installation
============

1. Download and install the Playmaker runtime unitypackage in your Unity project
http://sites.fastspring.com/hutonggames/product/playmaker
https://www.assetstore.unity3d.com/en/#!/content/368

2. Install the Fungus-PlayMaker.unitypackage in your Unity project. This file is located in the same folder as this text file.

3. Open the Fungus-PlayMaker/Scenes/Fungus Playmaker Example Scene for an example of how to use the Spine commands

Usage
============

To write Playmaker variable values to Fungus Variables, use the Action Browser > ScriptControl > Write To Fungus command.
To execute a block in a Fungus flowchart from , use the Action Browser > ScriptControl > Playmaker Execute Block command.
To trigger a global state transition in Playmaker from Fungus, use the Playmaker > Global State Transition command.
To read Fungus variable values into Playmaker Variables, use the Action Browser > ScriptControl > Receive Fungus Variable command.
To write Fungus variable values from the inspector into Playmaker global variables, use the Playmaker > Write To Playmaker command.