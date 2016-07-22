MoonSharp [http://www.moonsharp.org]
------------------------------------

This archive contains all the files required to setup MoonSharp on your machine.

Contents:

 - /interpreter    -> The main DLL of the MoonSharp interpreter itself. 
                      Use this if you want to just embed the interpreter in your application.
                      
 - /remotedebugger -> The DLL for the remote debugger facilities (plus the interpreter DLL itself). 
                      Use this if you want to embed the intepreter in your application with remote debugging enabled.
                      
 - /repl           -> The REPL interpreter. It's not really meant for production as much as to quickly test scripts,
                      or to compile bytecode.
                      
Each directory contains, where applyable, subdirectories for different .NET framework targets:


- net35 : 
This is build targeting .NET 3.5 Client Framework. 
Use this if you are building an app targeting .NET 3.5 or later, Mono 2.x (or later), Xamarin or Unity 3D.

- net40 : 
This is build targeting .NET 4.0 Client Framework. 
Use this if you are building an app targeting .NET 4.0 or later, Mono 3.x or Xamarin.

- portable_net40 : 
This is a Portable Class Library targeting .NET 4.0, Silverlight 5, Xamarin Android, Xamarin iOS, Windows Store 8, Windows Phone 8.1 
Use this if you target these platforms. Note that some functionality (involving file system access or the remote debugger) is not available 
in this build due to limitations of PCLs.
You also have to use this library if you target WSA/WP8 apps in Unity3D. Refer to this guide: http://docs.unity3d.com/Manual/windowsstore-plugins.html


 


 
 


