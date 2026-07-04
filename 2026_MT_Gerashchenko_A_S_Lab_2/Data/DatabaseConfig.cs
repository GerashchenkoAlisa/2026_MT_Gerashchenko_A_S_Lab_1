using Entities;

namespace Data;

public static class DatabaseConfig
{
    public const string ConnectionString = "Data Source=app.db";

    public const string MigrationCompleted = "Database successfully migrated to latest version.";
}
