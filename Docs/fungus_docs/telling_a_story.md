# Telling a story {#telling_a_story}

Having got up and running, here are some next steps to get you familiar with the storytelling features in Fungus.

<!-- **************************************** -->
## Adding Characters, for use in Say commands

We can associated words spoken by the Say Command with a particular Character. Consider the following Tom and Jerry scene:

```
[Tom] Where is that mouse?
[Jerry] Where is that cat?
[Tom] Aha...
[Jerry] Arrrrggggggg!!!!!!!
```

To implement the above in Fungus we need to create and name two Characters. Do the following:

1. (setup) Create a new scene, add a Fungus Flowchart to the scene, and select the Block in the Flowchart.

1. Rename the Flowchart Block "cat and mouse".

1. Choose menu:
<br>```Tools | Fungus | Create | Character```:
<br>
![Menu create character](./images/001_characters/1_menu_character.png "Menu create character")
<br>
<br>

1. You should now see a new gameObject "Character" in the Hierarchy window, named Character.
<br>
![new character](./images/001_characters/2_new_character.png "new character")
<br>
<br>

1. Ensure gameObject "Character" is selected, and edit its properties in the Inspector. Rename the gameObject to "Character1 - Tom", then in its Character (Script) component set the Name Text to "Tom" and the Name Color to red:
<br>
![character tom red](./images/001_characters/3_tom.png "character tom red")
<br>
<br>

1. Repeat the previous two steps to create a second character "Character2 - Jerry", then in its Character (Script) component set the Name Text to "Jerry" and the Name Color to blue:
<br>
![character jerry blue](./images/001_characters/4_jerry.png "character jerry blue")
<br>
<br>

1. Now we have our two character gameObjects, we can assign them to any Say commands as appropriate.

1. Create a Say Command for Tom, with text "Where is that mouse?", setting the Character of this Say command to "Character1 - Tom":
<br>
![tom say where cat](./images/001_characters/5_tom_say1.png "tom say where cat")
<br>
<br>

1. Repeat the above step for the 3 remaining statements, for:
    - Jerry "Where is that cat?"
    - Tom "Aha..."
    - Jerry "Arrrrggggggg!!!!!!!"
<br>Assigning the appropriate Character for each Say Command from the menu of Character gameObjects in the Hiearchy.

1. You should now have a sequence of 4 Say commands in your Block:
<br>
![tom jerry conversation](./images/001_characters/6_four_says.png "tom jerry conversation")
<br>
<br>

1. When you run the scene you should see a sequence of statements, clearly showing who is saying what - both the character name is given, and also that name is coloured according to the properties we set for the character gameObjects (red for Tom, and blue for Jerry):
<br>
![tom jerry conversation output](./images/001_characters/7_tom_jerry_chat.png "tom jerry conversation output")
<br>
<br>

<!-- **************************************** -->
## Listing portrait image(s) for use by Characters

If you add one or more portrait images to a character, then each Say command for that character can define which of those portrait images should be displayed, alongside the (colored) name of the Character.

To add portrait images to a character do the following:

1. (setup) Create a new scene, add a Fungus Flowchart to the scene, and select the Block in the Flowchart.

1. Rename the Flowchart Block "The case of the missing violin".

1. Create a character, and in the Inspector give your character Name Text (we chose "Sherlock") and a name color.

1. Now in the Inspector click the Add Portrait button (the plus-sign "+"), to get a 'slot' into which to add a portrait image:
<br>
![add portrait](./images/002_portrait/2_add_portrait.png "add portrait")
<br>
<br>

1. Drag the appropriate image into your new portrait image slot (in this screenshot we used the 'condident' image from the Sherlock example project). Also set the direction that the image is facing (left / front / right):
<br>
![sherlock image](./images/002_portrait/1_sherlock.png "sherlock image")
<br>
<br>

1. Create a second character (e.g. John, using Name Color blue, and portrait image 'annoyed').

1. Now select your Block in the Fungus Flowchart, so you can add some Commands to be executed...

1. Create a Say command, for your Sherlock Character, saying "Watson, have you seen my violin?" and choosing portrait 'confident' (since this is the only we added to the Character):
<br>
![sherlock Say command](./images/002_portrait/6_say_sherlock.png "sherlock Say command")
<br>
<br>

1. Add a second Say command, this time for Character John, saying "No, why don't you find it yourself using your amazing powers of deduction..." and choosing the 'annoyed' portrait for John.
<br>
![2 say commands](./images/002_portrait/8_two_say_commands.png "2 say commands")
<br>
<br>


1. When you run the scene you should see a sequence of statements, clearly showing who is saying both with (colored) name text AND also the portrait image you selected for each Say command:
<br>
![sherlock output](./images/002_portrait/4_sherlock_output.png "sherlock output")
<br>
![john output](./images/002_portrait/5_john_output.png "john output")
<br>

