# PedaLi

PedaLi is a Unity Android climbing-game prototype: a pixel-art character climbs upward through a colorful 3D world inspired by vertical platform climbers.

## Current starter files

- `Assets/Scripts/PedaLiController.cs` — movement, jump, ledge-grab input, and animation-state selection. Pixel animations now update even if a Mecanim Animator is not assigned.
- `Assets/Scripts/PedaLiSpriteAnimator.cs` — frame-based pixel-art animation for Idle, Run, Jump, Grab, Climb, and Fall.
- `Assets/Scripts/PedaLiVerticalCamera.cs` — smooth vertical camera follow, look-ahead, and configurable scenery parallax. It can auto-find the player target.
- `Assets/Scripts/PedaLiNatureParallaxBuilder.cs` — procedural nature backdrop with jungle, mountain, and snow-peak palettes plus height-based biome switching.
- `Assets/Scripts/PedaLiMobileButton.cs` — touch-button handling for left, right, grab, and jump.
- `Assets/Scripts/PedaLiMainMenu.cs` — auto-built portrait menu with biome selection, settings, drifting clouds, floating leaves, and a tiny pixel climber.
- `Assets/Animation/PedaLiAnimatorSetup.txt` — sprite slicing, import settings, and animation setup.
- `Assets/Scenes/README.md` — steps to assemble a first test scene.
- `BUILD_ANDROID.md` — what is needed for a remote Android APK build without installing Unity locally.

## Current limitations

This repository is still a starter-code repository, not yet a complete, verified Unity project. It does not currently include a configured gameplay scene, all imported art assets, Android build settings, or a verified APK. The menu, controller, camera, and backdrop scripts have been edited in GitHub, but have not been compiled or run in Unity in this session.

The ledge-grab system needs a ledge-check transform and colliders on the selected layer. For a quick movement test, disable **Require Ledge To Grab** on the controller. The pixel animator needs actual sliced sprite frames assigned to its animation clips to show frame-by-frame character animation.

## Visual direction

Bright, colorful 3D nature backgrounds with parallax depth; a crisp pixel-art character with point-filtered sprites; vertical progression and mobile touch controls.
