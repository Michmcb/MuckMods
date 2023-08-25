md output

dotnet publish MuckCharcoal/MuckCharcoal.csproj -c RELEASE
Compress-Archive -Path MuckCharcoal/bin/RELEASE/netstandard2.0/MuckCharcoal.dll,MuckCharcoal/icon.png,MuckCharcoal/README.md,MuckCharcoal/manifest.json -Destination output/MuckCharcoal.zip -Force

dotnet publish MuckConfigurePowerupDrops/MuckConfigurePowerupDrops.csproj -c RELEASE
Compress-Archive -Path MuckConfigurePowerupDrops/bin/RELEASE/netstandard2.0/MuckConfigurePowerupDrops.dll,MuckConfigurePowerupDrops/icon.png,MuckConfigurePowerupDrops/README.md,MuckConfigurePowerupDrops/manifest.json -Destination output/MuckConfigurePowerupDrops.zip -Force

dotnet publish MuckFoods/MuckFoods.csproj -c RELEASE
Compress-Archive -Path MuckFoods/bin/RELEASE/netstandard2.0/MuckFoods.dll,MuckFoods/icon.png,MuckFoods/README.md,MuckFoods/manifest.json -Destination output/MuckFoods.zip -Force

dotnet publish MuckMetalCoins/MuckMetalCoins.csproj -c RELEASE
Compress-Archive -Path MuckMetalCoins/bin/RELEASE/netstandard2.0/MuckMetalCoins.dll,MuckMetalCoins/icon.png,MuckMetalCoins/README.md,MuckMetalCoins/manifest.json -Destination output/MuckMetalCoins.zip -Force

dotnet publish MuckTimeModifier/MuckTimeModifier.csproj -c RELEASE
Compress-Archive -Path MuckTimeModifier/bin/RELEASE/netstandard2.0/MuckTimeModifier.dll,MuckTimeModifier/icon.png,MuckTimeModifier/README.md,MuckTimeModifier/manifest.json -Destination output/MuckTimeModifier.zip -Force
