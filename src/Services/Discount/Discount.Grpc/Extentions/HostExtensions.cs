using System.Data;
using Npgsql;
using Serilog;

namespace Discount.Grpc.Extentions;

public static class HostExtensions
{
    public static async Task MigrateDatabase<TContext>(this WebApplication host, int? retry = 0)
    {
        if (retry is null) return;
        var retryForAvailability = retry.Value;

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();

        try
        {
            Log.Information("Migrating postgresql database");

            await using var connection = new NpgsqlConnection
                (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            await using var command = new NpgsqlCommand
            {
                Connection = connection, CommandText = "DROP TABLE IF EXISTS Coupon"
            };

            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                            ProductName VARCHAR(24) NOT NULL,
                                                            Description TEXT,
                                                            Amount INT)";
            await command.ExecuteNonQueryAsync();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
            await command.ExecuteNonQueryAsync();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
            await command.ExecuteNonQueryAsync();

            Log.Information("Migrated postgresql database");
        }
        catch (NpgsqlException ex)
        {
            Log.Fatal(ex, "An error occurred while migrating the postgresql database");

            if (retryForAvailability < 50)
            {
                retryForAvailability++;
                System.Threading.Thread.Sleep(2000);
                await MigrateDatabase<TContext>(host, retryForAvailability);
            }
        }
    }
}