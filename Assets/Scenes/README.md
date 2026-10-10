# Scene setup

Create a Unity 3D scene named `PedaLi`.

1. Create a Player GameObject and add a CharacterController.
2. Add a SpriteRenderer (or a child GameObject with SpriteRenderer) and assign a pixel-art sprite.
3. Add an Animator and assign a controller configured using `Assets/Animation/PedaLiAnimatorSetup.txt`.
4. Add `PedaLiController` to the Player. Assign the Animator and SpriteRenderer if they are not found automatically.
5. Create a Canvas in Screen Space - Overlay. Add four UI Images/Buttons for Left, Right, Grab, Jump.
6. Add `PedaLiMobileButton` to each UI element, assign the Player controller, and select the matching action.
7. Add an EventSystem to the scene if Unity has not created one automatically.
8. For an initial test, set Require Ledge To Grab to false. Add ledge colliders and a LedgeCheck transform later.
9. Put the camera in front of the sprite and use a 3D background behind the player.
10. Add `PedaLiVerticalCamera` to the Main Camera and drag the Player Transform into its Target field.
11. Set Target Offset to place the camera behind the player (for example, X = 0, Y = 2, Z = -10). Adjust Smooth Time to tune how softly it follows.
12. To add parallax, create separate scenery layer GameObjects (for example, distant mountains, midground trees, and foreground leaves) and assign them to the Parallax Layers array. Start with Follow Factor around 0.1–0.2 for distant scenery and 0.35–0.6 for nearer scenery. A factor of 0 leaves a layer fixed in world space.
13. For a quick procedural backdrop, create an empty GameObject named `NatureBackdrop` behind the player and add `PedaLiNatureParallaxBuilder`. Press Play to generate the sky, two mountain ridges, forest silhouettes, and clouds. Adjust World Width/Height and the color constants in the script to fit your scene.
14. Expand `NatureBackdrop` and drag `Parallax_FarMountains_Follow10`, `Parallax_NearMountains_Follow22`, `Parallax_Forest_Follow42`, and `Parallax_Clouds_Follow14` into the camera's Parallax Layers array. Set their Follow Factor values to 0.10, 0.22, 0.42, and 0.14 respectively; enable Ignore Depth for each layer. Keep the generated sky out of the array so it stays fixed.
15. Keep scenery layers outside the Player hierarchy. If the camera reaches the end of a level, adjust Vertical Limits; the defaults allow a tall, unrestricted climb.

This is a source starter, not a complete playable scene or built APK yet.
