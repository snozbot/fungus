Controlling the Camera with Views
=================================

The [View](@ref Fungus.View) component specifies a single camera position. You can add multiple Views in a Fungus room and perform transitions between those Views to move the camera around the Room.

# How do I add a View to a Room?

1. Create an instance of Fungus/Prefabs/View.prefab and make it a child of the Room.
2. Resize and position the Room as desired.

- - -

# How do I move the camera using Views?

1. Add a public View property to your Room script and setup a reference to your View object in the inspector.
2. Use the [SetView](@ref Fungus.GameController.SetView) command to instantly move the camera to a different View.
3. Use the [PanToView](@ref Fungus.GameController.SetView) command to move the camera towards a different View over a period of time.
4. Use the [FadeToView](@ref Fungus.GameController.SetView) command to fade out and then fade in again using a different View.

## C# Code Example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	public View farView; // Zoomed out view
	public View closeView; // Zoomed in view

	void OnEnter() 
	{
		SetView(farView); // Camera moves instantly to this View
		PanToView(closeView, 2f); // Zoom in to the close up view over 2 seconds
		FadeToView(farView, 2f); // Fade out and then fade in to the original view over 2 seconds
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- The [View](@ref Fungus.View) component specifies a single camera position and orthographic size.
- Fungus automatically modifies the position and size of the camera when changing Rooms and transitioning between Views.
- You can add multiple Views in a Fungus room and perform transitions between those Views to move the camera around the Room.
- View transitions cause the command queue to pause until the transition has completed.
- The View class currently only supports the Orthographic mode for Unity's Camera component.

- - -

# How do I safely support multiple aspect ratios?

A common problem in games development is ensuring that graphics are correctly positioned on devices with widely varying resolutions and aspect ratios. The View component helps solve this by drawing colored rectangles which provide a quick visual preview of how the game will appear under different aspect ratios. This simple technique quickly highlights art layout issues, thus saving on testing effort.

- The inner rectangle shows the minimum supported aspect ratio. The default is 4:3 (e.g. iPad)
- The outer rectangle shows the maximum supported aspect ratio. The default is 2:1 (e.g. Surface Pro)

Follow these two simple rules to ensure correct and safe artwork layout.
1. The max aspect ratio box should be fully covered with artwork (i.e. no empty space at edges of box).
2. All interactive elements (e.g. Pages and Buttons) must be fully contained within the min aspect ratio box.

Failure to comply with rule 2 may cause some interactive elements to appear off-screen on devices with smaller aspect ratios.

## Notes
- The min/max aspect ratios and colors can be configured to preview any aspect ratio required.