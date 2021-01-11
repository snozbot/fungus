These demos use the new Unity Input system avaiable via the package manager.
You'll also need to:
- Add the new Input System package via the PackageManager or manifest
- Change the input system being used in the Project Settings->Player settings.
- Add the Unity.InputSystem assembly to list of asmdef References in the Fungus asmdef.

Adding that should also set ENABLE_INPUT_SYSTEM in your projects hash defines, which will switch out behaviour in a number of Fungus systems and commands, and events.