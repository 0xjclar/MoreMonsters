# MoreMonsters
Adds mob quantity scaling for the MoreCompany mod (found here: https://thunderstore.io/c/lethal-company/p/notnotnotswipez/MoreCompany/).

Players access a menu via the Insert key that allows them to customize how many of each monster spawns inside the building and how often a monster will spawn.
(Previously it was set to spawn 0.75 * number of players for anyone wishing to recreate the mob spawning quantity from the previous version.)

The number of mobs you set in the GUI is not affected by the number of mobs the game naturally spawns, i.e. the game may still spawn a 4th Jester if you have Jester set to 3, since the mod 
operates independently of the game's natural spawning.

Mobs will first start to spawn shortly after players drop.

# How to Use
Once in-game, the host opens the menu by pressing the Insert Key
The Number of Mobs slider changes the maximum number of monsters allowed inside the building from either 0 to 50 (This can lag quite badly after > 30 monsters total, especially if you are far away from the building.
Although this may only affect the host's performance).
The Time Between Mob Spawns slider adjusts the amount of time between each new individual monster spawn, From 0 to a max of 8 hours. 0 results in instantaneous spawning of the selected number of monsters.
When "Spawn an extra mob after..." is toggled, it will spawn a new monster after finding 25%, 50%, and 75% of the total scrap contained in the level, for a total of 3 additional monsters. Is is toggled off by default.

# Future Updates
Modifying the number of outside and daytime monsters as well.
Add more customization with regards to having more mobs spawn at various checkpoints, like with collecting
a certain amount of scrap or even reaching a various part of the dungeon.

# Installation
1. Install BepInEx.
2. Run game once with BepInEx installed to generate folders/files.
3. Drop the DLL inside of the BepInEx/plugins folder.
4. No further steps needed.

# Credits
Menu was possible thanks to @lawrencea13's code, creator of the Lethal Company Game Master Mod.
