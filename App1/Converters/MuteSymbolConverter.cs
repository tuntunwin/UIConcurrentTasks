using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace App1
{
    /// <summary>
    /// Converts a boolean mute state to a Symbol for UI display
    /// </summary>
    public class MuteSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isMuted)
            {
                return isMuted ? Symbol.Mute : Symbol.Volume;
            }
            
            return Symbol.Volume;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}