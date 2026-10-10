# PedaLi

PedaLi is a Unity Android climbing-game prototype: a pixel-art character climbs upward through a colorful 3D world inspired by vertical platform climbers.

## Current starter files

- `Assets/Scripts/PedaLiController.cs` — movement, jump, ledge-grab input, and animation-state selection.
- `Assets/Scripts/PedaLiSpriteAnimator.cs` — frame-based pixel-art animation for Idle, Run, Jump, Grab, Climb, and Fall.
- `Assets/Scripts/PedaLiVerticalCamera.cs` — smooth vertical camera follow with configurable scenery parallax layers.
- `Assets/Scripts/PedaLiNatureParallaxBuilder.cs` — procedural nature backdrop with jungle, mountain, and snow-peak palettes plus height-based biome switching.
- `Assets/Scripts/PedaLiMobileButton.cs` — touch-button handling for left, right, grab, and jump.
- `Assets/Scripts/PedaLiMainMenu.cs` — auto-built portrait main menu with illustrated biome backgrounds, Start Climbing, biome selection, and settings.
- `Assets/Animation/PedaLiAnimatorSetup.txt` — sprite slicing, import settings, and animation setup.
- `Assets/Scenes/README.md` — steps to assemble a first test scene.

## Getting started

1. Install a Unity LTS version with **Android Build Support**, Android SDK & NDK Tools, and OpenJDK.
2. Create/open a Unity 3D project.
3. Copy the `Assets` folder from this repository into the Unity project.
4. Create a scene and follow `Assets/Scenes/README.md`. The procedural main menu appears automatically in scenes that do not contain a `PedaLiController`.
5. Add your gameplay scene to **Build Settings / Build Profiles**. The menu's Start Climbing button opens the first non-menu scene included in the build.
6. Assign your own pixel-art sprite frames and configure the Animator as described in `Assets/Animation/PedaLiAnimatorSetup.txt`.
7. Test in the Unity Editor first, then switch the build platform to Android and build an APK.

## Important

This repository currently contains starter scripts and setup notes, not a complete Unity project, finished art assets, or a verified APK. The ledge-grab system needs a ledge-check transform and colliders on the selected layer. For a quick movement test, disable **Require Ledge To Grab** on the controller.

## Visual direction

Bright, colorful, realistic 3D nature backgrounds with parallax depth; a crisp pixel-art character with point-filtered sprites; vertical progression and mobile touch controls.

