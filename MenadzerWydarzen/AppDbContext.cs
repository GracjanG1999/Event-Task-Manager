using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // "Stwórzenie w bazie tabelę o nazwie 'Wydarzenia' na podstawie klasy 'Wydarzenie'"
    public DbSet<Wydarzenie> Wydarzenia { get; set; }
    public AppDbContext()
    {
        // Ta jedna linijka sprawdza bazę przy starcie aplikacji
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Tu wskazujemy, że bazą będzie prosty plik o nazwie MojaBaza.db
        options.UseSqlite("Data Source=MojaBaza.db");
    }
}