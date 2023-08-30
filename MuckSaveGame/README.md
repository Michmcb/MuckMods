# What It Does
This mod adds saving your game to Muck. It is basically, a fork and update of flarfo's fantastic [SaveUtility](https://muck.thunderstore.io/package/flarfo/SaveUtility) mod.
You CANNOT use this and SaveUtility at the same time. Remove SaveUtility and install this. The savefiles for SaveUtility will automatically be migrated to the new structure on startup.

Also, a few things more now save properly, which are:
- Mob spawnrate and weights are saved. This resets every night, but the fist night after reloading a save would have slower spawning mobs, and only weaker mobs were picked. This has been fixed.
- Boat and Gem markers on the map are saved.
- Boss rotation is saved. Muck has a pool of bosses to spawn that it picks from and empties as they spawn, then refills when the pool is empty. This is saved, so it doesn't reset to full on reload.
- If the Chief boss is currently spawned, he is saved (just like Gronk/Big Chunk/Guardians).
- Furnaces should start smelting on load (I'm not sure if this works in multiplayer or not).
- You can no longer save when leaving the island.
- You can no longer save if everybody is dead.


# Configurable
When running Muck for the first time with this plugin installed, MuckSaveGame.MichMcb.cfg will be created in the BepinEx/config folder.
The configurable aspects of this plugin are:

- The amount of real-life seconds you have to wait after saving before you can save again.
- Which mobs should be saved besides just the boss mobs.
- Whether or not the savegame file should be indented (to read it more easily if you want to edit your save).


# Developers
The new API to save additional chunks of data to the XML file uses `ISaveData` and `ISaveDataManager`. Implement these interfaces, and register your instance of `ISaveDataManager` by using `SaveSystem.Register`; the documentation has information on what each method does, and you can view the [source code](https://github.com/Michmcb/MuckMods) to see how it should be used.
There are useful extension methods for `XElement` in the `MuckSaveGame` namespace, to help with reading/writing data.