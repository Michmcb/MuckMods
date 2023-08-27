md output/bin
md output/pkg

dotnet publish MuckArrows/MuckArrows.csproj -c RELEASE
Copy-Item MuckArrows/bin/RELEASE/netstandard2.0/MuckArrows.dll output/bin/MuckArrows.dll
Compress-Archive -Path MuckArrows/bin/RELEASE/netstandard2.0/MuckArrows.dll,MuckArrows/icon.png,MuckArrows/README.md,MuckArrows/CHANGELOG.md,MuckArrows/manifest.json -Destination output/pkg/MuckArrows.zip -Force

dotnet publish MuckCharcoal/MuckCharcoal.csproj -c RELEASE
Copy-Item MuckCharcoal/bin/RELEASE/netstandard2.0/MuckCharcoal.dll output/bin/MuckCharcoal.dll
Compress-Archive -Path MuckCharcoal/bin/RELEASE/netstandard2.0/MuckCharcoal.dll,MuckCharcoal/icon.png,MuckCharcoal/README.md,MuckCharcoal/CHANGELOG.md,MuckCharcoal/manifest.json -Destination output/pkg/MuckCharcoal.zip -Force

dotnet publish MuckConfigurePowerupDrops/MuckConfigurePowerupDrops.csproj -c RELEASE
Copy-Item MuckConfigurePowerupDrops/bin/RELEASE/netstandard2.0/MuckConfigurePowerupDrops.dll output/bin/MuckConfigurePowerupDrops.dll
Compress-Archive -Path MuckConfigurePowerupDrops/bin/RELEASE/netstandard2.0/MuckConfigurePowerupDrops.dll,MuckConfigurePowerupDrops/icon.png,MuckConfigurePowerupDrops/README.md,MuckConfigurePowerupDrops/CHANGELOG.md,MuckConfigurePowerupDrops/manifest.json -Destination output/pkg/MuckConfigurePowerupDrops.zip -Force

dotnet publish MuckDontDestroyNeighbours/MuckDontDestroyNeighbours.csproj -c RELEASE
Copy-Item MuckDontDestroyNeighbours/bin/RELEASE/netstandard2.0/MuckDontDestroyNeighbours.dll output/bin/MuckDontDestroyNeighbours.dll
Compress-Archive -Path MuckDontDestroyNeighbours/bin/RELEASE/netstandard2.0/MuckDontDestroyNeighbours.dll,MuckDontDestroyNeighbours/icon.png,MuckDontDestroyNeighbours/README.md,MuckDontDestroyNeighbours/CHANGELOG.md,MuckDontDestroyNeighbours/manifest.json -Destination output/pkg/MuckDontDestroyNeighbours.zip -Force

dotnet publish MuckFoods/MuckFoods.csproj -c RELEASE
Copy-Item MuckFoods/bin/RELEASE/netstandard2.0/MuckFoods.dll output/bin/MuckFoods.dll
Compress-Archive -Path MuckFoods/bin/RELEASE/netstandard2.0/MuckFoods.dll,MuckFoods/icon.png,MuckFoods/README.md,MuckFoods/CHANGELOG.md,MuckFoods/manifest.json -Destination output/pkg/MuckFoods.zip -Force

dotnet publish MuckMetalCoins/MuckMetalCoins.csproj -c RELEASE
Copy-Item MuckMetalCoins/bin/RELEASE/netstandard2.0/MuckMetalCoins.dll output/bin/MuckMetalCoins.dll
Compress-Archive -Path MuckMetalCoins/bin/RELEASE/netstandard2.0/MuckMetalCoins.dll,MuckMetalCoins/icon.png,MuckMetalCoins/README.md,MuckMetalCoins/CHANGELOG.md,MuckMetalCoins/manifest.json -Destination output/pkg/MuckMetalCoins.zip -Force

dotnet publish MuckRememberLobbySettings/MuckRememberLobbySettings.csproj -c RELEASE
Copy-Item MuckRememberLobbySettings/bin/RELEASE/netstandard2.0/MuckRememberLobbySettings.dll output/bin/MuckRememberLobbySettings.dll
Compress-Archive -Path MuckRememberLobbySettings/bin/RELEASE/netstandard2.0/MuckRememberLobbySettings.dll,MuckRememberLobbySettings/icon.png,MuckRememberLobbySettings/README.md,MuckRememberLobbySettings/CHANGELOG.md,MuckRememberLobbySettings/manifest.json -Destination output/pkg/MuckRememberLobbySettings.zip -Force

dotnet publish MuckTimeModifier/MuckTimeModifier.csproj -c RELEASE
Copy-Item MuckTimeModifier/bin/RELEASE/netstandard2.0/MuckTimeModifier.dll output/bin/MuckTimeModifier.dll
Compress-Archive -Path MuckTimeModifier/bin/RELEASE/netstandard2.0/MuckTimeModifier.dll,MuckTimeModifier/icon.png,MuckTimeModifier/README.md,MuckTimeModifier/CHANGELOG.md,MuckTimeModifier/manifest.json -Destination output/pkg/MuckTimeModifier.zip -Force

