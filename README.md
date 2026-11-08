# Buried Echoes

Buried Echoes is a first-person digging and exploration game prototype developed in **Unity 6** with **C#** and the **Universal Render Pipeline (URP)** for the *Advanced Programming with C#* course.

The player digs through destructible voxel terrain, collects underground ores, sells them at a surface station, and purchases upgrades to dig deeper. This is a course-project prototype, not a finished game.

## Features

- Smooth, destructible voxel terrain generated using Marching Cubes.
- Shovel and drill tools with different digging characteristics, selected through the Strategy design pattern.
- Collectible iron, copper, silver, and gold, including larger variants.
- Limited-capacity inventory, selling station, and money system.
- Shovel, inventory, and rope-ascender upgrades.
- Mining shed, flashlight, placeable lamps, and depth-dependent ambient audio.
- Unity Edit Mode unit tests for core gameplay logic.

## Requirements

- **Unity 6** with URP support. Use the Unity Editor version recorded in `ProjectSettings/ProjectVersion.txt` for the closest match.
- Unity Hub to open the project.
- A Windows PC for building the Windows standalone version.

The repository contains the Unity project's package manifest in `Packages/manifest.json`. Unity will restore required packages when the project is opened.

## Open and run in Unity

1. Open **Unity Hub** and choose **Add project from disk**.
2. Select the **BuriedEchoes** folder (the one containing `Assets`, `Packages`, and `ProjectSettings`).
3. Open the project with the matching Unity Editor version. Allow Unity to import assets and compile scripts.
4. Open the scene `BuriedEchoes_Main` from the project's `Assets` folder (locate it using the Project window search if needed).
5. Click **Play** in the Unity Editor.

## Build a Windows version

1. Open the project in Unity and load `BuriedEchoes_Main`.
2. Go to **File > Build Profiles**.
3. Select or add the **Windows** build profile, and make sure `BuriedEchoes_Main` is included in the scenes to build.
4. Click **Build** and choose an output directory **outside** the Unity project's `Assets` folder.
5. When the build finishes, run the generated `.exe` file. Keep its accompanying data folder and other build files together.

## Controls

| Input | Action |
| --- | --- |
| WASD | Move |
| Mouse | Look around |
| Left Shift | Run |
| Space | Jump |
| Left mouse button (hold) | Dig continuously |
| 1 | Equip shovel |
| 2 | Equip drill |
| E | Collect ore / use selling or upgrade station |
| T (hold) | Ascend using rope |
| F | Place lamp |
| R | Toggle flashlight |
| Escape | Close the upgrade menu |
| M | Add $10,000 (development/testing shortcut) |

The **M** shortcut is intentionally retained in this prototype to make it easier to demonstrate and test upgrades.

## Run unit tests

1. Open the project in Unity.
2. Go to **Window > General > Test Runner**.
3. Select the **EditMode** tab.
4. Click **Run All**.

Tests cover core gameplay logic such as inventory operations, resource selling, player wallet, digging tool statistics, and digging strategies. All Edit Mode tests passed in the final development review.

## Implementation notes

The voxel terrain is based on a modified **Editable Smooth Voxel Terrain** implementation. It uses Marching Cubes and chunk-based mesh updates. Optimization work includes reuse of collections and normal arrays, local grid-cell updates following digs, and rebuilding affected neighboring chunks only when needed.

The core inventory, wallet, seller, and tool-statistics classes are separated from Unity scene interactions to support testing. `IDiggingStrategy`, `ShovelStrategy`, and `DrillStrategy` provide interchangeable digging configurations.

## Known limitations

- Gameplay progress is not saved between sessions.
- Small isolated pieces of dirt can remain floating after digging.
- Digging can still produce occasional frame-time spikes due to mesh and collider updates.
- The rope ascender is a movement mechanic, not a simulated physical rope.
- The planned deep underground secret cave is not implemented in this prototype.

## Third-party assets and acknowledgements

This project uses a modified third-party smooth voxel terrain implementation and imported art/audio assets. Consult the original packages and accompanying license files for author attribution and applicable reuse terms. Preserve any required license and notice files when distributing the project.
