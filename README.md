# CMS21 Quick Spray

A [MelonLoader](https://github.com/LavaGang/MelonLoader) mod for **Car Mechanic Simulator 2021** that makes the rusty bolt spray quick.

Every stuck bolt means waiting through the rust penetrant spray animation before you can undo it. Strip a whole car and that adds up. Quick Spray plays the spray 5× faster, and nothing else in the game changes.

## How it works

- The spray plays at 5× speed by default, and you can change that.
- The speed-up only lasts while the spray is playing. The game goes back to normal speed the moment it finishes.
- You still unscrew every bolt yourself, so bolt counts and achievement progress work as normal.
- If the game is paused, Quick Spray leaves it alone.
- If a spray gets cut off partway, normal speed comes back within 3 seconds anyway.
- It doesn't change any game files or save data. Delete the DLL and the game is back to stock.

## Requirements

- Car Mechanic Simulator 2021, game version **1.0.40** (Steam)
- **MelonLoader v0.5.7**

## Installation

1. Install **MelonLoader v0.5.7** from the [MelonLoader releases page](https://github.com/LavaGang/MelonLoader/releases/tag/v0.5.7). Download `MelonLoader.x64.zip` and extract it into your game folder.
2. Launch the game once so MelonLoader can set itself up, then close it.
3. Download `QuickSpray.dll` from the [latest release](../../releases/latest).
4. Put it in your `Car Mechanic Simulator 2021\Mods\` folder.
5. Launch the game. Once you're in the garage, the MelonLoader console should show `Ready. Rusty bolt spray runs at x5.`

## Configuration

Quick Spray creates its own config file at `UserData\QuickSpray.cfg` the first time the game launches with it:

```toml
[QuickSpray]
# Turn Quick Spray on or off
Enabled = true
# How many times faster the rusty bolt spray plays (1 = normal)
Multiplier = 5.0
```

Restart the game after changing these.

## Compatibility

- Works alongside **QoLmod** (MelonLoader 0.5.7), including its debug menu speed settings. If you've set the game speed there, Quick Spray multiplies from that speed and then puts it back.
- Not compatible with MelonLoader 0.6.x or newer.

## Troubleshooting

| Problem | Fix |
| --- | --- |
| Mod doesn't appear in the console | Make sure you're on MelonLoader **v0.5.7** and the DLL is in `Mods\`, not a subfolder. |
| `Setup failed, game code not found` | A game update changed the spray code. Open an issue with your game version and the full log line. |
| Spray isn't faster | Check that `Enabled = true` and `Multiplier` is above 1 in `UserData\QuickSpray.cfg`. |

Logs are in `MelonLoader\Latest.log`.

## Building from source

You'll need the [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or newer) and the game with MelonLoader 0.5.7 installed.

```
cd src
dotnet build -c Release
```

If your game isn't in the default Steam folder, pass its path:

```
dotnet build -c Release /p:GameDir="D:\Games\Car Mechanic Simulator 2021"
```

The DLL is built to `src\bin\Release\net472\QuickSpray.dll`.

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## Author

Made by [98freestyle](https://github.com/98freestyle).
