//EF Core context (the persistence layer)
//this is where data persists 
//SQLite file todos.db sits next to the executable and survives restarts
using System;
using Microsoft.EntityFrameworkCore;

public class TodoDbContext : DbContext {
    public DbSet<TodoItem> Tasks => Set<TodoItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
         options.UseSqlite($"Data Source={AppPaths.DatabasePath}");       
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();

        modelBuilder.Entity<Session>().HasOne(session => session.User).WithMany().HasForeignKey(session => session.UserId);

        modelBuilder.Entity<TodoItem>().HasOne(task => task.User).WithMany().HasForeignKey(task => task.UserId);
        
 
    }
    
}

