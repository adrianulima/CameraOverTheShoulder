# Camera - Over The Shoulder

Camera - Over The Shoulder is a mod for [Space Engineers](https://www.spaceengineersgame.com/) that adds an additional camera option, inspired by the over-the-shoulder camera view commonly used in modern third-person shooters and action games. This camera mode places the viewpoint above the back of the player's shoulder, providing a dynamic and immersive experience similar to those seen in films and television.

![image](https://github.com/user-attachments/assets/71677d5b-8ccd-4682-a7e1-7f9f4d39363e)

## How to Install

1. Visit the [Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=2923698329) for Camera - Over The Shoulder and subscribe to the mod.
2. Launch Space Engineers and navigate to the save game settings.
3. Activate both the Camera - Over The Shoulder mod in your active mods list.

## Features

- **Camera Mode Toggle:** Press the "Camera Mode key" [V] to cycle between vanilla cameras and the Shoulder Camera. Press [F9] to go directly to the Shoulder Camera.
- **Third-Person Compatibility:** Works even if third-person or spectator cameras are disabled.
- **Collision Detection:** The camera checks for collisions with the game world, preventing clipping.
- **Zoom Levels:** Offers two different zoom levels for better control.
- **Controller-Friendly:** Fully compatible with controller setups.
- **Weapon and Tool Integration:** Works seamlessly with guns and tools equipped.
- **Versatile Display:** Supports all screen ratios and field of view (FOV) settings.

## Chat Config Commands

Customize your camera experience with the following chat commands:

- `/otscam hold <true/false>`: If set to false, the player does not need to hold the "Secondary Action key" [RightMouseButton] to zoom.
- `/otscam zoom <true/false>`: If set to false, zoom is disabled and the "Secondary Action key" [RightMouseButton] will have no effect.
- `/otscam enabled <true/false>`: If set to false, the Shoulder Camera is disabled. Useful for players who prefer not to use it, even if the server supports it.
- `/otscam collision <true/false>`: If set to false, the camera will not collide with the world. Available only locally or to server admins.
- `/otscam left <true/false>`: If set to true, the camera will be positioned on the left side, which might look unusual as the character uses the right shoulder to aim.
- `/otscam keybind <true/false>`: If set to false, the camera will not cycle using the "Camera Mode key" [V]. The Shoulder Camera can still be accessed with the [F9] key, or by pressing "Alt + V" as an alternative when in first-person view.

**Example Command:** `/otscam zoom true`

Configurations are saved to the mod settings folder and persist across world saves.

## Using It in Interiors

Unlike many games with shoulder cameras that are designed with specific level layouts to minimize camera collisions, Space Engineers provides a high degree of freedom in design. As a result, players may experience camera collisions in confined spaces. You can disable collision detection using the command above, but this might cause the camera to see through walls in certain scenarios—especially why this feature is disabled in multiplayer.

With practice, you can learn to manually adjust the camera position to avoid most collisions by moving it away from walls.

## Credits

- Developed by [Adriano Lima](https://github.com/adrianulima)
- Special thanks to the Space Engineers modding community for their continuous support and feedback.

> [!IMPORTANT]
> This mod is not affiliated with or endorsed by Keen Software House. It is a fan-made project developed independently for Space Engineers.

---

For any issues or suggestions, please contact me on [GitHub](https://github.com/adrianulima), [Steam](https://steamcommunity.com/id/adrianulima), or Discord (@adrianolima).
