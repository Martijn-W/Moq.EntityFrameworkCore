using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Moq.EntityFrameworkCore.DbAsyncQueryProvider;

public class InMemoryAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _innerQueryProvider;

    public InMemoryAsyncQueryProvider(IQueryProvider innerQueryProvider)
    {
        _innerQueryProvider = innerQueryProvider;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new InMemoryAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new InMemoryAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        if (expression is MethodCallExpression
            {
                Method.Name: "Count" or nameof(RelationalQueryableExtensions.ExecuteDelete) or nameof(RelationalQueryableExtensions.ExecuteUpdate)
            } methodCall)
            expression = RewriteExpressionToCount(methodCall);

        return _innerQueryProvider.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _innerQueryProvider.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new())
    {
        var result = Execute(expression);

        var expectedResultType = typeof(TResult).GetGenericArguments()?.FirstOrDefault();
        if (expectedResultType == null)
            return default;

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
            ?.MakeGenericMethod(expectedResultType)
            .Invoke(null, new[] { result });
    }


    public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute(expression));
    }

    private static Expression RewriteExpressionToCount(MethodCallExpression methodCall)
    {
        var elementType = methodCall.Method.GetGenericArguments()[0];
        var isSelect = methodCall.Arguments[0] is MethodCallExpression { Method.Name: nameof(Queryable.Select) };

        var methodInfo = isSelect
            ? QueryableMethods.CountWithoutPredicate.MakeGenericMethod(elementType)
            : QueryableMethods.CountWithPredicate.MakeGenericMethod(elementType);

        if (isSelect)
            return Expression.Call(
                methodInfo,
                methodCall.Arguments[0]
            );

        var args = ((MethodCallExpression)methodCall.Arguments[0]).Arguments;

        return Expression.Call(
            methodInfo,
            args[0],
            args[1]
        );
    }
}