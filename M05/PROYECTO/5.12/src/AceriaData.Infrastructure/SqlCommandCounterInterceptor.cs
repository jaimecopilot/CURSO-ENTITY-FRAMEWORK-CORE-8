using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AceriaData.Infrastructure;

public sealed class SqlCommandCounterInterceptor : DbCommandInterceptor
{
    public static SqlCommandCounterInterceptor Instance { get; } = new();

    private readonly object _gate = new();
    private readonly List<string> _commands = new();
    private long _count;

    public long Count => Interlocked.Read(ref _count);

    public void Reset()
    {
        Interlocked.Exchange(ref _count, 0);
        lock (_gate)
        {
            _commands.Clear();
        }
    }

    public IReadOnlyList<string> SnapshotCommands()
    {
        lock (_gate)
        {
            return _commands.ToArray();
        }
    }

    private void Capture(DbCommand command)
    {
        Interlocked.Increment(ref _count);
        lock (_gate)
        {
            _commands.Add(command.CommandText);
        }
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Capture(command);
        return result;
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        Capture(command);
        return result;
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        Capture(command);
        return result;
    }
}
