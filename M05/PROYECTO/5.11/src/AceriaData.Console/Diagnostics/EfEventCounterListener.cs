using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

namespace AceriaData.ConsoleApp.Diagnostics;

public sealed class EfEventCounterListener : EventListener
{
    private readonly ConcurrentDictionary<string, double> _values = new(StringComparer.Ordinal);

    public int CounterCount => _values.Count;
    public IReadOnlyDictionary<string, double> Values => _values;

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == "Microsoft.EntityFrameworkCore")
        {
            EnableEvents(
                eventSource,
                EventLevel.LogAlways,
                EventKeywords.All,
                new Dictionary<string, string?> { ["EventCounterIntervalSec"] = "1" });
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName != "EventCounters" || eventData.Payload is null || eventData.Payload.Count == 0)
            return;

        if (eventData.Payload[0] is not IDictionary<string, object> payload)
            return;

        if (!payload.TryGetValue("Name", out var nameValue) || nameValue is not string name)
            return;

        double? value = null;
        if (payload.TryGetValue("Mean", out var mean) && mean is double meanValue)
            value = meanValue;
        else if (payload.TryGetValue("Increment", out var increment) && increment is double incrementValue)
            value = incrementValue;

        if (value.HasValue)
            _values[name] = value.Value;
    }
}
