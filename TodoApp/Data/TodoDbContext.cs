//EF Core context (the persistence layer)
//this is where data persists 
//SQLite file todos.db sits next to the executable and survives restarts
using Microsoft.EntityFrameworkCore;

public class TodoDbContext : DbContext {
    public DbSet<TodoItem> Tasks => Set<TodoItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
         options.UseSqlite("Data Source=todos.db");       
    }
    
}

