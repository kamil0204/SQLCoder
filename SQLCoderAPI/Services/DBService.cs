using Microsoft.Data.SqlClient;
using SQLCoderAPI.Interfaces;

namespace SQLCoderAPI.Services;

public class DBService : IDBService
{
    //send in the connection string as IConfiguration
    public DBService() { }

    public async Task<List<Dictionary<string, object>>> GetSQLResultAsync(string sql)
    {
        var result = new List<Dictionary<string, object>>();

        using (var connection = new SqlConnection(""))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(row);
                    }
                }
            }
        }

        return result;
    }
}
