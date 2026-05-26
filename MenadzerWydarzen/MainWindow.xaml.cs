using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MenadzerWydarzen
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context = new();
        private readonly DispatcherTimer _notificationTimer;
        private readonly HashSet<int> _notifiedIds = new();
        private bool _notificationsEnabled = true;

        public MainWindow()
        {
            InitializeComponent();
            PlannedDatePicker.SelectedDate = DateTime.Today;
            CalendarView.SelectedDate      = DateTime.Today;

            _notificationTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _notificationTimer.Tick += (_, _) => CheckNotifications();
            _notificationTimer.Start();

            RefreshList();
            CheckNotifications();
        }

        // ─── Ładowanie i filtrowanie ────────────────────────────────────────

        private void RefreshList()
        {
            RefreshCategoryFilter();
            ApplyFilters();
        }

        private void RefreshCategoryFilter()
        {
            CategoryFilter.SelectionChanged -= CategoryFilter_SelectionChanged;
            var prev = CategoryFilter.SelectedItem?.ToString();

            CategoryFilter.Items.Clear();
            CategoryFilter.Items.Add("Wszystkie");

            var cats = _context.Wydarzenia
                .Where(w => w.Category != null && w.Category != "")
                .Select(w => w.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            foreach (var c in cats)
                CategoryFilter.Items.Add(c);

            var idx = prev != null ? CategoryFilter.Items.IndexOf(prev) : -1;
            CategoryFilter.SelectedIndex = idx >= 0 ? idx : 0;
            CategoryFilter.SelectionChanged += CategoryFilter_SelectionChanged;
        }

        private void ApplyFilters()
        {
            if (EventsGrid == null) return;

            var all = LoadAll();
            IEnumerable<Wydarzenie> filtered = all;

            var search = SearchBox?.Text;
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(e =>
                    e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (e.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Category?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

            if ((StatusFilter?.SelectedIndex ?? 0) > 0)
                filtered = filtered.Where(e => e.Status == (EventStatus)(StatusFilter!.SelectedIndex - 1));

            var selCat = CategoryFilter?.SelectedItem?.ToString();
            if (selCat != null && selCat != "Wszystkie")
                filtered = filtered.Where(e => e.Category == selCat);

            EventsGrid.ItemsSource = filtered.ToList();
            UpdateSummary(all);
        }

        private void UpdateSummary(List<Wydarzenie> list)
        {
            int total   = list.Count;
            int notDone = list.Count(x => x.Status == EventStatus.NotDone);
            int inProg  = list.Count(x => x.Status == EventStatus.InProgress);
            int done    = list.Count(x => x.Status == EventStatus.Done);
            int overdue = list.Count(x => x.Status != EventStatus.Done &&
                                          x.PlannedDate.HasValue &&
                                          x.PlannedDate.Value.Date < DateTime.Today);

            TxtStatusSummary.Text =
                $"Wszystkich: {total}  |  Oczekujące: {notDone}  |  Trwa: {inProg}  " +
                $"|  Zrealizowane: {done}  |  Przeterminowane: {overdue}";
        }

        // ─── Obsługa formularza ─────────────────────────────────────────────

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text))
            {
                MessageBox.Show("Brak nazwy wydarzenia!", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var status = (StatusCombo.SelectedItem as ComboBoxItem)?.Content.ToString() switch
            {
                "Trwa"         => EventStatus.InProgress,
                "Zrealizowane" => EventStatus.Done,
                _              => EventStatus.NotDone
            };

            var priority = (PriorityCombo.SelectedItem as ComboBoxItem)?.Content.ToString() switch
            {
                "Niski"     => EventPriority.Low,
                "Wysoki"    => EventPriority.High,
                "Krytyczny" => EventPriority.Critical,
                _           => EventPriority.Medium
            };

            _context.Wydarzenia.Add(new Wydarzenie
            {
                Name        = NameInput.Text.Trim(),
                PlannedDate = PlannedDatePicker.SelectedDate ?? DateTime.Today,
                StartTime   = ParseTime(StartTimeInput.Text),
                EndTime     = ParseTime(EndTimeInput.Text),
                Status      = status,
                Priority    = priority,
                Category    = NullIfBlank(CategoryInput.Text),
                Description = NullIfBlank(DescriptionInput.Text)
            });
            _context.SaveChanges();

            NameInput.Clear();
            StartTimeInput.Clear();
            EndTimeInput.Clear();
            CategoryInput.Clear();
            DescriptionInput.Clear();
            PlannedDatePicker.SelectedDate = DateTime.Today;
            StatusCombo.SelectedIndex   = 0;
            PriorityCombo.SelectedIndex = 1;

            RefreshList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EventsGrid.SelectedItem is not Wydarzenie selected) return;

            var confirm = MessageBox.Show(
                $"Usunąć wydarzenie \"{selected.Name}\"?", "Potwierdzenie",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                _context.Wydarzenia.Remove(selected);
                _context.SaveChanges();
                RefreshList();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EventsGrid.SelectedItem is not Wydarzenie selected)
            {
                MessageBox.Show("Zaznacz wydarzenie na liście.", "Brak zaznaczenia",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new EditWindow(selected) { Owner = this };
            if (win.ShowDialog() == true)
            {
                _context.Update(selected);
                _context.SaveChanges();
                RefreshList();
            }
        }

        // ─── Eksport CSV ────────────────────────────────────────────────────

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter   = "CSV (*.csv)|*.csv",
                FileName = $"wydarzenia_{DateTime.Today:yyyy-MM-dd}.csv"
            };
            if (dlg.ShowDialog() != true) return;

            var statusConv = new StatusConverter();
            var prioConv   = new PriorityConverter();
            var ci         = CultureInfo.CurrentCulture;
            var sb         = new StringBuilder();
            sb.AppendLine("Id,Nazwa,Data,Start,Koniec,Status,Priorytet,Kategoria,Opis");

            foreach (var ev in LoadAll())
            {
                var st = statusConv.Convert(ev.Status,   typeof(string), null!, ci);
                var pr = prioConv.Convert(ev.Priority, typeof(string), null!, ci);
                sb.AppendLine(
                    $"{ev.Id}," +
                    $"\"{ev.Name}\"," +
                    $"{ev.PlannedDate:yyyy-MM-dd}," +
                    $"{ev.StartTime:hh\\:mm}," +
                    $"{ev.EndTime:hh\\:mm}," +
                    $"{st},{pr}," +
                    $"\"{ev.Category}\"," +
                    $"\"{ev.Description?.Replace("\"", "\"\"")}\"");
            }

            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Eksport zakończony!", "CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ─── Powiadomienia ──────────────────────────────────────────────────

        private void NotificationToggle_Click(object sender, RoutedEventArgs e)
        {
            _notificationsEnabled = !_notificationsEnabled;
            NotificationToggleBtn.Content    = _notificationsEnabled ? "Powiadomienia: ON" : "Powiadomienia: OFF";
            NotificationToggleBtn.Background = _notificationsEnabled
                ? System.Windows.Media.Brushes.PaleTurquoise
                : System.Windows.Media.Brushes.LightGray;
        }

        private void CheckNotifications()
        {
            if (!_notificationsEnabled) return;

            var now  = DateTime.Now;
            var soon = now.AddMinutes(15);

            var upcoming = LoadAll()
                .Where(w => w.Status != EventStatus.Done &&
                            w.PlannedDate.HasValue &&
                            w.StartTime.HasValue)
                .Where(w => !_notifiedIds.Contains(w.Id) &&
                            w.PlannedDate!.Value.Date == now.Date &&
                            w.StartTime!.Value >= now.TimeOfDay &&
                            w.StartTime!.Value <= soon.TimeOfDay);

            foreach (var ev in upcoming)
            {
                _notifiedIds.Add(ev.Id);
                new NotificationPopup(ev.Name, ev.StartTime!.Value) { Topmost = true }.Show();
            }
        }

        // ─── Widok kalendarza ───────────────────────────────────────────────

        private void CalendarView_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!CalendarView.SelectedDate.HasValue) return;
            var date = CalendarView.SelectedDate.Value.Date;
            CalendarDateLabel.Text = $"Wydarzenia: {date:dd MMMM yyyy}";

            CalendarEventsGrid.ItemsSource = LoadAll()
                .Where(w => w.PlannedDate?.Date == date)
                .ToList();
        }

        // ─── Filtry ─────────────────────────────────────────────────────────

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)         => ApplyFilters();
        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        // ─── Pomocnicze ─────────────────────────────────────────────────────

        // Odświeża tracker i zwraca świeżą listę — EF Core nie tłumaczy TimeSpan/Date na SQL,
        // więc ładujemy całość i filtrujemy in-memory.
        private List<Wydarzenie> LoadAll()
        {
            _context.ChangeTracker.Clear();
            return _context.Wydarzenia.ToList();
        }

        private static TimeSpan? ParseTime(string text)  => WydarzenieHelper.ParseTime(text);
        private static string?   NullIfBlank(string? s)  => WydarzenieHelper.NullIfBlank(s);
    }
}
