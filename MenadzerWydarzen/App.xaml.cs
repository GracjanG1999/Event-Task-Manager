using System.Configuration;
using System.Data;
using System.Windows;

namespace MenadzerWydarzen;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            // Próba wymuszenia startu
            var window = new MainWindow();
            window.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd przy starcie: {ex.Message}\n\nSzczegóły: {ex.StackTrace}");
        }
    }
}

