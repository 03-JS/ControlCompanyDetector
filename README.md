# Control Company Detector

![banner](https://i.imgur.com/5UIW3kn.png)

A fully configurable mod that detects Control Company by giving you an in-game warning, marking lobbies that have it and detecting when the host spawns an enemy indoors. It can also hide or only show said lobbies.

# Features
## Manually spawned enemies detection

If the Detect enemy spawning option is enabled (which it is by default) and the host has spawned an enemy, the mod will give you a warning with the enemy's name.

Here's a full list of all the enemies it can detect:

- Spore lizard
- Hoarding bug / Loot bug
- Slime / Hygrodere
- Snare flea
- Spider
- Thumper
- Barber
- Bracken
- Butler
- Coil-Head
- Masked / Mimic
- Nutcracker
- Jester
- Lasso man
- Obunga

A couple of examples that show this feature in action:

![obunga](https://i.imgur.com/UD5NRRN.png)

![yippee](https://i.imgur.com/OAAM6h3.png)

### WARNING:
Mods that alter how indoor enemies spawn will cause this feature to not work properly

## Manually spawned enemies detection when hosting

Even though this mod's primary function is to detect and combat those who abuse Control Company in public lobbies, it can also be used as an "Anti-Cheat" of sorts when it comes to enemy spawning.

If you host a lobby with Detect enemy spawning as host enabled, a warning will appear to indicate that an enemy has been spawned by someone else. This means that enemies that have been spawned using cheats or other mods will be detected.

This feature will forcefully disable itself if it finds that you, the host, have a mod that changes how enemy spawning works installed and will give you the following information message:

![infoMessage](https://i.imgur.com/VLvUu2O.png)

The name of the mod at the end of the message will change depending on which mod caused the message to appear.

This message can be hidden under the **Hosting** section in the configuration file by setting the option **Show info message** to false

## Lobby filtering, marking & highlighting

Control Company Detector will mark lobbies that have Control Company with the prefix [CC] in the lobby list and highlight them. The prefix can be removed and the highlighting can be disabled.

You can also change how highlighted lobbies look. By default they will look like this:

![defaultHighlightColor](https://i.imgur.com/tdb4Sa6.png)

But their color can be changed to:

- A random color for each lobby (like in the example image below)
- Blue
- Green
- Lime
- Cyan
- Pink
- Purple
- Yellow
- Gray / Grey (accepts both)
- Maroon (the default color)

Here's an example using the random color option and no [CC] prefix:

![customHighlightColors](https://i.imgur.com/9amDCjt.png)

In addition to all of this, Control Company Detector allows you to show all lobbies, hide those that have Control Company or only show the ones with Control Company.

![lobbyFiltering](https://i.imgur.com/9EDGFPp.png)

## In-game warning

If Control Company Detector finds that the lobby host is using Control Company, an in-game warning will appear when you join the lobby:

![warning](https://i.imgur.com/DORSRhv.png)