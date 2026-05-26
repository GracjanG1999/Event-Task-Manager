using System;
using System.Globalization;
using System.Windows.Data;

namespace MenadzerWydarzen
{
    public class PriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EventPriority priority)
            {
                return priority switch
                {
                    EventPriority.Low      => "Niski",
                    EventPriority.Medium   => "Średni",
                    EventPriority.High     => "Wysoki",
                    EventPriority.Critical => "Krytyczny",
                    _                      => "Nieznany"
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
