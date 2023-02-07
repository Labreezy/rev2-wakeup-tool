using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Converters;

[ValueConversion(typeof(string),typeof(IEnumerable<string>))]
public class InputConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        SlotInput slotInput = new SlotInput(value?.ToString() ?? "");

        if (!slotInput.IsValid)
        {
            return Enumerable.Empty<string>();
        }

        return slotInput.InputSplit;
        
        var result = slotInput.InputSplit.SelectMany(x => x.Select(p => p))
            .Where(c => c != '!')
            .Where(c => c is '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' or 'P' or 'K' or 'S' or 'H' or 'D')
            .Select(c=> c.ToString())
            ;

        return result;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}