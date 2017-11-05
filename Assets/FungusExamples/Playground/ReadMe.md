Playground
==========
Playground is a set of scenes that approximate some of the demos from https://github.com/UnityTechnologies/PlaygroundProject . They are simple sets of game like behaviour. They are not intended stand alone released or to played as games, they serve as demonstrations of how to configure and use physics driven behaviour in Unity3d with Fungus. It uses multiple flowcharts in one scene controlling individual objects, game resources, game rules and GUI. 


Common Structure
===============
All scenes make use of RigibBody2D, Collisions and Triggers. There is a FungusVariable to hold RigibBody2D references. Triggers and Collisions are EventHandlers, under the MonoBehaviour Event category. They also make use of other, MonoBehaviour EventHandlers and the newer Transform and Vector3 related Commands.

Movement and rotation are done via AddForce2D and AddTorque2D Commands based on input from the user. The drag factors on the RigibBody2D have been dialed in to give the desired feel.

Game interactions use SendMessage command to communicate specific interactions to the **GameRules** Flowchart. These messages are caught in its EventHandlers to alter score, lives, and the like. The GameRules also updates the UI as needed.


Defender
========
### Overview
Player rotates the turret in the centre with Left & Right arrow keys. Press space to launch a mushroom. Hit the falling squares before they reach the ground to win. If too many falling squares make it to the ground, it's game over.

### Elements
**Spawner** creates a random position within some defined limits at a specified interval and then uses the SpawnObject command.

**Cannon** has a block that uses an Input Axis as the input value to a AddTorque2D. Another block executes on KeyPress, it fetches the position and rotation of the cannon, spawns a prefab and gives it the fetched position and rotation. It also toggles a bool to prevent it from running faster than a defined fire rate.

**Ground** simply sends a message when an object tagged as Enemy Enters its trigger. This allows the **GameRules** to update lives remaining. This GameObject has a Box2D trigger on it.

**Enemy** is the prefab created by the Spawner, it has a RigibBody2D, a Box2D collider. Its flowchart spins itself and destroys itself when it Trigger Enters a Laser or the Ground.

Football
========
### Overview
A 1v1 football / air hockey setup. 1 player uses W,A,S,D to move, the other, the arrow keys. Get the red ball through your opponents goal to score a point.

### Elements
**<Name>Goal** Simply a Box2D Trigger that does a SendMessage command when the ball enters.

**P<Number>** Gathers input via GetAxis command, combines and scales them into a vector to be passed into an AddForce2D.

Lander
======
### Overview
Up arrow boosts the small yellow rocket mushroom. Left & Right arrow keys rotate it. The goal is to get the yellow rocket mushroom to the other white platform in the upper right, without going out of bounds or hitting the other parts of the environment.

### Elements
**Lander** has a RigibBody2D, it applies force and torque in an update event. It saves a position and rotation on game start so it can reset itself. It then has Collision Checks based on tag, one for death, one for success. Objects in the world are then named.
