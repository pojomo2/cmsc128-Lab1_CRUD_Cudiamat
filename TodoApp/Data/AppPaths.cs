using System;
using System.IO;
using TodoApp;

var appData = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "TodoApp"); 

Directory.CreateDirectory(appData);

var databasePath = Path.Combine(appData, "todos.db");
var sessionPath = Path.Combine(appData, "session.txt");