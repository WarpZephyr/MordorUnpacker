# MordorUnpacker
An unpacker for files in the Shadow of Mordor and Shadow of War games.  

# Requirements
Make sure the .NET 8.0 Runtime is installed or the program cannot run:  
https://dotnet.microsoft.com/en-us/download/dotnet/8.0  

Select ".NET Runtime"  
Most users will need the x64 installer.  
This program has only been tested on Windows x64.  

# Usage
This tool does not have a GUI.  
Instead a user drags and drops files it supports into MordorUnpacker.exe.  

Shadow of War makes use of Oodle compression.  
In order for this tool to unpack it's files correctly;  
Users must copy and paste the oodle library from their game into the MordorUnpacker folder.  
On windows for Shadow of War, this file will be oo2core_5_win64.dll, right next to the game's exe file.

# Troubleshooting
Q: The tool immediately closes and does nothing?  
A: See Requirements; This is generally caused by a lack of .NET.  

Q: I drag and drop into the window but nothing happens?  
A: See Usage; Users must drag and drop into the program exe file, not the window it opens.  

Q: The tool is saying something about needing oodle?  
A: See Usage; Copy one of the files it specifies from any game that has them into the MordorUnpacker folder;  
   If you have Shadow of War, the oodle dll is next to the exe file of the game.  

Q: I have an issue but don't see my problem listed, or the tool threw an error?  
A: Make an issue report and I might get to it at some point.  

# Building
Clone or download this project somewhere:  
```
git clone https://github.com/WarpZephyr/MordorUnpacker.git  
```

This project requires the following libraries to be cloned alongside it.  
Place them in the same top-level folder as this project.  
These dependencies may change at any time.  
```
git clone https://github.com/WarpZephyr/MordorFormats.git  
git clone https://github.com/WarpZephyr/OodleCoreSharp.git  
git clone https://github.com/WarpZephyr/Edoke.git  
```

Then build the project in Visual Studio 2022.  
Other IDEs or build solutions are untested.  