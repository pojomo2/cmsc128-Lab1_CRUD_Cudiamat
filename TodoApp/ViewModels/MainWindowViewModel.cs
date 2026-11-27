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
using Org.BouncyCastle.Asn1.Esf;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.X509;

namespace TodoApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    

    public AuthViewModel Auth { get; }

    
    public MainWindowViewModel()
    {

        Auth = new AuthViewModel();


        _undoTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _undoTimer.Tick += OnUndoTimerTick;

        Auth.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AuthViewModel.CurrentUser))
            {

                ResetTransientState();
                RefreshLists();
                IsProfileOpen = false;
                OnPropertyChanged(nameof(IsTasksVisible));
                OnPropertyChanged(nameof(IsListManagementVisible)); 

            }              
        };

        RefreshLists();

    }  


    private readonly TodoDbContext _db = new();

    public ObservableCollection<TodoItem> Tasks {get;} = new();
    //ObservableCollection means Avalonia constantly monitors it

    private TodoItem? _lastDeletedTask;
    //'?' eans the TodoItem can be null when no task is currently stored
    //These things need to be declared at the top of the class to be accessible by methods within the class

    private TodoItem? FindAccessibleTask(int taskId, int userId) => 
        _db.Tasks.SingleOrDefault(task => 
            task.Id == taskId && 
            (
                //Personal task: only its creator can access it
                (task.ListId == null && task.UserId == userId)

                //List task: only the list owner or a current member can access it
                || (task.ListId != null &&
                    _db.Lists.Any(list =>
                        list.Id == task.ListId && 
                        (list.OwnerId == userId ||
                        list.Memberships.Any(member => member.UserId == userId))))
            ));

    private readonly DispatcherTimer _undoTimer;
    


    [ObservableProperty] private string newTitle = "";
    [ObservableProperty] private DateTimeOffset newDueDate = DateTimeOffset.Now;
    [ObservableProperty] private string newPriority = "Low";
    [ObservableProperty] private string newTag = "Personal";
    [ObservableProperty] private bool _isUndoBannerVisible;
    [ObservableProperty] private TodoItem? _selectedTask;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTasksVisible))]
    [NotifyPropertyChangedFor(nameof(IsListManagementVisible))]
    private bool _isProfileOpen;

    public bool IsTasksVisible =>
    Auth.CurrentUser is not null && !IsProfileOpen;
    public bool IsListManagementVisible => IsTasksVisible && CanManageList;



    public string[] PriorityOptions {get;} = ["Low", "Med", "High"];
    
    public string[] TagOptions {get;} = ["School", "Personal", "Others"];



    private void Load() {
        Tasks.Clear();

        if (Auth.CurrentUser is not { } user)
            return;

        foreach (var t in _db.TasksFor(user.Id, CurrentListId).OrderBy(t => t.DueDate))
            Tasks.Add(t);
    }

    [RelayCommand]
    private void AddTask() {
        if (Auth.CurrentUser is not { } user)
            return;

        
        if (CurrentListId is int activeList && _db.FindVisibleList(activeList, user.Id) is null)
            return;

        var item = new TodoItem 
        {
            Title = NewTitle,
            DueDate = NewDueDate.DateTime,
            Priority = NewPriority,
            UserId = user.Id,
            Tag = NewTag,
            ListId = CurrentListId

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

        var ownedTask = FindAccessibleTask(item.Id, user.Id);
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

        var ownedTask = FindAccessibleTask(item.Id, user.Id);
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

        var ownedTask = FindAccessibleTask(item.Id, user.Id);
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

        ownedTask = FindAccessibleTask(item.Id, user.Id);
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

        var ownedTask = FindAccessibleTask(pendingTask.Id, user.Id);
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

        var ownedTask = FindAccessibleTask(pendingTask.Id, user.Id);
        if(ownedTask is not null && !Tasks.Any(task => task.Id == ownedTask.Id))
            Tasks.Add(ownedTask);
        
        _lastDeletedTask = null;
    }

    [RelayCommand] 
    private void SaveEdits()
        {

            if (Auth.CurrentUser is not { } user || SelectedTask is not { } selected) return;

            var ownedTask = FindAccessibleTask(selected.Id, user.Id); 
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


    /////////////////////////////////////////////
    public record ListChoice(int? Id, string Name, bool IsOwner); // Id == null => Personal

    public ObservableCollection<ListChoice> ListChoices { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanManageList))]
    [NotifyPropertyChangedFor(nameof(IsListManagementVisible))]
    private ListChoice? _selectedList;

    [ObservableProperty] private string _newListName = "";
    [ObservableProperty] private string _memberEmail = "";
    [ObservableProperty] private string _listMessage = "";

    private int? CurrentListId => SelectedList?.Id;                 // null => Personal (safe default)
    public bool CanManageList => SelectedList is { Id: not null, IsOwner: true };   // UI convenience only
    private bool _refreshingLists;

    partial void OnSelectedListChanged(ListChoice? value)
    {
        if (_refreshingLists) return;
        ResetTransientState();  // same policy as account change: cancel pending delete
        Load();
    }

    private void ResetTransientState()
    {
        _undoTimer.Stop();
        IsUndoBannerVisible = false;
        _lastDeletedTask = null;
        SelectedTask = null;
        NewTitle = "";
        NewDueDate = DateTimeOffset.Now;
        NewPriority = "Low";
        NewTag = "Personal";
        MemberEmail = "";
        ListMessage = "";
    }

    private void RefreshLists(int? keepSelectedId = null)
    {
        _refreshingLists = true;
        try
        {
            ListChoices.Clear();
            ListChoices.Add(new ListChoice(null, "Personal", true));

            if (Auth.CurrentUser is { } user)
                foreach (var l in _db.Lists.VisibleTo(user.Id).Include(l => l.Owner).OrderBy(l => l.Name))
                {
                    var mine = l.OwnerId == user.Id;
                    ListChoices.Add(new ListChoice(l.Id, mine ? l.Name : $"{l.Name} (shared by {l.Owner.DisplayName})", mine));
                }

                SelectedList = ListChoices.FirstOrDefault(c => c.Id == keepSelectedId) ?? ListChoices[0];
        }
        finally { _refreshingLists = false; }

        Load();

    }


    [RelayCommand]
    private void CreateList()
    {
        if (Auth.CurrentUser is not { } user) return;

        var name = NewListName.Trim();
        if (name.Length == 0)
        {
            ListMessage = "Enter a list name.";
            return;
        }

        var list = new TodoList { Name = name, OwnerId = user.Id };
        _db.Lists.Add(list);
        _db.SaveChanges();

        NewListName = "";
        ResetTransientState();
        RefreshLists(list.Id);

    }

    [RelayCommand]
    private void AddMember()
    {
        if (Auth.CurrentUser is not { } user || CurrentListId is not int listId) return;

        if(_db.FindOwnedList(listId, user.Id) is null)
        {
            ListMessage = "Only the list owner can add members.";
            return;
        }

        var email = AuthViewModel.NormalizeEmail(MemberEmail);
        var target = _db.Users.SingleOrDefault(u => u.Email == email);
        if (target is null) { ListMessage = "No account with that email."; return;  }
        if (target.Id == user.Id) { ListMessage = "You already own this list."; return; }
        if (_db.ListMemberships.Any(m => m.ListId == listId && m.UserId == target.Id)) { ListMessage = "Already a member."; return; }

        _db.ListMemberships.Add(new ListMembership { ListId = listId, UserId = target.Id });
        _db.SaveChanges();
        MemberEmail = "";
        ListMessage = $"Added {target.DisplayName}.";
    }
}



//Tasks in ObservableCollection is DIFFERENT from _db.Tasks, the former is driving the UI, the latter is the actualy table

