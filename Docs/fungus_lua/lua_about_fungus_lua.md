# About FungusLua # {#lua_about_funguslua}
[TOC]

FungusLua is a simple way to embed Lua scripting into your Unity project. Lua is an easy to learn scripting language so it's a great way to empower artists, writers and designers to use more of the power of Unity.

At its core, FungusLua allows you to control any Unity object from Lua script. It has useful utilities for using %Fungus flowcharts and dialogs, persisting variables between scene loads, localization, and working with the Unity Test Tools. 

We made FungusLua in response to requests from the %Fungus community for a way to script %Fungus commands from a text file or spreadsheet. We figured that if people are going to be writing commands in text files, why not go all the way and add a powerful embedded scripting language?

FungusLua comes as part of the [%Fungus asset] available on the Unity Asset Store.

# Platform Compatibility # {#lua_platform_compatibility}

%FungusLua should work on most platforms supported by Unity. We list any known platform compatibility issues here, please let us know if you find more. 

- Windows Store (.NET scripting backend): Both %Fungus & %FungusLua compile ok, but generate runtime errors when executing Lua scripts. The IL2CPP scripting backend compiles and runs fine.

# Tutorial Video # {#lua_tutorial_video}

This video shows how to use many of the features available in FungusLua. It's more of a demonstration than a step-by-step tutorial, but hopefully between this video, the docs and the forums you'll have enough to figure it all out :)

@htmlonly
<div align="center">
<iframe width="560" height="315" src="https://www.youtube.com/embed/M_Oo9FpVTos" frameborder="0" allowfullscreen></iframe>
</div>
@endhtmlonly

# FungusLua without Fungus # {#funguslua_without_fungus}

FungusLua can easily be used on its own if you don't need the rest of the functionality in %Fungus.

1. In the project window, move the %Fungus/Thirdparty/FungusLua folder up to the root of the project.
2. Delete the %Fungus and FungusExamples folders.
3. Add FUNGUSLUA_STANDALONE to the Scripting Define Symbols in Edit > Project Settings > Player

The Tools > %Fungus menu will now only show options for creating FungusLua objects. Obviously you won't be able to use %Fungus functions like say(), menu(), etc. anymore, but you can still use LuaEnvironment, LuaBindings, LuaScript to add Lua scripting to your game.

# About Lua # {#about_lua}

[Lua] is a powerful, fast, lightweight, embeddable scripting language. It is a popular language for game development and supporting user modding. The standard resource for learning Lua is [Programming in Lua].

![Lua logo]

# About MoonSharp # {#about_moonsharp}

[MoonSharp] is an open source implementation of the Lua scripting language written entirely in C#. 

![MoonSharp Logo]

FungusLua is essentially a set of wrapper components built on top of MoonSharp which make it easier to use Lua scripting directly in the Unity editor. MoonSharp does all the hard work really and is a completely awesome project :)

The [MoonSharp tutorials] and [MoonSharp forum] are great resources to learn how MoonSharp works, especially for more advanced usage.

[Lua]: http://www.lua.org/about.html
[%Fungus asset]: http://u3d.as/f0T
[Programming in Lua]: http://www.lua.org/pil/1.html
[MoonSharp]: http://www.moonsharp.org
[MoonSharp tutorials]: http://www.moonsharp.org/getting_started.html
[MoonSharp forum]: https://groups.google.com/forum/#!forum/moonsharp

[Lua logo]: ./fungus_lua/lua.png
[MoonSharp Logo]: ./fungus_lua/moonsharp.png