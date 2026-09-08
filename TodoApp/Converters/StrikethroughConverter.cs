using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace TodoApp.Converters;

public class StrikethroughConverter : IValueConverter
{
    public static readonly StrikethroughConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isDone = value is bool b && b;
        return isDone ? TextDecorations.Strikethrough : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}