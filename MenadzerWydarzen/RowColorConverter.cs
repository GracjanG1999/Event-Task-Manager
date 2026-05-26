using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MenadzerWydarzen
{
    public class RowColorConverter : IMultiValueConverter
    {
        private static readonly SolidColorBrush DoneColor     = new(Color.FromRgb(190, 235, 190));
        private static readonly SolidColorBrush OverdueColor  = new(Color.FromRgb(255, 190, 190));
        private static readonly SolidColorBrush ActiveColor   = new(Color.FromRgb(255, 255, 190));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return Brushes.White;

            var status = values[0] is EventStatus s ? s : EventStatus.NotDone;
            var date   = values[1] is DateTime dt ? (DateTime?)dt : null;

            if (status == EventStatus.Done)
                return DoneColor;

            if (date.HasValue && date.Value.Date < DateTime.Today)
                return OverdueColor;

            if (status == EventStatus.InProgress)
                return ActiveColor;

            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
