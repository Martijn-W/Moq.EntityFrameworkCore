﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore.Examples.Users.Entities;

namespace Moq.EntityFrameworkCore.Examples.Users;

public class UsersService
{
    private readonly UsersContext usersContext;

    public UsersService(UsersContext usersContext)
    {
        this.usersContext = usersContext;
    }

    public IList<User> GetLockedUsers()
    {
        return usersContext.Users.Where(x => x.AccountLocked).ToList();
    }

    public async Task<IList<User>> GetLockedUsersAsync()
    {
        return await usersContext.Users.Where(x => x.AccountLocked).ToListAsync();
    }

    public IList<Role> GetDisabledRoles()
    {
        return usersContext.Roles.Where(x => !x.IsEnabled).ToList();
    }

    public async Task<IList<Role>> GetDisabledRolesAsync()
    {
        return await usersContext.Roles.Where(x => !x.IsEnabled).ToListAsync();
    }

    public async Task<User> FindOneUserAsync(Expression<Func<User, bool>> predicate)
    {
        return await usersContext.Set<User>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<User>> QueryableAsync()
    {
        var query = usersContext.Users.AsQueryable();

        return await query.ToListAsync();
    }
}