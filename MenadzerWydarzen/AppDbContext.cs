using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;

public class AppDbContext : DbContext
{
    public DbSet<Wydarzenie> Wydarzenia { get; set; }

    public AppDbContext()
    {
        Database.EnsureCreated();
        EnsureNewColumns();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=MojaBaza.db");
    }

    private void EnsureNewColumns()
    {
        var connection = Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "PRAGMA table_info(Wydarzenia)";
        using var reader = cmd.ExecuteReader();
        var existing = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
        while (reader.Read())
            existing.Add(reader.GetString(1));

        var toAdd = new Dictionary<string, string>
        {
            ["Priority"]    = "INTEGER NOT NULL DEFAULT 1",
            ["Category"]    = "TEXT",
            ["Description"] = "TEXT"
        };

        foreach (var (col, def) in toAdd)
        {
            if (!existing.Contains(col))
            {
                using var alter = connection.CreateCommand();
                alter.CommandText = $"ALTER TABLE Wydarzenia ADD COLUMN {col} {def}";
                alter.ExecuteNonQuery();
            }
        }
    }
}