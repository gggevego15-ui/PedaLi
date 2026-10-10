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

This is a source starter, not a complete playable scene or built APK yet.
