namespace Loupedeck.LogiHapticsPlugin;

using System;

public class LogiHapticsPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    private HapticEventListener? _listener;
    private const Int32 DefaultPort = 8484;

    private static readonly (String Name, String DisplayName, String Description)[] BaseEvents =
    {
        ("menuItemHover",     "Menu Hover",        "Detent click when hovering between menu entries"),
        ("menuOpen",          "Menu Open",         "Right-click menu opened"),
        ("menuOptionSelect",  "Menu Select",       "Menu option selected"),
        ("npcClick",          "NPC Click",         "Clicked on an NPC"),
        ("objectClick",       "Object Click",      "Clicked on a game object"),
        ("groundItemClick",   "Ground Item Click", "Clicked on a ground item"),
        ("inventoryClick",    "Inventory Click",   "Clicked on an inventory item"),
        ("moveClick",         "Move Click",        "Clicked to walk or run"),
        ("npcHover",          "NPC Hover",         "Cursor hovered over an NPC"),
        ("objectHover",       "Object Hover",      "Cursor hovered over a game object"),
    };

    private static readonly (String Suffix, String DisplaySuffix)[] Waveforms =
    {
        ("subtle_collision",    "Subtle Collision"),
        ("damp_collision",      "Damp Collision"),
        ("damp_state_change",   "Damp State Change"),
        ("sharp_collision",     "Sharp Collision"),
        ("sharp_state_change",  "Sharp State Change"),
        ("knock",               "Knock"),
    };

    public LogiHapticsPlugin()
    {
        PluginLog.Init(this.Log);
        PluginResources.Init(this.Assembly);
    }

    public override void Load()
    {
        foreach (var (name, displayName, description) in BaseEvents)
        {
            foreach (var (suffix, displaySuffix) in Waveforms)
            {
                this.PluginEvents.AddEvent(
                    $"{name}.{suffix}",
                    $"{displayName} ({displaySuffix})",
                    $"{description} [{displaySuffix}]");
            }
        }

        var port = DefaultPort;
        if (this.TryGetPluginSetting("port", out var portStr) && Int32.TryParse(portStr, out var parsed))
        {
            port = parsed;
        }
        else
        {
            this.SetPluginSetting("port", DefaultPort.ToString());
        }

        _listener = new HapticEventListener(port, OnHapticEvent);
        _listener.Start();

        PluginLog.Info($"Logitech Haptics plugin loaded ({BaseEvents.Length * Waveforms.Length} events)");
    }

    public override void Unload()
    {
        _listener?.Dispose();
        _listener = null;
        PluginLog.Info("RuneLite Haptics plugin unloaded");
    }

    private void OnHapticEvent(String eventName)
    {
        this.PluginEvents.RaiseEvent(eventName);
    }
}
