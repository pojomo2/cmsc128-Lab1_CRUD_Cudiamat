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
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Asn1.Misc;

namespace TodoApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    

    public AuthViewModel Auth { get; }

    
    public MainWindowViewModel()
    {

        Auth = new AuthViewModel();
        Load();


        OnPropertyChanged(nameof(IsTasksVisible));

        _undoTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _undoTimer.Tick += OnUndoTimerTick;

        Auth.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AuthViewModel.CurrentUser))
            {

                // Cancel any pending deletion when logging out or switching accounts
                _undoTimer.Stop();
                IsUndoBannerVisible = false;
                _lastDeletedTask = null;

                SelectedTask = null;
                NewTitle = "";
                NewDueDate = DateTimeOffset.Now;
                NewPriority = "Low";
                NewTag = "Personal";

                Load();

            }
                
        };

    }  


    private readonly TodoDbContext _db = new();

    public ObservableCollection<TodoItem> Tasks {get;} = new();
    //ObservableCollection means Avalonia constantly monitors it

    private TodoItem? _lastDeletedTask;
    //'?' eans the TodoItem can be null when no task is currently stored
    //These things need to be declared at the top of the class to be accessible by methods within the class

    private TodoItem? FindOwnedTask(int taskId, int userId) => 
        _db.Tasks.SingleOrDefault(task => task.Id == taskId && task.UserId == userId);

    private readonly DispatcherTimer _undoTimer;
    


    [ObservableProperty] private string newTitle = "";
    [ObservableProperty] private DateTimeOffset newDueDate = DateTimeOffset.Now;
    [ObservableProperty] private string newPriority = "Low";
    [ObservableProperty] private string newTag = "Personal";
    [ObservableProperty] private bool _isUndoBannerVisible;
    [ObservableProperty] private TodoItem? _selectedTask;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTasksVisible))]
    private bool _isProfileOpen;

    public bool IsTasksVisible =>
    Auth.CurrentUser is not null && !IsProfileOpen;



    public string[] PriorityOptions {get;} = ["Low", "Med", "High"];
    
    public string[] TagOptions {get;} = ["School", "Personal", "Others"];



    private void Load() {
        Tasks.Clear();

        if (Auth.CurrentUser is not { } user)
            return;

        foreach (var t in _db.Tasks.Where(task => task.UserId == user.Id).OrderBy(t => t.DueDate))
            Tasks.Add(t);
    }

    [RelayCommand]
    private void AddTask() {
        if (Auth.CurrentUser is not { } user)
            return;

        
        var item = new TodoItem {
            Title = NewTitle,
            DueDate = NewDueDate.DateTime,
            Priority = NewPriority,
            UserId = user.Id,
            Tag = NewTag
        };

        if (_lastDeletedTask != null) {
            return; 
        }

        _db.Tasks.Add(item);
        _db.SaveChanges(); //this is the "Create" write to disk
        Tasks.Add(item);
        NewTitle = "";
    }

    [RelayCommand]
    private void ToggleDone(TodoItem item)
    {
        if (Auth.CurrentUser is not { } user)
            return;

        var ownedTask = FindOwnedTask(item.Id, user.Id);
        if (ownedTask is null)
            return;

        ownedTask.IsDone = item.IsDone;
        _db.SaveChanges();
    }


    [RelayCommand]
    private void EditTask(TodoItem item)
    {
        if (Auth.CurrentUser is not { } user)
            return;

        var ownedTask = FindOwnedTask(item.Id, user.Id);
        if (ownedTask is null)
            return;

        SelectedTask = ownedTask;
        NewTitle = ownedTask.Title;
        NewDueDate = ownedTask.DueDate;
        NewPriority = ownedTask.Priority;
        NewTag = ownedTask.Tag;
    }
 

    [RelayCommand]
    private async Task DeleteTask(TodoItem item)
    {

        if (_lastDeletedTask is not null) return;

        if (Auth.CurrentUser is not { } user)
            return;

        var ownedTask = FindOwnedTask(item.Id, user.Id);
        if (ownedTask is null)
            return;

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow is not { } ownerWindow)
        {
            return;
        }

        var dialog = new ConfirmDialog($"Are you sure you want to delete '{ownedTask.Title}'?");
        var confirmed = await dialog.ShowDialog<bool>(ownerWindow);

        // The account could have changed while the confirmation dialog was open.
        if (!confirmed || Auth.CurrentUser?.Id != user.Id)
            return;

        ownedTask = FindOwnedTask(item.Id, user.Id);
        if (ownedTask is null)
            return;

        _lastDeletedTask = ownedTask;

        var displayedTask = Tasks.FirstOrDefault(task => task.Id == ownedTask.Id);
        if (displayedTask is not null)
            Tasks.Remove(displayedTask);

        IsUndoBannerVisible = true;
        _undoTimer.Start();
    }

    private void OnUndoTimerTick(object? sender, EventArgs e)
    {
        _undoTimer.Stop();
        IsUndoBannerVisible = false;

        if (_lastDeletedTask is not { } pendingTask
            || Auth.CurrentUser is not { } user)
        {
            _lastDeletedTask = null;
            return;
        }

        var ownedTask = FindOwnedTask(pendingTask.Id, user.Id);
        if (ownedTask is not null)
        {
            _db.Tasks.Remove(ownedTask);
            _db.SaveChanges();
        }

        _lastDeletedTask = null;
    }

    [RelayCommand]
    private void Undo()
    {
        _undoTimer.Stop();
        IsUndoBannerVisible = false;

        if (_lastDeletedTask is not { } pendingTask 
            || Auth.CurrentUser is not { } user)
        {
            _lastDeletedTask = null;
            return;
        }

        var ownedTask = FindOwnedTask(pendingTask.Id, user.Id);
        if(ownedTask is not null && !Tasks.Any(task => task.Id == ownedTask.Id))
            Tasks.Add(ownedTask);
        
        _lastDeletedTask = null;
    }

    [RelayCommand] 
    private void SaveEdits()
        {

            if (Auth.CurrentUser is not { } user || SelectedTask is not { } selected) return;

            var ownedTask = FindOwnedTask(selected.Id, user.Id); 
            if (ownedTask is null) return;

            ownedTask.Title = NewTitle;
            ownedTask.DueDate = NewDueDate.DateTime;
            ownedTask.Priority = NewPriority;
            ownedTask.Tag = NewTag;

              
            _db.SaveChanges();
            Load();

            SelectedTask = null;
            NewTitle="";
            NewDueDate = DateTimeOffset.Now;
            NewPriority="Low";
            NewTag="Personal";
        }

    [RelayCommand]
    private void ShowProfile() => IsProfileOpen = true;

    [RelayCommand]
    private void ShowTasks() => IsProfileOpen = false;


}



//Tasks in ObservableCollection is DIFFERENT from _db.Tasks, the former is driving the UI, the latter is the actualy table

