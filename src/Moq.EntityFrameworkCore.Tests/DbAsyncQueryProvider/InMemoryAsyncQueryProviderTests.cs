using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
using Xunit;

namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider;

public class InMemoryAsyncQueryProviderTests
{
    private readonly Mock<IQueryProvider> _queryProviderMock = new();
    private readonly Expression _expression = new Mock<Expression>().Object;
    private readonly InMemoryAsyncQueryProvider<int> _inMemoryAsyncQueryProvider;

    public InMemoryAsyncQueryProviderTests()
    {
        _inMemoryAsyncQueryProvider = new InMemoryAsyncQueryProvider<int>(_queryProviderMock.Object);
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_CreatingQuery_Then_CorrectInMemoryAsyncEnumerableIsReturned()
    {
        // Act
        IQueryable result = _inMemoryAsyncQueryProvider.CreateQuery(_expression);

        // Assert
        Assert.IsType<InMemoryAsyncEnumerable<int>>(result);
        Assert.Equal(_expression, result.Expression);
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_CreatingQueryGeneric_Then_CorrectInMemoryAsyncEnumerableIsReturned()
    {
        // Act
        IQueryable result = _inMemoryAsyncQueryProvider.CreateQuery<int>(_expression);

        // Assert
        Assert.IsType<InMemoryAsyncEnumerable<int>>(result);
        Assert.Equal(_expression, result.Expression);
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQuery_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        _inMemoryAsyncQueryProvider.Execute(_expression);

        // Assert
        _queryProviderMock.Verify(x=> x.Execute(_expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryGeneric_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        _inMemoryAsyncQueryProvider.Execute<int>(_expression);

        // Assert
        _queryProviderMock.Verify(x => x.Execute<int>(_expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsync_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        _inMemoryAsyncQueryProvider.ExecuteAsync<int>(_expression);

        // Assert
        _queryProviderMock.Verify(x => x.Execute(_expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsyncWithCancellationToken_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        _inMemoryAsyncQueryProvider.ExecuteAsync(_expression, CancellationToken.None);

        // Assert
        _queryProviderMock.Verify(x => x.Execute(_expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsyncAndGeneric_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        _inMemoryAsyncQueryProvider.ExecuteAsync<int>(_expression, CancellationToken.None);

        // Assert
        _queryProviderMock.Verify(x => x.Execute(_expression));
    }
}