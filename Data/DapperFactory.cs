namespace AspNetEmptyApp.Data;

using System.Data;
using System.Globalization;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

public interface IDapperContext
{
    IDbConnection CreateConnection();
}

public class DapperFactory : IDapperContext
{
    private readonly string _connectionString;

    static DapperFactory()
    {
        // Setup Dapper TypeHandlers for SQLite data types
        SqlMapper.AddTypeHandler(new SqliteGuidHandler());
        SqlMapper.AddTypeHandler(new SqliteDecimalHandler());
    }

    public DapperFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=app.db";
    }

    public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}

public class SqliteGuidHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        if (value is Guid g) return g;
        if (value is string s && Guid.TryParse(s, out var parsed)) return parsed;
        return Guid.Empty;
    }
}

public class SqliteDecimalHandler : SqlMapper.TypeHandler<decimal>
{
    public override void SetValue(IDbDataParameter parameter, decimal value)
    {
        parameter.Value = value.ToString(CultureInfo.InvariantCulture);
    }

    public override decimal Parse(object value)
    {
        if (value is decimal d) return d;
        if (value == null || value is DBNull) return 0m;
        return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }
}
