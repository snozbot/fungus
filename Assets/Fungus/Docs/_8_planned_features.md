Planned Features
================

# Cross Fade
- Fade a sprite over time to use a different texture.

~~~~~~~~~~~~~~~~~~~~
CrossFade(SpriteRenderer spriteRenderer, Texture2D texture2D, float duration);
~~~~~~~~~~~~~~~~~~~~

# Page Icons
- Set a sprite icon on left or right side of the active page
- Size is specified as a fraction of screen height for consistent display on different device types

~~~~~~~~~~~~~~~~~~~~
SetLeftIcon(Texture2D texture2D, float iconScale);
SetRightIcon(Texture2D texture2D, float iconScale);
SetNoIcon();
~~~~~~~~~~~~~~~~~~~~

# More examples
- Examples Rooms showing usage for common game types (RPG, Visual Novel, Comic Strip, ...)

# Fixed Page
- Option to clamp page to bottom or top section of screen (standard Visual Novel layout)

# Comic Strip Bubbles
- Comic strip style bubbles
- Automatic fitting for bubble text

# Button Hover
- Highlight buttons on mouse hover

# New Unity GUI Support
- Complete rewrite Page system to use new Unity GUI system (when it's released!)
- Support checkboxes, scroll boxes, sliders, advanced font rendering, ...

# Dice Roll Page
- Simple combat system via visual dice roll

