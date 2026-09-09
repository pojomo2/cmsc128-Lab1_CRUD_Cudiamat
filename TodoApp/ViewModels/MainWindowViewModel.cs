using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace TodoApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    private readonly TodoDbContext _db = new();

    public ObservableCollection<TodoItem> Tasks {get;} = new();
    //ObservableCollection means Avalonia constantly monitors it

    private TodoItem? _lastDeletedTask;
    //'?' eans the TodoItem can be null when no task is currently stored
    //These things need to be declared at the top of the class to be accessible by methods within the class

    private readonly DispatcherTimer _undoTimer;
    


    [ObservableProperty] private string newTitle = "";
    [ObservableProperty] private DateTimeOffset newDueDate = DateTimeOffset.Now;
    [ObservableProperty] private string newPriority = "Low";
    [ObservableProperty] private string newTag = "Personal";
    [ObservableProperty] private bool _isUndoBannerVisible;
    [ObservableProperty] private TodoItem? _selectedTask;



    public MainWindowViewModel() {
        _db.Database.EnsureCreated();
        Load();



        _undoTimer = new DispatcherTimer 
        {
            Interval = TimeSpan.FromSeconds(5),
        };
        _undoTimer.Tick += OnUndoTimerTick;

    }

    public string[] PriorityOptions {get;} = ["Low", "Med", "High"];
    
    public string[] TagOptions {get;} = ["School", "Personal", "Others"];



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
    private void ToggleDone(TodoItem item) 
    {
        _db.SaveChanges(); //"Update"
    }


    [RelayCommand]
    private void EditTask(TodoItem item)
    {
        SelectedTask = item;
        NewTitle = item.Title;
        NewDueDate = item.DueDate;
        NewPriority = item.Priority;
        NewTag = item.Tag;
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
                    _lastDeletedTask = item;
                    Tasks.Remove(item);
                    IsUndoBannerVisible = true;
                    _undoTimer.Start();
                }
            }
        }
    }

        private void OnUndoTimerTick(object? sender, EventArgs e)
    {
        _undoTimer.Stop();
        IsUndoBannerVisible = false;

        if(_lastDeletedTask != null)
        {
            _db.Tasks.Remove(_lastDeletedTask);
            _db.SaveChanges();
            _lastDeletedTask = null; //clear the cached ref
        }
    }

    [RelayCommand]
    private void Undo()
    {
        if(_lastDeletedTask != null)
        {
            _undoTimer.Stop();
            Tasks.Add(_lastDeletedTask);
            IsUndoBannerVisible = false;
            _lastDeletedTask = null;
        }
    }

    [RelayCommand] 
    private void SaveEdits()
        {
            if(SelectedTask == null) return;

            SelectedTask.Title = NewTitle;
            SelectedTask.DueDate = NewDueDate.DateTime;
            SelectedTask.Priority = NewPriority;
            SelectedTask.Tag = NewTag;

              
            _db.SaveChanges();
            Load();
            SelectedTask = null;

            NewTitle="";
            NewPriority="Low";
            NewTag="Others";
        }

    
}

//Tasks in ObservableCollection is DIFFERENT from _db.Tasks, the former is driving the UI, the latter is the actualy table

