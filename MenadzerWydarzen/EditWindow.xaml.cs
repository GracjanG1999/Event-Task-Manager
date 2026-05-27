using System;
using System.Windows;

namespace MenadzerWydarzen
{
    public partial class EditWindow : Window
    {
        public Wydarzenie Wydarzenie { get; private set; }

        public EditWindow(Wydarzenie wydarzenie)
        {
            InitializeComponent();
            Wydarzenie = wydarzenie;

            NameInput.Text            = wydarzenie.Name;
            DatePickerCtrl.SelectedDate = wydarzenie.PlannedDate;
            StartTimeInput.Text       = wydarzenie.StartTime.HasValue
                ? $"{wydarzenie.StartTime.Value:hh\\:mm}" : "";
            EndTimeInput.Text         = wydarzenie.EndTime.HasValue
                ? $"{wydarzenie.EndTime.Value:hh\\:mm}" : "";
            StatusCombo.SelectedIndex   = (int)wydarzenie.Status;
            PriorityCombo.SelectedIndex = (int)wydarzenie.Priority;
            CategoryInput.Text          = wydarzenie.Category ?? "";
            DescriptionInput.Text       = wydarzenie.Description ?? "";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
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

            Wydarzenie.Name        = NameInput.Text.Trim();
            Wydarzenie.PlannedDate = DatePickerCtrl.SelectedDate;
            Wydarzenie.StartTime   = ParseTime(StartTimeInput.Text);
            Wydarzenie.EndTime     = ParseTime(EndTimeInput.Text);
            Wydarzenie.Status      = (EventStatus)StatusCombo.SelectedIndex;
            Wydarzenie.Priority    = (EventPriority)PriorityCombo.SelectedIndex;
            Wydarzenie.Category    = WydarzenieHelper.NullIfBlank(CategoryInput.Text);
            Wydarzenie.Description = WydarzenieHelper.NullIfBlank(DescriptionInput.Text);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static TimeSpan? ParseTime(string text) => WydarzenieHelper.ParseTime(text);
    }
}
