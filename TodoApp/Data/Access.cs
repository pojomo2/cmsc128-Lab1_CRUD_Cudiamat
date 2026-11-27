using System.Linq;

public static class Access
{
    public static IQueryable<TodoList> VisibleTo(this IQueryable<TodoList> lists, int userId) => lists.Where(l => l.OwnerId == userId || l.Memberships.Any(m => m.UserId == userId));

    public static TodoList? FindVisibleList(this TodoDbContext db, int listId, int userId) => db.Lists.VisibleTo(userId).SingleOrDefault(l => l.Id == listId);

    public static TodoList? FindOwnedList(this TodoDbContext db, int listId, int userId) => db.Lists.SingleOrDefault(l => l.Id == listId && l.OwnerId == userId);

    //listId == null means the Personal list
    public static IQueryable<TodoItem> TasksFor(this TodoDbContext db, int userId, int? listId) =>
        listId is null
            ? db.Tasks.Where(t => t.UserId == userId && t.ListId == null)
            : db.Tasks.Where(t => t.ListId == listId &&
                  (t.List!.OwnerId == userId || t.List.Memberships.Any(m => m.UserId == userId)));
    
    public static TodoItem? FindTask(this TodoDbContext db, int taskId, int userId, int? listId) => db.TasksFor(userId, listId).SingleOrDefault(t => t.Id == taskId);

}