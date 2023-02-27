using System.Collections.Generic;
using System.Threading;
using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
using Xunit;

namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider;

public class InMemoryDbAsyncEnumeratorTests
{
    private readonly Mock<IEnumerator<int>> _enumeratorMock = new();
    private readonly InMemoryDbAsyncEnumerator<int> _inMemoryDbAsyncEnumerator;

    public InMemoryDbAsyncEnumeratorTests()
    {
        _inMemoryDbAsyncEnumerator = new InMemoryDbAsyncEnumerator<int>(_enumeratorMock.Object);
    }

    [Fact]
    public void Given_InMemoryDbAsyncEnumerator_When_Dispose_Then_InnerEnumeratorShouldBeDisposed()
    {
        // Act
        _inMemoryDbAsyncEnumerator.Dispose();

        // Assert
        _enumeratorMock.Verify(x => x.Dispose());
    }

    [Fact]
    public void Given_InMemoryDbAsyncEnumerator_When_Current_Then_CurrentFromInInnerEnumeratorShouldBeUsed()
    {
        // Act
        int result = _inMemoryDbAsyncEnumerator.Current;

        // Assert
        _enumeratorMock.VerifyGet(x=> x.Current);
    }

    [Fact]
    public async void Given_InMemoryDbAsyncEnumerator_When_Current_Then_CurrentShouldBeSameAsInInnerEnumerator()
    {
        // Act
        await _inMemoryDbAsyncEnumerator.MoveNext(CancellationToken.None);

        // Assert
        _enumeratorMock.Verify(x => x.MoveNext());
    }
}