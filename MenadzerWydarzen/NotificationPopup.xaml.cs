using System;
using System.Windows;
using System.Windows.Threading;

namespace MenadzerWydarzen
{
    public partial class NotificationPopup : Window
    {
        private readonly DispatcherTimer _timer;

        public NotificationPopup(string eventName, TimeSpan startTime)
        {
            InitializeComponent();

            EventNameText.Text = eventName;
            TimeText.Text      = $"Godzina startu: {startTime:hh\\:mm}";

            Loaded += (_, _) =>
            {
                var w = SystemParameters.PrimaryScreenWidth;
                var h = SystemParameters.PrimaryScreenHeight;
                Left = w - Width - 20;
                Top  = h - Height - 60;
            };

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(8) };
            _timer.Tick += (_, _) => Close();
            _timer.Start();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
