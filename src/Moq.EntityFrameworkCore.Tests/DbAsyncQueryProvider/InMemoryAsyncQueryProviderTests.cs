using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Moq.EntityFrameworkCore.DbAsyncQueryProvider;
using Xunit;

namespace Moq.EntityFrameworkCore.Tests.DbAsyncQueryProvider;

public class InMemoryAsyncQueryProviderTests
{
    private readonly Mock<IQueryProvider> queryProviderMock = new();
    private readonly Expression expression = new Mock<Expression>().Object;
    private readonly InMemoryAsyncQueryProvider<int> inMemoryAsyncQueryProvider;

    public InMemoryAsyncQueryProviderTests()
    {
        inMemoryAsyncQueryProvider = new InMemoryAsyncQueryProvider<int>(queryProviderMock.Object);
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_CreatingQuery_Then_CorrectInMemoryAsyncEnumerableIsReturned()
    {
        // Act
        IQueryable result = inMemoryAsyncQueryProvider.CreateQuery(expression);

        // Assert
        Assert.IsType<InMemoryAsyncEnumerable<int>>(result);
        Assert.Equal(expression, result.Expression);
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_CreatingQueryGeneric_Then_CorrectInMemoryAsyncEnumerableIsReturned()
    {
        // Act
        IQueryable result = inMemoryAsyncQueryProvider.CreateQuery<int>(expression);

        // Assert
        Assert.IsType<InMemoryAsyncEnumerable<int>>(result);
        Assert.Equal(expression, result.Expression);
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQuery_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        inMemoryAsyncQueryProvider.Execute(expression);

        // Assert
        queryProviderMock.Verify(x=> x.Execute(expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryGeneric_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        inMemoryAsyncQueryProvider.Execute<int>(expression);

        // Assert
        queryProviderMock.Verify(x => x.Execute<int>(expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsync_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        inMemoryAsyncQueryProvider.ExecuteAsync<int>(expression);

        // Assert
        queryProviderMock.Verify(x => x.Execute(expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsyncWithCancellationToken_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        inMemoryAsyncQueryProvider.ExecuteAsync(expression, CancellationToken.None);

        // Assert
        queryProviderMock.Verify(x => x.Execute(expression));
    }

    [Fact]
    public void Given_InMemoryAsyncQueryProvider_When_ExecutingQueryAsyncAndGeneric_Then_ExecutionIsDoneAtInnerQueryProvider()
    {
        // Act
        inMemoryAsyncQueryProvider.ExecuteAsync<int>(expression, CancellationToken.None);

        // Assert
        queryProviderMock.Verify(x => x.Execute(expression));
    }
}