using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AceriaData.Infrastructure;

public sealed class SqlCommandCounterInterceptor : DbCommandInterceptor
{
    public static SqlCommandCounterInterceptor Instance { get; } = new();

    private long _count;
    public long Count => Interlocked.Read(ref _count);

    public void Reset() => Interlocked.Exchange(ref _count, 0);

    private void Increment() => Interlocked.Increment(ref _count);

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Increment();
        return result;
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        Increment();
        return result;
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        Increment();
        return result;
    }
}
