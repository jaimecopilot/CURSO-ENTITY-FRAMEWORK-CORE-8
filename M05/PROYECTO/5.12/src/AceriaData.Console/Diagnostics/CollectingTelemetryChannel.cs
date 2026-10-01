using System.Collections.Concurrent;
using Microsoft.ApplicationInsights.Channel;

namespace AceriaData.ConsoleApp.Diagnostics;

public sealed class CollectingTelemetryChannel : ITelemetryChannel
{
    private readonly ConcurrentQueue<ITelemetry> _items = new();

    public bool? DeveloperMode { get; set; } = true;
    public string EndpointAddress { get; set; } = "https://localhost.invalid/v2/track";
    public int Count => _items.Count;

    public void Send(ITelemetry item) => _items.Enqueue(item);
    public void Flush() { }
    public void Dispose() { }
}
