using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore.Examples.Users.Entities;

namespace Moq.EntityFrameworkCore.Examples.Users;

public class UsersService
{
    private readonly UsersContext _usersContext;

    public UsersService(UsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public IList<User> GetLockedUsers()
    {
        return _usersContext.Users.Where(x => x.AccountLocked).ToList();
    }

    public async Task<IList<User>> GetLockedUsersAsync()
    {
        return await _usersContext.Users.Where(x => x.AccountLocked).ToListAsync();
    }

    public IList<Role> GetDisabledRoles()
    {
        return _usersContext.Roles.Where(x => !x.IsEnabled).ToList();
    }

    public async Task<IList<Role>> GetDisabledRolesAsync()
    {
        return await _usersContext.Roles.Where(x => !x.IsEnabled).ToListAsync();
    }

    public async Task<User> FindOneUserAsync(Expression<Func<User, bool>> predicate)
    {
        return await _usersContext.Set<User>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<User>> QueryableAsync()
    {
        var query = _usersContext.Users.AsQueryable();

        return await query.ToListAsync();
    }

    public async Task<int> BulkDeleteLockedUsersAsync()
    {
        return await _usersContext.Users.Where(u => u.AccountLocked)
            .ExecuteDeleteAsync();
    }
    
    public async Task<int> BulkDeleteWithMultipleWhereAsync()
    {
        return await _usersContext.Users.Where(u => u.AccountLocked)
            .Where(u => u.Name == "Unit")
            .ExecuteDeleteAsync();
    }

    public async Task<int> BulkUpdateUsersAsync()
    {
        return await _usersContext.Users.Where(u => u.AccountLocked)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.AccountLocked, false));
    }

    public async Task<int> BulkUpdateWithMultipleWhereAsync()
    {
        return await _usersContext.Users.Where(u => u.AccountLocked)
            .Where(u => u.Name.Contains("Unit"))
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.AccountLocked, false));
    }
}