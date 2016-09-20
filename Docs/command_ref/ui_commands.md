# UI commands {#ui_commands}

## Fade UI
Fades a UI object

Property | Type | Description
 --- | --- | ---
Target Objects | System.Collections.Generic.List`1[UnityEngine.GameObject] | List of objects to be affected by the tween
Tween Type | LeanTweenType | Type of tween easing to apply
Wait Until Finished | Fungus.Variables.BooleanData | Wait until this command completes before continuing execution
Duration | Fungus.Variables.FloatData | Time for the tween to complete

## Get Text
Gets the text property from a UI Text object and stores it in a string variable.

Property | Type | Description
 --- | --- | ---
Target Text Object | UnityEngine.GameObject | Text object to get text value from
String Variable | Fungus.Variables.StringVariable | String variable to store the text value in

## Set Interactable
Set the interactable sate of selectable objects.

Property | Type | Description
 --- | --- | ---
Target Objects | System.Collections.Generic.List`1[UnityEngine.GameObject] | List of objects to be affected by the command
Interactable State | Fungus.Variables.BooleanData | Controls if the selectable UI object be interactable or not

## Set Slider Value
Sets the value property of a slider object

Property | Type | Description
 --- | --- | ---
Slider | UnityEngine.UI.Slider | Target slider object to set the value on
Value | Fungus.Variables.FloatData | Float value to set the slider value to.

## Set Text
Sets the text property on a UI Text object and/or an Input Field object.

Property | Type | Description
 --- | --- | ---
Target Text Object | UnityEngine.GameObject | Text object to set text on. Can be a UI Text, Text Field or Text Mesh object.
Text | Fungus.Variables.StringDataMulti | String value to assign to the text object
Description | System.String | Notes about this story text for other authors, localization, etc.

## Write
Writes content to a UI Text or Text Mesh object.

Property | Type | Description
 --- | --- | ---
Text Object | UnityEngine.GameObject | Text object to set text on. Text, Input Field and Text Mesh objects are supported.
Text | Fungus.Variables.StringDataMulti | String value to assign to the text object
Description | System.String | Notes about this story text for other authors, localization, etc.
Clear Text | System.Boolean | Clear existing text before writing new text
Wait Until Finished | System.Boolean | Wait until this command finishes before executing the next command

