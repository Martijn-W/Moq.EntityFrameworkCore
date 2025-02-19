﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq.EntityFrameworkCore.Examples.Users;
using Moq.EntityFrameworkCore.Examples.Users.Entities;
using Xunit;

namespace Moq.EntityFrameworkCore.Examples;

public class UsersServiceTest
{
    private static readonly Fixture Fixture = new();

    [Fact]
    public void Given_ListOfUsersWithOneUserAccountLock_When_CheckingWhoIsLocked_Then_CorrectLockedUserIsReturned()
    {
        // Arrange
        IList<User> users = GenerateNotLockedUsers();
        var lockedUser = Fixture.Build<User>().With(u => u.AccountLocked, true).Create();
        users.Add(lockedUser);

        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(x => x.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);

        // Act
        var lockedUsers = usersService.GetLockedUsers();

        // Assert
        Assert.Equal(new List<User> { lockedUser }, lockedUsers);
    }

    [Fact]
    public void Given_ListOfUsersWithOneUserAccountLockAndMockingWithSetupGet_When_CheckingWhoIsLocked_Then_CorrectLockedUserIsReturned()
    {
        // Arrange
        IList<User> users = GenerateNotLockedUsers();
        var lockedUser = Fixture.Build<User>().With(u => u.AccountLocked, true).Create();
        users.Add(lockedUser);

        var userContextMock = new Mock<UsersContext>();
        userContextMock.SetupGet(x => x.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);

        // Act
        var lockedUsers = usersService.GetLockedUsers();

        // Assert
        Assert.Equal(new List<User> { lockedUser }, lockedUsers);
    }

    [Fact]
    public async Task Given_ListOfUsersWithOneUserAccountLock_When_CheckingWhoIsLockedAsync_Then_CorrectLockedUserIsReturned()
    {
        // Arrange
        IList<User> users = GenerateNotLockedUsers();
        var lockedUser = Fixture.Build<User>().With(u => u.AccountLocked, true).Create();
        users.Add(lockedUser);

        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(x => x.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);

        // Act
        var lockedUsers = await usersService.GetLockedUsersAsync();

        // Assert
        Assert.Equal(new List<User> { lockedUser }, lockedUsers);
    }

    [Fact]
    public void Given_ListOfGroupsWithOneGroupDisabled_When_CheckingWhichOneIsDisabled_Then_CorrectDisabledRoleIsReturned()
    {
        // Arrange
        IList<Role> roles = GenerateEnabledGroups();
        var disabledRole = Fixture.Build<Role>().With(u => u.IsEnabled, false).Create();
        roles.Add(disabledRole);

        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(x => x.Roles).ReturnsDbSet(roles);

        var usersService = new UsersService(userContextMock.Object);

        // Act
        var disabledRoles = usersService.GetDisabledRoles();

        // Assert
        Assert.Equal(new List<Role> { disabledRole }, disabledRoles);
    }

    [Fact]
    public async Task Given_ListOfGroupsWithOneGroupDisabled_When_CheckingWhichOneIsDisabledAsync_Then_CorrectDisabledRoleIsReturned()
    {
        // Arrange
        IList<Role> roles = GenerateEnabledGroups();
        var disabledRole = Fixture.Build<Role>().With(u => u.IsEnabled, false).Create();
        roles.Add(disabledRole);

        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(x => x.Roles).ReturnsDbSet(roles);

        var usersService = new UsersService(userContextMock.Object);

        // Act
        var disabledRoles = await usersService.GetDisabledRolesAsync();

        // Assert
        Assert.Equal(new List<Role> { disabledRole }, disabledRoles);
    }

    [Fact]
    public async Task Given_ListOfUser_When_FindOneUserAsync_Then_CorrectUserIsReturned()
    {
        var users = GenerateNotLockedUsers();
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(x => x.Set<User>()).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);
        var user = users.FirstOrDefault();

        //Act
        var userToAssert = await usersService.FindOneUserAsync(x => x.Id == user.Id);

