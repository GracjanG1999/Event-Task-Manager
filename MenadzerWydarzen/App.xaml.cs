using System.Windows;

namespace MenadzerWydarzen;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        try
        {
            new MainWindow().Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd przy starcie:\n{ex.Message}\n\n{ex.StackTrace}",
                "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
