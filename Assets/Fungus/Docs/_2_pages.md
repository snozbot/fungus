Writing Story Text With Pages
=============================

The [Page](@ref Fungus.Page) component defines an area where the story textbox will appear on screen. 

- The Page can display title text, say text and a list of options.
- The Page automatically hides when there is no remaining story text or options to display.
- The appearance of the text in the Page is controlled via the active [PageStyle](@ref Fungus.PageStyle) object.


# How do I add a Page to a Room?

1. Create an instance of Fungus/Prefabs/Page.prefab and make it a child of the View which it should appear inside.
2. Resize and position the Page as desired.

- - -

# How do I control which Page is used to display text?

1. Ensure there are at least two Page objects in your Room. 
2. Add public Page properties to your Room script and setup references to each Page in the Room.
3. Use the [SetPage](@ref Fungus.GameController.SetPage) command to control which Page rect will be used for rendering text.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	public Page leftPage; // A Page on the left hand side of the Room
	public Page rightPage; // A Page on the right hand side of the Room

	void OnEnter() 
	{
		SetPage(leftPage);
		Say("This text will now display on the left page");
		SetPage(rightPage);
		Say("This text will now display on the right page");
	}
}
~~~~~~~~~~~~~~~~~~~~

- - -

# How do I write story text to the active Page?

1. Use the [SetHeader](@ref Fungus.GameController.SetHeader) command to set the header text to be displayed at the top of the active page.
1. Use the [SetFooter](@ref Fungus.GameController.SetFooter) command to set the footer text to be displayed at the bottom of the active page.
2. Use the [Say](@ref Fungus.GameController.Say) command to display a single line of story text and then wait for the player to click to continue.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	void OnEnter() 
	{
		SetHeader("The Title"); // Sets the header text
		Say("Hello"); // Writes the story text and waits for player to continue
	}
}
~~~~~~~~~~~~~~~~~~~~

- - -

# How do I create a multiple choice menu on the active Page?

- Use the [AddOption](@ref Fungus.GameController.AddOption) command to add an option to the current options list.
- Use the [Choose](@ref Fungus.GameController.AddOption) command to display the current list of options and wait for the player to select an option.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	void OnEnter() 
	{
		AddOption("Option 1", DoOption1);
		AddOption("Option 2", DoOption2);
		Choose("Pick an option");
	}

	// Delegate method for option 1
	void DoOption1()
	{
		Say("Picked option 1");
	}

	// Delegate method for option 2
	void DoOption2()
	{
		Say("Picked option 2");
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- The [AddOption](@ref Fungus.GameController.AddOption) command takes an optional delegate method to call when the player selects the option.
- If no delegate method is provided, the menu is dismissed and the next command in the queue is executed.
- The appearance of the option button is controlled via the active [PageStyle](@ref Fungus.PageStyle) object.

- - -

# How do I control the appearance of Page text?

1. Find Fungus/Prefabs/PageStyle1.prefab in the Fungus library
2. Duplicate and rename this file (e.g. MyPageStyle.prefab)
3. Modify the style settings in the prefab to adjust the appearance.
4. Add a public PageStyle property to your Room script and setup a reference to your PageStyle prefab in the inspector.
5. Use the [SetPageStyle](@ref Fungus.GameController.SetPage) command to change the style used to render Pages.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	public PageStyle myPageStyle; // Reference to a custom PageStyle prefab

	void OnEnter() 
	{
		Say("This text will display using the default PageStyle");
		SetPageStyle(myPageStyle);
		Say("This text will now display using the custom PageStyle");
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes

- PageStyle uses standard Unity GUIStyle properties to control the appearance of the text and box background.
- You can easily change font, text color, bold, italic, etc. for each type of text.
- Modify the texture and settings in the boxStyle to change the appearance of the background box.
- Fungus overrides the font size properties for text to ensure consistent text scaling across devices with varying resolutions.
- Use the xxxFontScale properties to specify font size as a fraction of screen height.
