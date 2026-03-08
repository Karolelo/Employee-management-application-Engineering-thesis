using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repo.Core.Infrastructure.Database;
using FluentAssertions;
using Repo.Core.Models;
using Repo.Server.WorkTimeModule.Services;
using Task = System.Threading.Tasks.Task;

namespace Repo.Server.Tests.UserManagmentModule.Services;

[TestClass]
public class WorkTimeTest
{
    private MyDbContext _context;
    private WorkEntryService _service;
    [TestInitialize]
    public void SetUp()
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new MyDbContext(optionsBuilder.Options);
        _service = new WorkEntryService(_context);
    }

    [TestCleanup]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region getEntryFromId

    [TestMethod]
    public async Task GetEntryFromId_shouldReturnWorkEntry_WhenExists()
    {
        //Arrange
        await _context.Users.AddRangeAsync(
            new User { ID = 1, Name = "John",Surname = "Down",Login = "Johnyy",
                Password = "KArol12",
                Nickname = "Benny",
                Email = "john@test.com", 
                Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            },
            new User { ID = 2, Name = "Jane",Surname = "Lee",Login = "JaneDons12",Password = "KArolasd12"
                ,Nickname = "Ronnie", Email = "jane@test.com", Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            }
        );

        await _context.WorkTables.AddAsync(new WorkTable()
        {
            ID = 1,
            User_ID = 1,
            AbsenceDays = new List<AbsenceDay>(),
            Account_Number = "12314-13123-13123-12312",
            Hourly_Rate = Decimal.One,
            WorkEntries = new List<WorkEntry>(),
            WorkTasks = new List<WorkTask>()
        });

        await _context.WorkEntries.AddAsync(new WorkEntry()
        {
            ID = 1,
            Comment = "Bro you did a good job",
            Hours_Worked = new decimal(10),
            Work_Date = DateTime.Now,
            WorkTable_ID = 1
        });

        await _context.SaveChangesAsync();
        
        //Act
        var result = await _service.GetEntryById(1);
        
        //Assert
        result.Data.Should().NotBeNull();
        result.Data.ID.Should().Be(1);
        result.Data.Hours_Worked.Should().Be(new decimal(10));
        result.Data.Comment.Should().Be("Bro you did a good job");

    }

    [TestMethod]
    public async Task GetEntryFromId_shouldReturnFailResponse_WhenNotExists()
    {
        //Arrange
        await _context.Users.AddRangeAsync(
            new User
            {
                ID = 1, Name = "John", Surname = "Down", Login = "Johnyy",
                Password = "KArol12",
                Nickname = "Benny",
                Email = "john@test.com",
                Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            },
            new User
            {
                ID = 2, Name = "Jane", Surname = "Lee", Login = "JaneDons12", Password = "KArolasd12",
                Nickname = "Ronnie", Email = "jane@test.com", Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            }
        );

        await _context.WorkTables.AddAsync(new WorkTable()
        {
            ID = 1,
            User_ID = 1,
            AbsenceDays = new List<AbsenceDay>(),
            Account_Number = "12314-13123-13123-12312",
            Hourly_Rate = Decimal.One,
            WorkEntries = new List<WorkEntry>(),
            WorkTasks = new List<WorkTask>()
        });

        await _context.WorkEntries.AddAsync(new WorkEntry()
        {
            ID = 1,
            Comment = "Bro you did a good job",
            Hours_Worked = new decimal(10),
            Work_Date = DateTime.Now,
            WorkTable_ID = 1
        });

        await _context.SaveChangesAsync();
        
        //Act
        var result = await _service.GetEntryById(4);
        
        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Work entry not found");
    }
    

    #endregion

    #region GetEntriesForAdmin
    [TestMethod]
    public async Task GetEntriesForAdmin_ShouldReturnEntries_WhenUserExists()
    {
        //Arrange
        await _context.Users.AddRangeAsync(
            new User
            {
                ID = 1, Name = "John", Surname = "Down", Login = "Johnyy",
                Password = "KArol12",
                Nickname = "Benny",
                Email = "john@test.com",
                Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            },
            new User
            {
                ID = 2, Name = "Jane", Surname = "Lee", Login = "JaneDons12", Password = "KArolasd12",
                Nickname = "Ronnie", Email = "jane@test.com", Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            }
        );

        await _context.WorkTables.AddAsync(new WorkTable()
        {
            ID = 1,
            User_ID = 2,
            AbsenceDays = new List<AbsenceDay>(),
            Account_Number = "12314-13123-13123-12312",
            Hourly_Rate = Decimal.One,
            WorkEntries = new List<WorkEntry>(),
            WorkTasks = new List<WorkTask>()
        });

        await _context.WorkEntries.AddRangeAsync(
            new WorkEntry()
        {
            ID = 1,
            Comment = "Bro you did a good job",
            Hours_Worked = new decimal(10),
            Work_Date = DateTime.Now.AddDays(10),
            WorkTable_ID = 1
        },
            new WorkEntry() 
            {
             ID = 2, 
             Comment = "Could be little better",
             Hours_Worked = new decimal(10),
             Work_Date = DateTime.Now.AddDays(100),
             WorkTable_ID = 1
            }
            );
        
        await _context.SaveChangesAsync();
        
        //Act
        var result = await _service.GetEntriesForAdmin(2, DateTime.Now.AddDays(10), DateTime.Now.AddDays(200));
        
        //Assert
        result.Success.Should().BeTrue();
        result.Data.ElementAt(0).Comment.Should().Contain("Could be little better");
        result.Data.Count.Should().Be(2);
    }

    [TestMethod]
     public async Task GetEntriesForAdmin_ShouldReturnError_WhenUserHasNoEntries()
    {
        //Arrange
        await _context.Users.AddRangeAsync(
            new User
            {
                ID = 1, Name = "John", Surname = "Down", Login = "Johnyy",
                Password = "KArol12",
                Nickname = "Benny",
                Email = "john@test.com",
                Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            },
            new User
            {
                ID = 2, Name = "Jane", Surname = "Lee", Login = "JaneDons12", Password = "KArolasd12",
                Nickname = "Ronnie", Email = "jane@test.com", Roles = new List<Role>(),
                Salt = Core.Infrastructure.AuthenticationHelpers.GenerateSalt(5)
            }
        );

        await _context.WorkTables.AddAsync(new WorkTable()
        {
            ID = 1,
            User_ID = 2,
            AbsenceDays = new List<AbsenceDay>(),
            Account_Number = "12314-13123-13123-12312",
            Hourly_Rate = Decimal.One,
            WorkEntries = new List<WorkEntry>(),
            WorkTasks = new List<WorkTask>()
        });
        
        
        await _context.SaveChangesAsync();
        
        //Act
        var result = await _service.GetEntriesForAdmin(2, DateTime.Now.AddDays(10), DateTime.Now.AddDays(20));
        
        //Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("No work entries");
    }
    #endregion
    
}