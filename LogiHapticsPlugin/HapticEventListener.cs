namespace Loupedeck.LogiHapticsPlugin;

using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal sealed class HapticEventListener : IDisposable
{
    private readonly HttpListener _listener;
    private readonly Action<String> _onEvent;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;

    public HapticEventListener(Int32 port, Action<String> onEvent)
    {
        _onEvent = onEvent;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/haptic/");
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        _listener.Start();
        _listenTask = Task.Run(() => ListenLoop(_cts.Token));
        PluginLog.Info($"Haptic listener started on {_listener.Prefixes.First()}");
    }

    public void Stop()
    {
        _cts?.Cancel();
        _listener.Stop();
        try { _listenTask?.Wait(TimeSpan.FromSeconds(2)); } catch { }
        PluginLog.Info("Haptic listener stopped");
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
        _listener.Close();
    }

    private async Task ListenLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync().ConfigureAwait(false);
                _ = Task.Run(() => HandleRequest(context), ct);
            }
            catch (HttpListenerException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Listener error: {ex.Message}");
            }
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "POST")
            {
                response.StatusCode = 405;
                response.Close();
                return;
            }

            if (request.ContentLength64 > 1024)
            {
                response.StatusCode = 413;
                response.Close();
                return;
            }

            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            var body = reader.ReadToEnd();

            var eventName = ParseEventName(body);
            if (eventName != null)
            {
                _onEvent(eventName);
                response.StatusCode = 200;
            }
            else
            {
                response.StatusCode = 400;
            }

            response.Close();
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Request handling error: {ex.Message}");
            try { context.Response.StatusCode = 500; context.Response.Close(); } catch { }
        }
    }

    private static String? ParseEventName(String json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("event", out var eventElement))
            {
                return eventElement.GetString();
            }
        }
        catch (JsonException ex)
        {
            PluginLog.Warning($"Invalid JSON: {ex.Message}");
        }
        return null;
    }
}
