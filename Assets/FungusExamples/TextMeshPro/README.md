In this scene we have a SayDialog and a MenuDialog from their default prefabs.
We have then replaced the UI Text elements used for story text, character text and the text items in the menu options with TextMeshPro Text UI elements.
The only other addtional change required is to set the Name Text GO and the Story Text GO in the SayDialog component on the SayDialog GameObject. These will show as bold be default in this scene as they differ from the prefab.
A similar approach can be used in any of your custom SayDialogs or menus is your projects.
Alternatively you may wish to update the prefabs on the defaults if that is all your project requires to exist.

As of 12-2018 this is done via the TextAdapter as it includes the previous functionality of fungus that attempts to find a component on the target GO with a 'text' property, which TMPro components have. In the future there are some more advanced features of TMPro that could be taken advantage of if the presence of TMPro is forced and more deeply integrated, such as into the Writer to use TMPro's revealed characters feature.