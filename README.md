# Logitech Haptics - Logi Options+ Plugin

A [Logi Options+](https://www.logitech.com/en-us/software/logi-options-plus.html) Actions SDK plugin that receives haptic events from RuneLite and triggers waveforms on the [Logitech MX Master 4](https://www.logitech.com/en-us/products/mice/mx-master-4.html) mouse.

Requires the companion [RuneLite plugin](https://github.com/claytonmark/logitech_runelite_haptics-runelite_plugin) to send events.

## Features

- Hosts a localhost HTTP listener (port 8484) that receives haptic event payloads from the RuneLite plugin
- Maps 10 base events to haptic waveforms with 6 waveform variants each (70 total registered events)
- Waveform selection is driven by the RuneLite plugin's config panel

## Requirements

- [Logi Options+](https://www.logitech.com/en-us/software/logi-options-plus.html) 1.95+
- Logitech MX Master 4 mouse (only mouse with haptic motor support)
- .NET 8 SDK (for building from source)
- `LogiPluginTool` CLI (`dotnet tool install --global LogiPluginTool`)

## Installation

### From GitHub Releases (Recommended)

1. Go to the [Releases](https://github.com/claytonmark/logitech_runelite_haptics-logi_options_action/releases) page
2. Download `LogiHaptics.lplug4` from the latest release
3. Double-click the downloaded `.lplug4` file — Logi Options+ will open and install the plugin automatically
4. Restart Logi Options+ if prompted

### From Source

1. Build the plugin:
   ```bash
   dotnet build -c Release
   ```
2. Package for distribution:
   ```bash
   logiplugintool pack ./bin/Release/ ./LogiHaptics.lplug4
   logiplugintool verify ./LogiHaptics.lplug4
   ```
3. Install the `.lplug4` file via Logi Options+.

## Haptic Events

| Event | Default Waveform | Trigger |
|---|---|---|
| `menuItemHover` | `subtle_collision` | Mouse moves to a different menu entry |
| `menuOpen` | `damp_state_change` | Right-click menu opens |
| `menuOptionSelect` | `sharp_state_change` | User clicks a menu option |
| `npcClick` | `damp_collision` | Click on an NPC |
| `objectClick` | `damp_collision` | Click on a game object |
| `groundItemClick` | `subtle_collision` | Click on a ground item |
| `inventoryClick` | `subtle_collision` | Click on an inventory item |
| `moveClick` | `subtle_collision` | Click to walk/run |
| `npcHover` | `subtle_collision` | Cursor hovers over an NPC |
| `objectHover` | `subtle_collision` | Cursor hovers over a game object |

## Protocol

The RuneLite plugin sends HTTP POST requests to `http://localhost:8484/haptic/`:

```json
{ "event": "npcClick" }
```

For non-default waveforms, the event name includes a suffix:

```json
{ "event": "npcClick.sharp_collision" }
```

## Log Files

- **Windows**: `C:\Users\<user>\AppData\Local\Logi\LogiPluginService\Logs\plugin_logs\`
- **macOS**: `~/Library/Application Support/Logi/LogiPluginService/Logs/plugin_logs/`

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE.md](LICENSE.md) for details.
