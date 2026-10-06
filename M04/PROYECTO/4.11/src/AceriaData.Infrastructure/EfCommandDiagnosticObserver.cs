/*
 // RETO M04 4.11 - OBSERVADOR DIAGNOSTICLISTENER
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AceriaData.Infrastructure;

public sealed class EfCommandDiagnosticObserver :
    IObserver<DiagnosticListener>,
    IObserver<KeyValuePair<string, object?>>,
    IDisposable
{
    private readonly TimeSpan _threshold;
    private IDisposable? _allListenersSubscription;
    private IDisposable? _efCoreSubscription;

    private EfCommandDiagnosticObserver(TimeSpan threshold)
    {
        _threshold = threshold;
    }

    public int CommandExecutedCount { get; private set; }
    public int SlowQueryCount { get; private set; }

    public static EfCommandDiagnosticObserver Start(TimeSpan threshold)
    {
        var observer = new EfCommandDiagnosticObserver(threshold);
        observer._allListenersSubscription = DiagnosticListener.AllListeners.Subscribe(observer);
        return observer;
    }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Microsoft.EntityFrameworkCore")
        {
            _efCoreSubscription?.Dispose();
            _efCoreSubscription = listener.Subscribe(this);
        }
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key == RelationalEventId.CommandExecuted.Name &&
            value.Value is CommandExecutedEventData data)
        {
            CommandExecutedCount++;
            if (data.Duration >= _threshold)
                SlowQueryCount++;
        }
    }

    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void Dispose()
    {
        _efCoreSubscription?.Dispose();
        _allListenersSubscription?.Dispose();
    }
}
*/
