using System.Collections.Concurrent;
using System.Diagnostics;

namespace AceriaData.ConsoleApp.Diagnostics;

public sealed class EfDiagnosticObserver :
    IObserver<DiagnosticListener>,
    IObserver<KeyValuePair<string, object?>>,
    IDisposable
{
    private readonly ConcurrentBag<IDisposable> _subscriptions = new();
    private readonly IDisposable _allListenersSubscription;
    private int _efEventCount;

    public EfDiagnosticObserver()
    {
        _allListenersSubscription = DiagnosticListener.AllListeners.Subscribe(this);
    }

    public int EfEventCount => Volatile.Read(ref _efEventCount);

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Microsoft.EntityFrameworkCore")
        {
            _subscriptions.Add(listener.Subscribe(this, IsEnabled));
        }
    }

    private static bool IsEnabled(string eventName, object? arg1, object? arg2) =>
        eventName.Contains("Command", StringComparison.Ordinal) ||
        eventName.Contains("SaveChanges", StringComparison.Ordinal);

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key.StartsWith("Microsoft.EntityFrameworkCore.", StringComparison.Ordinal))
        {
            Interlocked.Increment(ref _efEventCount);
        }
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();
        _allListenersSubscription.Dispose();
    }
}
