using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GGXrdReversalTool.Converters;

public class AnimationEventConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var shouldCheckWakingUp = (bool)values[0];
        var shouldCheckWallSplat = (bool)values[1];
        var shouldCheckAirTech = (bool)values[2];


        if (!shouldCheckWakingUp && !shouldCheckWallSplat && !shouldCheckAirTech)
        {
            //TODO implement new property : actionInvalid
            return "!!!! check nothing";
        }



        var events = new List<string>()
        {
            shouldCheckWakingUp ? "wakes up" : "",
            shouldCheckWallSplat ? "recovers from wall splat" : "",
            shouldCheckAirTech ? "recovers from air tech" : ""
        };

        var result = "Dummy ";


        result += events.Where(evt => !string.IsNullOrEmpty(evt)).Aggregate((a, b) => $"{a} or {b}");
        

        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}