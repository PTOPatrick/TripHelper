using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TripHelper.Infrastructure.Common.Persistence;

namespace TripHelper.Api.IntegrationTests.Common;

/// <summary>
/// We're using SQLite so no need to spin an actual database.
/// </summary>
public class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var testDatabase = new SqliteTestDatabase("DataSource=:memory:");

        testDatabase.InitializeDatabase();

        return testDatabase;
    }

    public void InitializeDatabase()
    {
        Connection.Open();
        var options = new DbContextOptionsBuilder<TripHelperDbContext>()
            .UseSqlite(Connection)
            .Options;

        var context = new TripHelperDbContext(options, null!, null!);

        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        Connection.Close();

        InitializeDatabase();
    }

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        Connection.Close();
    }
}