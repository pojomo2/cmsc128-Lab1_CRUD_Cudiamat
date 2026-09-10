using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace TodoApp.Converters;

public class StrikethroughConverter : IValueConverter //IValueConverter is a contract requiring both Convert and ConvertBack to exist
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        if (value is bool isDone && isDone)
        {
            var decoration = new TextDecoration
            {
                Location = TextDecorationLocation.Strikethrough,
                Stroke = Brushes.Gray,
                StrokeThickness = 1.5
            };
            return new TextDecorationCollection { decoration };
        }
        
        return null; //no decoration when unchecked

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}