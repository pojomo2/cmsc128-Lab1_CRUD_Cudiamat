using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TodoApp.Views;

public partial class ConfirmDialog : Window {
    public ConfirmDialog() {
        InitializeComponent();
    }

    public ConfirmDialog(string message) : this(){
        this.FindControl<TextBlock>("MessageText")!.Text = message;
        
    }

    private void YesButton_Click(object? sender, RoutedEventArgs e)
        => Close(true); //returns true to whoever opened the dialog
    
    private void NoButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(false); //returns false     
    }
}