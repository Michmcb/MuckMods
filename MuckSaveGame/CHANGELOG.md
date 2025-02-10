# v0.5.0
- Initial fork of flarfo's Save Utility released
- Markers for the boat and gems on the map are saved
- Mob spawnrate and weights are saved
- Boss rotation is saved
- Saving and reloading during a boss night does not forcibly spawn another new boss. Only one forced boss spawn will occur, at the beginning of the night, as is the case in vanilla. More bosses may still spawn by random chance during nighttime
- Chief, if currently spawned, is saved (just like Big Chunk, Gronk, and Guardians)
- Furnaces start smelting on game load (unsure if works in multiplayer)
- You cannot save when leaving the island
- You cannot save if everybody is dead

# v0.6.0
- Fixed Cauldrons not starting up on load
- Added ability to offset local player Y position on load, to help prevent falling through the floor when a savegame is loaded

# v0.7.0
- More checks to try and prevent errors on saving game

# v0.8.0
- Mods that don't add their items to allScriptableItems and instead just add items to allItems are saved and loaded correctly

# v0.9.0
- Partial fix for upgradeable buildings; saving and loading now works. However, the state of upgraded objects is not saved. This would have to be something that the other mod would have to support saving and loading, or would have to be explicitly checked for and handled. Either way, for now, a partial fix has been applied.

# v0.9.1
- Fix for floating-point values being parsed according to the current locale, which could cause failures on load as floats were always saved with a . for the decimal point. Now they are parsed in a locale-independent manner.