As you can see in some of the Fungus Example projects, many games will have a wide range of different portrait images for each character, to allow a full range of visual expression of emotion to support the text of Say commands:
<br>
![sherlock image list](./images/002_portrait/7_many_portait_images.png "sherlock image list")
<br>
<br>

<!-- **************************************** -->
## Add a Stage

Portrait images can be used in two ways in Fungus.

- They can be shown as part of the **Say** commands in the Say Dialog.
- Alternatively Portraits can be displayed and moved around the screen inside Fungus **Stages**, using the Portrait Command.

Create a simple stage that covers the whole game Window as follows:

1. (setup) Create a new scene, add a Fungus Flowchart to the scene, and select the Block in the Flowchart.

1. Rename the Flowchart Block "stage demo".

1. Create a Fungus Stage gameObject in the scene by choosing menu:
<br>```Tools | Fungus | Create | Stage```:
<br>
![menu add stage](./images/003_stage/1_menu_stage.png "menu add stage")
<br>
<br>

1. You should now see a new gameObject "Stage" added to the scene Hierarchy.

1. If you select it you will see its properties in the Inspector. We can leave the default settings, since these are for the stage to cover the whole Game window. There are some child gameObjects inside the Stage, but you don't need to worry about these unless you are doing some advanced customisation of stages for a particular game effect.
<br>
![stage gameObject](./images/003_stage/2_stage_gameobject.png "stage gameObject")
<br>
<br>


Now you have added a Fungus Stage to your scene, you will be able to make large Portrait images appear / move in-out of the screen using the **Portrait** Command in Fungus Flowchart Blocks...


<!-- **************************************** -->
## Displaying Portrait images on stages with the Portrait command

Once you have a Fungus Stage, and a character then you can instruct Fungus to display / move onscreen the Character Portrait images. To make character images appear as part of a scene do the following:

1. (setup) Create a new scene, add a Fungus Flowchart to the scene, and select the Block in the Flowchart.

1. Rename the Flowchart Block "sherlock enters dramatically".

1. Create a Fungus Stage gameObject in the scene by choosing menu:
<br>```Tools | Fungus | Create | Stage```.

1. Create a new character, name the gameObject "Character1 - Sherlock", set the Name Text to "sherlock" and the Name Color to green. Add to this character a portrait (we used the sherlock-confident image from the Fungus Example project "Sherlock"). And set the image facing to the appropriate side (in our case: left):
<br>
![sherlock image](./images/002_portrait/1_sherlock.png "sherlock image")
<br>
<br>

1. Add a Portrait Command by clicking the Add Command button (the plus-sign "+"), then choosing menu:
<br>
```Narrative | Portrait```:
<br>
![add command portrait](./images/005_portrait_command/1_menu_command_portrait.png "add command portrait")
<br>
<br>

1. You will now see your new Portrait command in the top half of the Inspector, and its Command properties in the bottom half of the Inspector. Note the red exclamation mark at the right of the highlighted (green) Command row - this indicated when a command has one or more required properties that have not been set. We see the error message *"No character selected"*:
<br>
![new command portrait](./images/005_portrait_command/4_new_command_portrait.png "new command portrait")
<br>
<br>

1. Set the portrait's character to "Character1 - sherlock", and set the following properties:
    - Portrait: confident
    - Facing: <-- (left)
    - Move: Yes (check the checkbox)
    - From Position: Offscreen Right
    - To Position: Right
<br>
![command portrait for sherlock](./images/005_portrait_command/2_portrait_command.png "command portrait for sherlock")
<br>
<br>

1. When you run the scene, the Sherlock portrait image should move into view having started from Offscreen - Right. The image stops when it gets to about a third the way onto the screen:
<br>
![sherlock portrait output](./images/005_portrait_command/3_portrait_output.png "sherlock portrait output")
<br>
<br>

Note, a common Command flow sequence is:

- to have a character enter on screen (Portrait command),
- then have that character say something (Say command),
- then have another character enter the screen (Portrait command),
- and then that second character says something (Say command).

Here is just such a sequence for the "Case of the missing violin" two-sentence scenario explored in the recipe to learning how to create Fungus Characters (recipe: Listing portrait image(s) for use by Characters):<br>
![sherlock portrait output](./images/005_portrait_command/3_portrait_output.png "sherlock portrait output")
<br>
<br>

Here we see the Play Mode user experience of the output of running such a workflow:
<br>
![sherlock portrait then say](./images/005_portrait_command/5_sherlock_say.png "sherlock portrait then say")
<br>
<br>
![john portrait then say](./images/005_portrait_command/6_john_say.png "john portrait then say")
<br>

