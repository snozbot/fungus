
The Game Component
===================

The [Game](@ref Fungus.Game) class is the main controller component for every Fungus game. 
- It is a singleton class, so there can be only one instance of it in any Unity scene. 
- It can be accessed anywhere in your code using [Game.GetInstance()](@ref Fungus.Game.GetInstance).

The main responsibility of the [Game](@ref Fungus.Game) class is to keep track of the current game state. This includes:
- Currently active Room object
- Currently active Page object
- Currently active PageStyle object
- Gameplay state values (e.g. inventory items)

This class also manages global configuration parameters such as text writing speed, transition time between rooms, etc.

- - -

# Where can I find a full list of supported commands?

- See the [GameController](@ref Fungus.GameController) class for a full list of available commands.

- - -

# How do I add a Game object to my scene?

1. Create an instance of Fungus/Prefabs/Game.prefab.
2. When you add a Room to the scene, set the [Game.activeRoom](@ref Fungus.Game.activeRoom) property to use it as the starting Room.

If no activeRoom is set then Fungus will start in an idle state.