        //Assert
        Assert.Equal(userToAssert, user);
    }

    [Fact]
    public async Task Given_Two_ListOfUser_Then_CorrectListIsReturned_InSequence()
    {
        var users = GenerateNotLockedUsers();
        var userContextMock = new Mock<UsersContext>();
        userContextMock.SetupSequence(x => x.Set<User>())
            .ReturnsDbSet(new List<User>())
            .ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);

        var user = users.FirstOrDefault();

        //Act
        var userToAssertWhenFirstCall = await usersService.FindOneUserAsync(x => x.Id == user.Id);
        var userToAssertWhenSecondCall = await usersService.FindOneUserAsync(x => x.Id == user.Id);

        //Assert
        Assert.Null(userToAssertWhenFirstCall);
        Assert.Equal(userToAssertWhenSecondCall, user);
    }

    [Fact]
    public async Task Given_Queryable_Then_Return_Queryable()
    {
        var users = GenerateNotLockedUsers();
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(c => c.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);

        //Act
        var actual = await usersService.QueryableAsync();

        //Assert
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public async Task Given_ExecuteDeleteAsync_Then_ReturnInstead_Count()
    {
        var users = new[]
        {
            Fixture.Build<User>().With(u => u.AccountLocked, true).Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, true).Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, false).Create()
        };
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(c => c.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);
        
        //Act
        var actual = await usersService.BulkDeleteLockedUsersAsync();

        //Assert
        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task Given_ExecuteDeleteAsync_With_Multiple_Statements_Then_ReturnInstead_Count()
    {
        var users = new[]
        {
            Fixture.Build<User>().With(u => u.AccountLocked, true).With(u => u.Name, "Unit").Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, true).With(u => u.Name, "Hans").Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, false).With(u => u.Name, "Jane").Create()
        };
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(c => c.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);
        
        //Act
        var actual = await usersService.BulkDeleteWithMultipleWhereAsync();

        //Assert
        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task Given_ExecuteUpdateAsync_Then_ReturnInstead_Count()
    {
        var users = new[]
        {
            Fixture.Build<User>().With(u => u.AccountLocked, true).Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, true).Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, true).Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, false).Create()
        };
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(c => c.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);
        
        //Act
        var actual = await usersService.BulkUpdateUsersAsync();

        //Assert
        Assert.Equal(3, actual);
    }
    
    [Fact]
    public async Task Given_ExecuteUpdateAsync_With_Multiple_Statements_Then_ReturnInstead_Count()
    {
        var users = new[]
        {
            Fixture.Build<User>().With(u => u.AccountLocked, true).With(u => u.Name, "Unit Tester").Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, true).With(u => u.Name, "Unit Runner").Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, false).With(u => u.Name, "Jane").Create()
        };
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(c => c.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);
        
        //Act
        var actual = await usersService.BulkUpdateWithMultipleWhereAsync();

        //Assert
        Assert.Equal(2, actual);
    }

    [Fact]
    public async Task Given_ExecuteUpdateAsync_With_Select_Then_ReturnInstead_Count()
    {
        var users = new[]
        {
            Fixture.Build<User>().With(u => u.AccountLocked, true).With(u => u.Name, "Unit Tester").Create()
        };
        var userContextMock = new Mock<UsersContext>();
        userContextMock.Setup(c => c.Users).ReturnsDbSet(users);

        var usersService = new UsersService(userContextMock.Object);
        
        //Act
        var actual = await usersService.BulkUpdateWithSelectAsync();

        //Assert
        Assert.Equal(1, actual);
    }

    private static IList<User> GenerateNotLockedUsers()
    {
        IList<User> users = new List<User>
        {
            Fixture.Build<User>().With(u => u.AccountLocked, false).Create(),
            Fixture.Build<User>().With(u => u.AccountLocked, false).Create()
        };

        return users;
    }

    private static IList<Role> GenerateEnabledGroups()
    {
        IList<Role> users = new List<Role>
        {
            Fixture.Build<Role>().With(u => u.IsEnabled, true).Create(),
            Fixture.Build<Role>().With(u => u.IsEnabled, true).Create()
        };

        return users;
    }
}