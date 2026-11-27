# CMSC 128 Lab 1 — To-Do List (CRUD)

A desktop To-Do List app built with **C# and Avalonia UI**, using **SQLite** for persistent storage.

## Tech Stack

- **UI Framework:** [Avalonia UI](https://avaloniaui.net/) (MVVM pattern) — a cross-platform .NET UI framework, chosen so the app runs natively on Linux without needing Windows-only tooling.
- **MVVM Toolkit:** [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) — provides `[ObservableProperty]` and `[RelayCommand]` source generators to cut down on MVVM boilerplate.
- **Database:** SQLite, accessed through **Entity Framework Core** (`Microsoft.EntityFrameworkCore.Sqlite`).
  - Chosen because it's a single-file, zero-configuration database — appropriate for a small single-user to-do list, with no server setup required. The database file (`todos.db`) sits next to the app and persists automatically across restarts.
  - The schema is created automatically on first run via `EnsureCreated()` in the `TodoDbContext` — no manual migration step is needed to get started.

## Features Implemented

- Add / Edit / Delete / Mark-done tasks (Title, Due Date, Priority, Tag)
- Delete requires confirmation via a custom dialog window
- **Undo on Delete** — deleting a task shows an undo option for a few seconds before it's permanently removed from the database
- **Calendar View** — see which dates have tasks due, and manage tasks for a selected day

## How to Run Locally

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (latest version)
- Linux, Windows, or macOS

### Setup

```bash
# clone the repo
git clone https://github.com/pojomo2/cmsc128-Lab1_CRUD_Cudiamat.git
cd cmsc128-Lab1_CRUD_Cudiamat/TodoApp

# restore dependencies
dotnet restore

# run the app
dotnet run
```

No manual database setup is required — the SQLite database (`todos.db`) is created automatically in the project folder the first time the app runs.

### Troubleshooting (Linux)
If the window doesn't render correctly on some distros, try forcing software rendering:
```bash
dotnet run -- --avalonia-render-backend=software
```

## Data Operations (CRUD)

This app doesn't expose REST endpoints — CRUD operations happen directly against the local SQLite database via Entity Framework Core, called from the `MainWindowViewModel`. Rough equivalents:

| Operation | Where it happens | What it does |
|---|---|---|
| **Create** | `AddTask()` in `MainWindowViewModel.cs` | Builds a new `TodoItem`, adds it to `_db.Tasks`, calls `_db.SaveChanges()` to write it to `todos.db` |
| **Read** | `Load()` in `MainWindowViewModel.cs` | Queries `_db.Tasks.OrderBy(...)` on startup and populates the on-screen `ObservableCollection<TodoItem>` |
| **Update** | `ToggleDone()` in `MainWindowViewModel.cs` | Flips `IsDone` on an existing tracked `TodoItem`, then `_db.SaveChanges()` persists the change |
| **Delete** | `DeleteTask()` in `MainWindowViewModel.cs` | Shows a confirmation dialog; on confirm, removes the task from the UI list and starts a 5-second undo timer before actually removing it from `_db` via `_db.SaveChanges()` |

## Project Structure

```
TodoApp/
├── Models/
│   └── TodoItem.cs           # data model — one task record
├── Data/
│   └── TodoDbContext.cs      # EF Core context — persistence layer (SQLite)
├── Converters/
│   └── StrikethroughConverter.cs  # bool -> TextDecorations for marking done tasks
├── ViewModels/
│   ├── ViewModelBase.cs
│   └── MainWindowViewModel.cs # CRUD logic, commands, undo timer
├── Views/
│   ├── MainWindow.axaml       # main UI (task list + add form)
│   ├── MainWindow.axaml.cs
│   ├── ConfirmDialog.axaml    # delete confirmation dialog
│   └── ConfirmDialog.axaml.cs
├── App.axaml.cs
└── Program.cs
```

## Screenshots


| ![SS1](TodoApp/Assets/Screenshot_20260910_231046.png) | ![SS2](TodoApp/Assets/Screenshot_20260910_231121.png) | ![SS3](TodoApp/Assets/Screenshot_20260910_231138.png) |

| ![SS4](TodoApp/Assets/Screenshot_20260911_115700.png) | ![SS5](TodoApp/Assets/Screenshot_20260911_120217.png) |

Activities 1 and 2: Merge Strategy
Activity 1 and Activity 2 were combined into the Lab 3 application using [describe your actual branch and merge strategy, such as merging the Activity 2 branch into the Lab 3 branch]. Conflicts were resolved by retaining the existing registration, login, password-hashing, and session-handling behavior from Activity 2 while integrating the task-management and collaborative-list features. The merged application was then built and tested.

Database Schema
The application uses SQLite with Entity Framework Core.

Users stores account information. Email addresses have a unique index.
Tasks stores task data. Each task has a required UserId foreign key to its creator. A task may have a nullable ListId: NULL represents a personal task, while a value associates it with a collaborative list.
Lists stores collaborative lists and their OwnerId.
ListMemberships associates users with lists. Its composite key is (ListId, UserId).
Sessions stores sessions associated with users through UserId.
A user can see a collaborative list if they own it or have a corresponding membership record. Personal tasks remain restricted to their owner.

Ownership and Access Checks
This is an Avalonia desktop application, not an HTTP API, so it has no REST endpoints. Access checks are performed in the application’s data-access operations before task or list data is returned or changed.

Examples include:

Listing available lists filters them to lists owned by the signed-in user or lists they belong to.
Loading tasks filters personal tasks by their owner and collaborative-list tasks by list access.
Looking up a task for an operation checks its ID together with the signed-in user and, for shared tasks, the selected accessible list.
Adding a member is restricted to the collaborative-list owner.
Task operations should repeat the access check before updating or deleting data; the UI alone is not treated as an authorization boundary.
Installation and Local Run
Requirements: the .NET SDK version targeted by TodoApp.csproj and the EF Core command-line tool if migrations are not applied automatically.

From the repository root:


If the application does not apply migrations at startup, update the local database:


Run the app:


Run the tests from the repository root:


The SQLite database is stored at the path configured by AppPaths.DatabasePath.