//EF Core context (the persistence layer)
//this is where data persists 
using System;
using Microsoft.EntityFrameworkCore;



public class TodoDbContext : DbContext {
    public DbSet<TodoItem> Tasks => Set<TodoItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<TodoList> Lists => Set<TodoList>();
    public DbSet<ListMembership> ListMemberships => Set<ListMembership>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
         options.UseSqlite($"Data Source={AppPaths.DatabasePath}");       
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();

        modelBuilder.Entity<Session>().HasOne(session => session.User).WithMany(user => user.Sessions).HasForeignKey(session => session.UserId);

        modelBuilder.Entity<TodoItem>().HasOne(task => task.User).WithMany(user => user.Tasks).HasForeignKey(task => task.UserId);
        
        modelBuilder.Entity<TodoList>().HasOne(l => l.Owner).WithMany(u => u.OwnedLists).HasForeignKey(l => l.OwnerId);

        modelBuilder.Entity<ListMembership>().HasKey(m => new { m.ListId, m.UserId });
        modelBuilder.Entity<ListMembership>()
            .HasOne(m => m.List).WithMany(l => l.Memberships).HasForeignKey(m => m.ListId);
        modelBuilder.Entity<ListMembership>()
            .HasOne(m => m.User).WithMany(u => u.Memberships).HasForeignKey(m => m.UserId);

        modelBuilder.Entity<TodoItem>()
            .HasOne(t => t.List).WithMany(l => l.Tasks).HasForeignKey(t => t.ListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}

