using System;
using System.IO;


public static class AppPaths 
{
    public static string AppDataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TodoApp"
    );

    public static string DatabasePath { get; } = Path.Combine(AppDataDirectory, "todos.db");

    public static string SessionPath { get; } = Path.Combine(AppDataDirectory, "session.txt");

    static AppPaths()
    {
        Directory.CreateDirectory(AppDataDirectory);
    }
}