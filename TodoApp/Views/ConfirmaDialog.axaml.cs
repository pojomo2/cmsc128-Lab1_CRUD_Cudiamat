using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TodoApp.View;

public partial class ConfirmDialog : Window {
    public ConfirmDialog() {
        InitializeComponent();
    }

    public ConfirmDialog(string message) : this(){
        this.FindControl<TextBlock>("MessageText")!.Text = message;
        
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
        => Close(true); //returns true to whoever opened the dialog
    
    private void OnCancelClick(object? sender, RoutedEventArgs e)
        => Close(false); //returns false
}