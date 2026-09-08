# Map editor Windows setup for Zelda

This repository contains the WPF Zelda map editor and both script compiler
implementations. The current editor program is `WPFZ80MapEditorExe`; pass it a
Zelda repository directory to load the maps, images, and definitions.

## Prerequisites

- Visual Studio 2022 Community (or newer) with **.NET desktop development** and
  **Desktop development with C++**.
- .NET Framework 4.7.1 Developer Pack.
- The SPASM and Wabbitemu toolchain branches built and COM-registered first.
- A sibling or otherwise accessible Zelda checkout.

The solution's legacy NuGet assemblies are versioned in `packages`, so the
documented build does not depend on downloading packages from the network.

Clone the toolchain into sibling directories, because
`WPFZ80MapEditor.sln` references `..\spasm` and `..\wabbitemu`:

```powershell
Set-Location C:\Projects
git clone --branch sputt/zelda-toolchain https://github.com/sputt/spasm.git
git clone --branch sputt/zelda-toolchain https://github.com/sputt/wabbitemu.git
git clone --branch sputt/zelda-toolchain https://github.com/sputt/mapeditor.git
git clone --branch sputt/zelda-toolchain https://github.com/sputt/zelda.git Zelda
```

## Build

Restore packages and build the x64 release configuration:

```powershell
MSBuild .\WPFZ80MapEditor.sln /restore /p:Configuration=Release /p:Platform=x64
```

The executable is produced under
`WPFZ80MapEditorExe\bin\x64\Release\`. Build the solution from Visual Studio
instead if NuGet prompts for package recovery.

## Open Zelda

Pass the absolute Zelda repository path as the program's one argument:

```powershell
.\WPFZ80MapEditorExe\bin\x64\Release\WPFZ80MapEditorExe.exe C:\Projects\Zelda
```

The editor uses the Zelda folder to load map files, graphics, definitions, and
the script engine. It reads the calculator ROM path from Wabbitemu's registry
settings; configure Wabbitemu with a legally obtained TI-83 Plus ROM first.

`Script Compiler` contains the legacy native compiler, while `VB Zelda Script
Compiler` is the compiler referenced by the WPF editor. Both are retained in
source because existing Zelda scripts and editor workflows depend on them.
