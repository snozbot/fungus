# Narrative commands # {#narrative_commands}

[TOC]
# Clear Menu # {#ClearMenu}
Clears the options from a menu dialogue

Defined in Fungus.ClearMenu

Property | Type | Description
 --- | --- | ---
Menu Dialog | Fungus.MenuDialog | Menu Dialog to clear the options on

# Control Stage # {#ControlStage}
Controls the stage on which character portraits are displayed.

Defined in Fungus.ControlStage

Property | Type | Description
 --- | --- | ---
Stage | Fungus.Stage | Stage to display characters on
Replaced Stage | Fungus.Stage | Stage to swap with
Use Default Settings | System.Boolean | Use Default Settings
Fade Duration | System.Single | Fade Duration
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command
Display | Fungus.StageDisplayType | Display type

# Conversation # {#Conversation}
Do multiple say and portrait commands in a single block of text. Format is: [character] [portrait] [stage position] [: Story text]

Defined in Fungus.Conversation
# Menu # {#Menu}
Displays a button in a multiple choice menu

Defined in Fungus.Menu

Property | Type | Description
 --- | --- | ---
Text | System.String | Text to display on the menu button
Description | System.String | Notes about the option text for other authors, localization, etc.
Target Block | Fungus.Block | Block to execute when this option is selected
Hide If Visited | System.Boolean | Hide this option if the target block has been executed previously
Interactable | Fungus.BooleanData | If false, the menu option will be displayed but will not be selectable
Set Menu Dialog | Fungus.MenuDialog | A custom Menu Dialog to use to display this menu. All subsequent Menu commands will use this dialog.
Hide This Option | Fungus.BooleanData | If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.

# Menu Shuffle # {#MenuShuffle}
Shuffle the order of the items in a Fungus Menu

Defined in Fungus.MenuShuffle

Property | Type | Description
 --- | --- | ---
Shuffle Mode | Fungus.MenuShuffle+Mode | Determines if the order is shuffled everytime this command is it (Every) or if it is consistent when returned to but random (Once)

# Menu Timer # {#MenuTimer}
Displays a timer bar and executes a target block if the player fails to select a menu option in time.

Defined in Fungus.MenuTimer

Property | Type | Description
 --- | --- | ---
_duration | Fungus.FloatData | Length of time to display the timer for
Target Block | Fungus.Block | Block to execute when the timer expires

# Portrait # {#Portrait}
Controls a character portrait.

Defined in Fungus.Portrait

Property | Type | Description
 --- | --- | ---
Stage | Fungus.Stage | Stage to display portrait on
Character | Fungus.Character | Character to display
Replaced Character | Fungus.Character | Character to swap with
Portrait | UnityEngine.Sprite | Portrait to display
Offset | Fungus.PositionOffset | Move the portrait from/to this offset position
From Position | UnityEngine.RectTransform | Move the portrait from this position
To Position | UnityEngine.RectTransform | Move the portrait to this position
Facing | Fungus.FacingDirection | Direction character is facing
Use Default Settings | System.Boolean | Use Default Settings
Fade Duration | System.Single | Fade Duration
Move Duration | System.Single | Movement Duration
Shift Offset | UnityEngine.Vector2 | Shift Offset
Move | System.Boolean | Move portrait into new position
Shift Into Place | System.Boolean | Start from offset position
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command
Display | Fungus.DisplayType | Display type

# Say # {#Say}
Writes text in a dialog box.

Defined in Fungus.Say

Property | Type | Description
 --- | --- | ---
Description | System.String | Notes about this story text for other authors, localization, etc.
Character | Fungus.Character | Character that is speaking
Portrait | UnityEngine.Sprite | Portrait that represents speaking character
Voice Over Clip | UnityEngine.AudioClip | Voiceover audio to play when writing the text
Show Always | System.Boolean | Always show this Say text when the command is executed multiple times
Show Count | System.Int32 | Number of times to show this Say text when the command is executed multiple times
Extend Previous | System.Boolean | Type this text in the previous dialog box.
Fade When Done | System.Boolean | Fade out the dialog box when writing has finished and not waiting for input.
Wait For Click | System.Boolean | Wait for player to click before continuing.
Stop Voiceover | System.Boolean | Stop playing voiceover when text finishes writing.
Wait For V O | System.Boolean | Wait for the Voice Over to complete before continuing
Set Say Dialog | Fungus.SayDialog | Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.

# Set Language # {#SetLanguage}
Set the active language for the scene. A Localization object with a localization file must be present in the scene.

Defined in Fungus.SetLanguage

Property | Type | Description
 --- | --- | ---
_language Code | Fungus.StringData | Code of the language to set. e.g. ES, DE, JA

# Set Menu Dialog # {#SetMenuDialog}
Sets a custom menu dialog to use when displaying multiple choice menus

Defined in Fungus.SetMenuDialog

Property | Type | Description
 --- | --- | ---
Menu Dialog | Fungus.MenuDialog | The Menu Dialog to use for displaying menu buttons

# Set Say Dialog # {#SetSayDialog}
Sets a custom say dialog to use when displaying story text

Defined in Fungus.SetSayDialog

Property | Type | Description
 --- | --- | ---
Say Dialog | Fungus.SayDialog | The Say Dialog to use for displaying Say story text

