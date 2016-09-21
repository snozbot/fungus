# Flowcharts {#flowcharts}

A fundamental concept of Fungus is the **Flowchart**. Scenes can contain a single Flowchart or multiple Flowcharts.

<!-- **************************************************** -->
## What is a Flowchart?

A Fungus **Flowchart** contains the Blocks in which all your Fungus Commands are located. A Unity scene can contain multiple Flowcharts, and commands can be executing simultaneously in different Flowcharts. However, for many games it is sufficient for one Block in one Flowcart to be executing at any one time.

Here is an example of a Fungus Flowchart:
<br>
![flowchart example](./images/001_what_is/1_example_flowchart.png "flowchart example")
<br>


<!-- **************************************************** -->
## Opening and docking the Flowchart window
You'll need the Fungus Flowchart window when working with Fungus. Open and dock this window somewhere handy by following these steps:

1. Choose menu: ``Tools | Fungus | Flowchart Window``
<br>
![Menu open Fungus window](./images/002_docking/1_menu.png "Menu open Fungus window")
<br>
<br>

2. Drag-and-drop the Flowchart window to the location you wish to dock it:
<br>
![Drag Fungus window](./images/002_docking/2_window.png "Drag Fungus window")
<br>
<br>

3. The Flowchart window is now docked and part of your Unity window layout:
<br>
![Docked Fungus window](./images/002_docking/3_docked.png "Docked Fungus window")


<!-- **************************************************** -->
## Creating a Flowchart
To create a Fungus Flowchart do the following:

1. Choose menu: ```Tools | Fungus | Create Flowchart```
<br>
![menu create Flowchart](./images/005_create_flowchart/1_tools_create.png "menu create Flowchart")
<br>
<br>

2. A new **Flowchart** gameObject should appear in the Hierarchy window.
<br>
![new Flowchart gameobject](./images/005_create_flowchart/2_flowchart_gameobject.png "new Flowchart gameobject")
<br>
<br>

3. Select the **Flowchart** gameObject in the Hierarchy window, and you'll see the **Flowchart's** properties in the Inspector Window:
<br>
![Flowchart properties](./images/005_create_flowchart/3_flowchart_properties.png "Flowchart properties")
<br>
<br>

4. If you have not already displayed the Flowchart Window, you can do so by clicking the Flowchart Window button in the Inspector.

5. As you can see, when a new Flowchat is created a single command Block named "New Block" is automatically created, with the Event handler "Game Started" (so it will start executing Fungus commands as soon as the scene goes into **Play Mode**).

<!-- **************************************************** -->
## Panning the Flowchart window
Panning means moving the contents of the Flowchart window as if they are on a piece of paper. Click and drag with the RIGHT mouse button to pan the contents of the Flowchart window.

![pan flowchart 1](./images/003_panning/1_pan1.png "pan flowchart 1")

![pan flowchart 2](./images/003_panning/2_pan2.png "pan flowchart 2")

![pan flowchart animated](./images/003_panning/animated_drag_to_pan.gif "pan flowchart animated")


<!-- **************************************************** -->
## Zooming the Flowchart window
Zooming refers to making the contents larger or smaller. To zoom the Flowchart window contents either click and drag the UI slider, or use the mouse wheel (or trackpad).

![zoom flowchart 1](./images/004_zooming/1_zoom1.png "zoom flowchart 1")

![zoom flowchart 2](./images/004_zooming/2_zoom2.png "zoom flowchart 2")

![zoom flowchart 2](./images/004_zooming/animated_zoom.gif "zoom flowchart animated")
