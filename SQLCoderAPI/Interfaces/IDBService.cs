namespace SQLCoderAPI.Interfaces
{
    public interface IDBService
    {
        Task<List<Dictionary<string, object>>> GetSQLResultAsync(string sql);
    }
}