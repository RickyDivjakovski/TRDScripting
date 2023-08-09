
![App Screenshot](https://i.postimg.cc/T2m87tVY/logo.png) 
# TRD Scripting 

An interpreter language for fivem server modding/mod installing, the aim is to be able to have mods with drag/drop capabillities.
Hopefully this creates a bright future for developers and server owners.

Im building a community and suppot on discord: [https://discord.gg/v83pBUH9mE](https://discord.gg/v83pBUH9mE)

## Documentation

The processor will set the working directory to the directory the script is in, if it cannot find a server.cfg in the current directory it will search 
the root directory of the directory the script is in and set the working directory to it, if no config found it will recursively search the script directory 
for a server.cfg, if found the working directory will be set to that. if not found, the script will not run.

Download prebuilt versiosn from - [Prebuilt](https://github.com/RickyDivjakovski/TRDScripting/tree/main/Prebuilt)

Place a .tscr file in your project folder, you can run a command prompt and execute "TRDScriptProcessor.exe MyTscriptFile.tscr" to run it

Examples of working/not working.
Imagine this is the path of your server.cfg  -  C:\users\projects\testProject\server.cfg

C:\users\projects\testProject1\script1.tscr                  | This will work      working directory will be set to C:\users\projects\testProject
C:\users\projects\testProject1\folder1\script2.tscr          | This will work      working directory will be set to C:\users\projects\testProject
C:\users\projects\testProject1\folder1\folder2\script3.tscr  | This will NOT work  
C:\users\projects\script4.tscr                               | This will work, but will search   C:\users\projects for a server.cfg and set the working directory to the first

The script file extension must be a .tscr
Lines beginning with # are ignored
To escape a character use ^ before it, you will only need to escape commas(,), open brackets(() and closed brackets())
## Usage/Examples

A sample script file can be downloaded from [here](https://github.com/RickyDivjakovski/TRDScripting/blob/main/TRDScriptSample.txt)

```csharp
Scripting: 	
		ModifyFile expects 5 args
		ModifyFile(File, [ReplaceLine, ReplaceText, ConvertEOL], [LineStart, LineEnd, Containing], StringToFind, StringToReplaceWith)

		ImportSql expects 1 arg
		ImportSql(SqlFile)
		
		MoveFile expects 2 args
		MoveFile(InputFile, OutputFile)
		
		MoveFoler expects 2 args
		MoveFolder(InputDirectory, OutputDirectory)
		
		CreateFile expects 1 arg
		CreateFile(FileToCreate)
		
		CreateFolder expects 1 arg
		CreateFolder(FolderToCreate)
		
		ExtractZip expects 2 args
		ExtractZip(FileToExtract, OutputFolder)
```

This may not seem like much, but its a start for automated modification of fivem servers.