We can also see that the character that is Say'ing something, has a WHITE-outline around its Stage portrait, to visually reinforce to the user which character is speaking at any point in time...

<!-- **************************************** -->
## Play some music
Music sound clips loop, so they are restarted once they have finished playing. Often the first Command in a Block is a **Play Music** Command. Add music to a Block as follows:

1. (setup) Create a new scene, add a Fungus Flowchart to the scene, and select the Block in the Flowchart.

1. Add a Play Music Command to the current Block by clicking the Add Command (plus-sign "+" button) in the Inspector, and then choosing menu: ```Audio | Play Music```.

1. Ensure the Play Music command is selected (green highlight) in the top of the Inspector, and then drag the desired music clip file into the "Music Clip" property in the bottom half of the Inspector:
<br>
![Add Play Music command](./images/004_play_music/1_add_playmusic_command.png "Add Play Music command")
<br>
<br>

1. Change the volume as desired
<br>(the default is 1, values are between 0.0 and 1.0, representing percentages of volume from 0% - 100%).

1. Play your scene - the music clip should play, and keep looping.

NOTE: If you wish to start playing the music clip from a known time-point (rather than from the beginning), then enter the desired time-point in the Inspector property "At Time" for your Play Music command.

<!-- **************************************** -->
## Add menu commands to branch to other blocks
Let's use a Say command above to ask a tricky mathematical question, and demonstrate the Menu command by offering the user a choice been "correct' and "incorrect" answers.  Menu commands transfer control to another block - so we'll need to add 2 new blocks to correspond to the 2 answers.
Do the following:

1. (setup) Create a new scene, add a Fungus Flowchart to the scene, and select the Block in the Flowchart.

1. Rename the Block in the Flowchart to "Question".

1. Create a Say command, with **Story Text** to ask the question: "Is 2 + 2?".

2. Uncheck the "Wait For Click" checkbox (this is so we see the menu options immediately after the Say command has displayed the question):
<br>
![maths say command](./images/011_menu_maths/2_edited_say.png "maths say command")
<br>
<br>

3. Create a new Block, named "Correct" which contains a **Say** command with the text "Well done, you are very mathematical!". Click the plus-sign button in the Flowchart window to add a new Block to the Flowchart, rename it "Correct" and then add that Say command:
<br>
![correct block](./images/011_menu_maths/1_correct_block.png "correct block")
<br>
<br>

4. Select the "Question" block, and add a Menu command by clicking the plus-sign add Command button in the Inspector and then choosing menu: ```Narrative | Menu```.
<br>
![add menu command](./images/011_menu_maths/6_add_menu.png "add menu command")
<br>
<br>

5. With this new Menu command selected (green) in the top half of the Inspector window, set the **Text** to "Yes" and the **Target Block** to your new "Correct" block:
<br>
![menu command](./images/011_menu_maths/4_menu_correct.png "menu command")
<br>
<br>

6. You should now see how the 'flow' of commands can change from Block "hello" to Block "Correct" in the Flowchart window:
<br>
![flow between blocks in Flowchart](./images/011_menu_maths/5_connected_blocks.png "flow between blocks in Flowchart")
<br>
<br>

7. Add a second new Block named "Wrong", containing a Say command with text "Bad luck, perhaps consider a non-mathematical career path..."
<br>
![block for wrong answer](./images/011_menu_maths/7_wrong_block.png "block for wrong answer")
<br>
<br>

8. Now we need to add another Menu command to our "hello" block, offering the user the "No" answer to our maths question, and passing control to Block "Wrong" if they disagree that 2 + 2 = 4. Select the "hello" block, and add a Menu command. With this new Menu command selected (green) in the top half of the Inspector window, set the **Text** to "No" and the **Target Block** to your new "Wrong" block.

9. You should now see in the Flowchart window how block "hello" can pass control to either block "Correct" or Block "Wrong" - depending on which menu answer the user selects.
<br>
![block connected to 2 others](./images/011_menu_maths/8_three_block_menu.png "block connected to 2 others")
<br>
<br>

10. Run the scene, and you should see the Say question appear at the bottom of the screen, and also the two Menu buttons "Yes" and "No" in the middle of the screen. Clicking "Yes" then runs the "Correct" Block's commands, and clicking "No" runs the "Wrong" block's commands:
<br>
![menu running](./images/011_menu_maths/9_menu_running.png "menu running")
<br>
<br>

<br>
![correct screen](./images/011_menu_maths/10_correct.png "correct screen")
<br>
<br>


<br>
![wrong screen](./images/011_menu_maths/11_wrong.png "wrong screen")
<br>
<br>

<!-- **************************************** -->
## Change Camera background colour

