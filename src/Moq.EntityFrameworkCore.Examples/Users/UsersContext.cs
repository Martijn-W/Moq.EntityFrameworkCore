using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore.Examples.Users.Entities;

namespace Moq.EntityFrameworkCore.Examples.Users;

public class UsersContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Role> Roles { get; set; }
}