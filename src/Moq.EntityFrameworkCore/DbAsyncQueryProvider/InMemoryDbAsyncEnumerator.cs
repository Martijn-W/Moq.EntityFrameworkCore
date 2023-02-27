using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moq.EntityFrameworkCore.DbAsyncQueryProvider;

public class InMemoryDbAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _innerEnumerator;
    private bool _disposed;

    public InMemoryDbAsyncEnumerator(IEnumerator<T> enumerator)
    {
        _innerEnumerator = enumerator;
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
        return Task.FromResult(_innerEnumerator.MoveNext());
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(Task.FromResult(_innerEnumerator.MoveNext()));
    }

    public T Current => _innerEnumerator.Current;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources.
                _innerEnumerator.Dispose();
            }

            _disposed = true;
        }
    }
}