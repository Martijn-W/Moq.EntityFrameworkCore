using System.Collections.Generic;
using System.Threading;
using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
using Xunit;

namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider;

public class InMemoryDbAsyncEnumeratorTests
{
    private readonly Mock<IEnumerator<int>> enumeratorMock = new Mock<IEnumerator<int>>();
    private readonly InMemoryDbAsyncEnumerator<int> inMemoryDbAsyncEnumerator;

    public InMemoryDbAsyncEnumeratorTests()
    {
        inMemoryDbAsyncEnumerator = new InMemoryDbAsyncEnumerator<int>(enumeratorMock.Object);
    }

    [Fact]
    public void Given_InMemoryDbAsyncEnumerator_When_Dispose_Then_InnerEnumeratorShouldBeDisposed()
    {
        // Act
        inMemoryDbAsyncEnumerator.Dispose();

        // Assert
        enumeratorMock.Verify(x => x.Dispose());
    }

    [Fact]
    public void Given_InMemoryDbAsyncEnumerator_When_Current_Then_CurrentFromInInnerEnumeratorShouldBeUsed()
    {
        // Act
        int result = inMemoryDbAsyncEnumerator.Current;

        // Assert
        enumeratorMock.VerifyGet(x=> x.Current);
    }

    [Fact]
    public async void Given_InMemoryDbAsyncEnumerator_When_Current_Then_CurrentShouldBeSameAsInInnerEnumerator()
    {
        // Act
        await inMemoryDbAsyncEnumerator.MoveNext(CancellationToken.None);

        // Assert
        enumeratorMock.Verify(x => x.MoveNext());
    }
}