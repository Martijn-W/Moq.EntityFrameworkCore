using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moq.EntityFrameworkCore.DbAsyncQueryProvider;

public class InMemoryDbAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> innerEnumerator;
    private bool disposed;

    public InMemoryDbAsyncEnumerator(IEnumerator<T> enumerator)
    {
        innerEnumerator = enumerator;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return new ValueTask();
    }

    public Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        return Task.FromResult(innerEnumerator.MoveNext());
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(Task.FromResult(innerEnumerator.MoveNext()));
    }

    public T Current => innerEnumerator.Current;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources.
                innerEnumerator.Dispose();
            }

            disposed = true;
        }
    }
}