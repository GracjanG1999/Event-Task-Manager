using System;
using System.Globalization;
using System.Windows.Data;

namespace MenadzerWydarzen
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EventStatus status)
            {
                return status switch
                {
                    EventStatus.NotDone => "Oczekujące",
                    EventStatus.InProgress => "Trwa",
                    EventStatus.Done => "Zrealizowane",
                    _ => "Nieznane"
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}