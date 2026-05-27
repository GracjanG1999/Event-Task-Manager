using System;
using System.Windows;

namespace MenadzerWydarzen
{
    public partial class AddWindow : Window
    {
        public Wydarzenie? Result { get; private set; }

        public AddWindow()
        {
            InitializeComponent();
            DatePickerCtrl.SelectedDate = DateTime.Today;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text))
            {
                MessageBox.Show("Nazwa jest wymagana!", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameInput.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(StartTimeInput.Text) &&
                WydarzenieHelper.ParseTime(StartTimeInput.Text) == null)
            {
                MessageBox.Show("Nieprawidłowy format godziny startu.\nUżyj formatu HH:mm, np. 09:30",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                StartTimeInput.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(EndTimeInput.Text) &&
                WydarzenieHelper.ParseTime(EndTimeInput.Text) == null)
            {
                MessageBox.Show("Nieprawidłowy format godziny końca.\nUżyj formatu HH:mm, np. 17:00",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                EndTimeInput.Focus();
                return;
            }

            Result = new Wydarzenie
            {
                Name        = NameInput.Text.Trim(),
                PlannedDate = DatePickerCtrl.SelectedDate ?? DateTime.Today,
                StartTime   = WydarzenieHelper.ParseTime(StartTimeInput.Text),
                EndTime     = WydarzenieHelper.ParseTime(EndTimeInput.Text),
                Status      = (EventStatus)StatusCombo.SelectedIndex,
                Priority    = (EventPriority)PriorityCombo.SelectedIndex,
                Category    = WydarzenieHelper.NullIfBlank(CategoryInput.Text),
                Description = WydarzenieHelper.NullIfBlank(DescriptionInput.Text)
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
