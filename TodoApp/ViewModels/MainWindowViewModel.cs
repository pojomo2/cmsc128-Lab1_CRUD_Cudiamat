using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TodoApp.ViewModels;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Avalonia.Controls;
using TodoApp.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace TodoApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly TodoDbContext _db = new();

    public ObservableCollection<TodoItem> Tasks {get;} = new();

    [ObservableProperty] private string newTitle = "";
    [ObservableProperty] private DateTimeOffset newDueDate = DateTimeOffset.Now;
    [ObservableProperty] private string newPriority = "Low";
    [ObservableProperty] private string newTag = "Others";

    public MainWindowViewModel() {
        _db.Database.EnsureCreated();
        Load();
    }

    public string[] PriorityOptions {get;} = ["Low", "Med", "High"];

    private void Load() {
        Tasks.Clear();
        foreach (var t in _db.Tasks.OrderBy(t => t.DueDate))
            Tasks.Add(t);
    }

    [RelayCommand]
    private void AddTask() {
        var item = new TodoItem {
            Title = NewTitle,
            DueDate = NewDueDate.DateTime,
            Priority = NewPriority,
            Tag = NewTag
        };

        _db.Tasks.Add(item);
        _db.SaveChanges(); //this is the "Create" write to disk
        Tasks.Add(item);
        NewTitle = "";
    }

    [RelayCommand]
    private void ToggleDone(TodoItem item) {
        item.IsDone = !item.IsDone;
        _db.SaveChanges(); //"Update"
    }

 

    [RelayCommand]
    private async Task DeleteTask(TodoItem item)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var ownerWindow = desktop.MainWindow;
            
            if(ownerWindow != null)
            {
                var dialog = new ConfirmDialog($"Are you sure you want to delete '{item.Title}'?");
                var result = await dialog.ShowDialog<bool>(ownerWindow);

                if(result)
                {
                    _db.Tasks.Remove(item);
                    _db.SaveChanges(); //"Delete",, save the delete
                    Tasks.Remove(item);
                }
            }
        }
    }

 
}

//Tasks in ObservableCollection is DIFFERENT from _db.Tasks, the former is driving the UI, the latter is the actualy table

