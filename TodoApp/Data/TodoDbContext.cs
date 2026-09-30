//EF Core context (the persistence layer)
//this is where data persists 
//SQLite file todos.db sits next to the executable and survives restarts
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

public class TodoDbContext : DbContext {
    public DbSet<TodoItem> Tasks => Set<TodoItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
         options.UseSqlite("Data Source=todos.db");       
    }
    
}

