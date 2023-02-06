using System;
using System.Globalization;
using System.Windows.Data;

namespace GGXrdReversalTool.Converters;


[ValueConversion(typeof(int), typeof(string))]
public class PercentageFrequencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int percentage = (int)value;

        return percentage switch
        {
            <= 0 => "It will never be triggered",
            >= 100 => "It will always be triggered",
            _ => $"It will be triggered {percentage}% of the time"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}