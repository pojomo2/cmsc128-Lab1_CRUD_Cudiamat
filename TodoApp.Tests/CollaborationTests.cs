using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CollaborationTests : IDisposable
{
    private readonly SqliteConnection _conn = new("Data Source=:memory:");
    private readonly TodoDbContext _db;
    private readonly User _nikko = new() { Email = "nikko@test.com", DisplayName = "Nikko" };
    private readonly User _victor = new() { Email = "victor@test.com", DisplayName = "Victor" };
    private readonly TodoList _nikkoList, _victorList;
    private readonly TodoItem _nikkoTask, _victorTask;

    public CollaborationTests()
    {
        _conn.Open();
        _db = new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>().UseSqlite(_conn).Options);
        _db.Database.EnsureCreated();
        _db.Users.AddRange(_nikko, _victor);
        _db.SaveChanges();

        _nikkoList  = new TodoList { Name = "Nikko's Group List",  OwnerId = _nikko.Id };
        _victorList = new TodoList { Name = "Victor's Group List", OwnerId = _victor.Id };
        _db.Lists.AddRange(_nikkoList, _victorList);
        _db.SaveChanges();

        _nikkoTask  = new TodoItem { Title = "N1", UserId = _nikko.Id,  ListId = _nikkoList.Id };
        _victorTask = new TodoItem { Title = "V1", UserId = _victor.Id, ListId = _victorList.Id };
        _db.Tasks.AddRange(_nikkoTask, _victorTask);
        _db.SaveChanges();
    }

    private void AddVictorToNikkosList()
    {
        _db.ListMemberships.Add(new ListMembership { ListId = _nikkoList.Id, UserId = _victor.Id });
        _db.SaveChanges();
    }

    [Fact]
    public void HandoutScenario_VictorSeesBoth_NikkoSeesOnlyOwn()
    {
        AddVictorToNikkosList();
        Assert.Equal(new[] { "Nikko's Group List", "Victor's Group List" },
            _db.Lists.VisibleTo(_victor.Id).Select(l => l.Name).OrderBy(n => n).ToArray());
        Assert.Equal(new[] { "Nikko's Group List" },
            _db.Lists.VisibleTo(_nikko.Id).Select(l => l.Name).ToArray());
    }

    [Fact]
    public void NonMember_CannotReachListTask_MemberCan()
    {
        Assert.Null(_db.FindTask(_nikkoTask.Id, _victor.Id, _nikkoList.Id));
        AddVictorToNikkosList();
        Assert.NotNull(_db.FindTask(_nikkoTask.Id, _victor.Id, _nikkoList.Id));
    }

    [Fact]
    public void TaskIdFromAnotherList_IsRejected()
    {
        AddVictorToNikkosList();
        Assert.Null(_db.FindTask(_victorTask.Id, _victor.Id, _nikkoList.Id));   // wrong list for this task
        Assert.Null(_db.FindTask(_victorTask.Id, _nikko.Id, _victorList.Id));   // Nikko isn't a member
    }

    [Fact]
    public void ListTask_IsNotReachableThroughPersonalPath()
        => Assert.Null(_db.FindTask(_nikkoTask.Id, _nikko.Id, null));           // the ListId == null guard

    [Fact]
    public void RemovingMembership_RevokesAccess()
    {
        AddVictorToNikkosList();
        _db.ListMemberships.RemoveRange(_db.ListMemberships);
        _db.SaveChanges();
        Assert.Null(_db.FindTask(_nikkoTask.Id, _victor.Id, _nikkoList.Id));
    }

    [Fact]
    public void MemberIsNotOwner()
    {
        AddVictorToNikkosList();
        Assert.Null(_db.FindOwnedList(_nikkoList.Id, _victor.Id));
    }

    public void Dispose() { _db.Dispose(); _conn.Dispose(); }
}