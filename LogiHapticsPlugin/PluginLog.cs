namespace Loupedeck.LogiHapticsPlugin;

using System;

internal static class PluginLog
{
    private static PluginLogFile? _log;

    public static void Init(PluginLogFile log) =>
        _log = log;

    public static void Info(String message) =>
        _log?.Info(message);

    public static void Warning(String message) =>
        _log?.Warning(message);

    public static void Error(String message) =>
        _log?.Error(message);
}
