namespace Loupedeck.LogiHapticsPlugin;

using System;
using System.IO;
using System.Reflection;

internal static class PluginResources
{
    private static Assembly? _assembly;

    public static void Init(Assembly assembly) =>
        _assembly = assembly;

    public static Stream? GetStream(String resourceName) =>
        _assembly?.GetManifestResourceStream(resourceName);
}
