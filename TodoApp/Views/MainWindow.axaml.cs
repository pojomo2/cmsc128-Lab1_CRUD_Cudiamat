using Avalonia.Controls;
using Avalonia.Interactivity;
using TodoApp.ViewModels;
namespace TodoApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnDeleteClick(object? sender, RoutedEventArgs e)    {
        if (sender is Button {DataContext: TodoItem item})
        {
            var dialog = new ConfirmDialog($"Delete '{item.Title}'?");
            var result = await dialog.ShowDialog<bool>(this);
            if (result && DataContext is MainWindowViewModel vm)
            {
                vm.DeleteTaskCommand.Execute(item);
            } 
        }
    }
}