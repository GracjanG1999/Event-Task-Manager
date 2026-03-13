using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MenadzerWydarzen
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context = new AppDbContext();

        public MainWindow()
        {
            InitializeComponent();
            PlannedDatePicker.SelectedDate = DateTime.Today;
            UpdateSummary();
            RefreshList();
        }

        private void RefreshList()
        {
            EventsGrid.ItemsSource = _context.Wydarzenia.ToList();
        }

        private TimeSpan? ParseTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            string clean = text.Replace(":", "").Trim();
            if (clean.Length <= 2 && int.TryParse(clean, out int hour)) return new TimeSpan(hour, 0, 0);
            if (clean.Length == 4 && int.TryParse(clean.Substring(0, 2), out int h) && int.TryParse(clean.Substring(2, 2), out int m)) return new TimeSpan(h, m, 0);
            return null;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text)) 
            {
                MessageBox.Show("Brak nazwy wydarzenia, proszę wpisać nazwę wydarzenia!");
                return;
            }

            var selectedItem = StatusCombo.SelectedItem as ComboBoxItem; 
            var statusString = selectedItem?.Content.ToString() ?? "NotDone";

            EventStatus SatusEvent = statusString switch
            {
                "Oczekujące" => EventStatus.NotDone,
                "Trwa" => EventStatus.InProgress,
                "Zrealizowane" => EventStatus.Done,
                _ => EventStatus.NotDone
            };

            var newEvent = new Wydarzenie
            {
                Name = NameInput.Text, 
                PlannedDate = PlannedDatePicker.SelectedDate ?? DateTime.Now, 
                StartTime = ParseTime(StartTimeInput.Text), 
                EndTime = ParseTime(EndTimeInput.Text), 
                Status = SatusEvent
            };

            _context.Wydarzenia.Add(newEvent);
            _context.SaveChanges();
            
            NameInput.Clear();
            StartTimeInput.Clear();
            EndTimeInput.Clear();
            PlannedDatePicker.SelectedDate = DateTime.Today;
            
            RefreshList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EventsGrid.SelectedItem is Wydarzenie selected)
            {
                _context.Wydarzenia.Remove(selected);
                _context.SaveChanges();
                RefreshList();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EventsGrid.SelectedItem is Wydarzenie selected)
            {
                selected.Status = selected.Status switch
                {
                    EventStatus.NotDone => EventStatus.InProgress,
                    EventStatus.InProgress => EventStatus.Done,
                    _ => EventStatus.NotDone
                };
                
                _context.Update(selected);
                _context.SaveChanges();
                RefreshList();
            }
        }
        private void UpdateSummary()
        {
            var list = _context.Wydarzenia.ToList();
            int count = list.Count;
            int notDone = list.Count(x => x.Status == EventStatus.NotDone);
            int inProgress = list.Count(x => x.Status == EventStatus.InProgress);
            int done = list.Count(x => x.Status == EventStatus.Done);

            TxtStatusSummary.Text = $"Wszystkich: {count} | Oczekujące: {notDone} | Trwa: {inProgress} | Zrealizowane: {done}";
        }
    }
}