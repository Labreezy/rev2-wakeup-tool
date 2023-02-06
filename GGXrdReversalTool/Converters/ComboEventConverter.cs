using System;
using System.Globalization;
using System.Windows.Data;

namespace GGXrdReversalTool.Converters;

public class ComboEventConverter : IMultiValueConverter 
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var min = (int)values[0];
        var max = (int)values[1];

        return min == max ? $"Dummy gets hit by {min} hits combo" : $"Dummy gets hit by a combo from {min} hit up to {max} hits";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}