Unity cameras determine what the user sees when a scene is running. When nothing is present in all or part of the camera's rectangle a solid "Background" colour is displayed. Unity cameras have a default Background of a medium dark blue colour. You can change this as follows:

1. (setup) Create a new 2D scene, unless you already have a scene with which to work.

1. Select the Main Camera in the Hierarchy.

1. In the Inspector for the Camera component, click and choose a different value for the Background property - often black works well.
<br>
![camera background colour](./images/06_camera_background/1_background_black.png "camera background colour")
<br>
<br>

1. Now when any part of the camera rectangle (frustrum) shows no gameOjects then your custom Background colour will be what the user sees.

<!-- **************************************** -->
## Add a background sprite

To add any sprite image file from your Unity Project folder into the current scene, simply drag a reference to the sprite image file from the Project window onto the Scene window, and rotate / resize desired. The sprite will appear as a new gameObject (with same name as Sprite Project image file) in the Hierarchy window:
<br>
![sprite into scene](./images/07_background_sprite/1_sprite.png "sprite into scene")
<br>

NOTE: You may not be able to see the sprite, because what we see depends on the current settings for the camera. What the camera shows, how it moves etc. can be controlled by Fungus Views and Commmands relating to Views.

<!-- **************************************** -->
## Adding and customising a view

What the main camera of a scene displays to the user, and how it moves etc. can be controlled by Fungus Views and Fungus Commmands relating to Views. A Fungus View is a special gameObject in the Hierarchy, it appears as a green outlined inner rectangle, with two filled green rectangles on the left and the right. The ratio of the outlined inner rectangle is 4:3. The ratio of the outer rectangle (which includes the two filled green left and right rectangles) is 16:9. These two ratios cover almost every common phone, tablet and computer screen width-to-height ratio. So arranging the view so that a background Sprite image looks good for both inner- and outer- rectangles of a view, pretty much ensures your game will look good on any device. Setting the background color of the camera to something like black also means on the rare device that has an odd ratio showing content outside of the view outer rectangle, the game should still look perfectly acceptable.

To add a view to the current scene do the following:

1. (setup) Create / Edit a scene that has a Sprite background image gameObject

1. Choose menu: ```Tools | Fungus | Create | View```:
<br>
![menu new view](./images/08_add_view/1_menu_view.png "menu new view")
<br>
<br>

1. Rename this View as "View1".

1. Use the two white squares to resize the view (it maintains its proportions). Use the center square outline, or vertical and horizontal arrows to move the View around the Scene window.
<br>
![move and resize handles](./images/08_add_view/4_move_resize_handles.png "move and resize handles")
<br>
<br>
![new view](./images/08_add_view/2_gameobjects.png "new view")
<br>
<br>

1. Ensure the View is selected in the Hierarchy, then position the view so that it is approximately centered on your background sprite image

1. Resize (and if necessary reposition) the View to be as big as possible, but ensuring that its outer rectangle stays within the bounds of the background sprite. (Note we've tinted the Sprite red so the green View rectangles can be more easily seen in this screenshot):
<br>
![resize view](./images/08_add_view/3_resize_view.png "resize view")
<br>
<br>

1. Note: You can also rotate the view with the Unity Rotate tool

NOTE: Utnil you add a "Fade To View" Fungus command, you still may not see the Sprite in the Game window when the scene plays, since the Main Camera has not been oriented to resize and align with the view.

<!-- **************************************** -->
## Add a Fade To View command

Once you have a Scene that contains some background Sprites and Fungus Views, you are ready to use the Fungus camera related Commands to control what the user sees. The simplest camera control is to make the Game window fade from a solid colour to the Main Camera being sized, positioned (and if necessary rotated) to show a specified Fungus View. Do the following:

1. (setup) Create / being editing a Scene containing a background Sprite image, and a Fungus View that has been positioned to show all / some of the Sprite.

1. In the Fungus Flowchart rename the Block "Camera Control".

1. Add a new "Fade to View" Command to the Block. First click the Plus button in the bottom half of the Inspector window, to add a new Command, then choose menu: ```Camera | Fade To View```:
<br>
![menu Fade to View](./images/08_add_view/5_menu_fade_to_view.png "menu Fade to View")
<br>
<br>

1. Now Drag "View1" from the Hierarchy window into the "Target View" property of the Fade to View Command  in the Inspector:
<br>
![assign Target View](./images/08_add_view/6_drag_view.png "assign Target View")
<br>
<br>

1. (We'll keep the defaults of 1 second and fade From Color of black).

1. When you run the Scene the Game window should start off solid black, and then slowly the background Sprite image within the View rectangle should fade into view.
1. Now Drag "View1" from the Hierarchy window into the
<br>
![menu Fade to View](./images/08_add_view/7_scene_running.png "menu Fade to View")
<br>
<br>

