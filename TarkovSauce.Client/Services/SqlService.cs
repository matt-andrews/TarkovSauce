
using Microsoft.Extensions.Logging;
using SQLite;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Services
{
    public interface ISqlService
    {
        void Insert<T>(T? entity)
            where T : class;
        void InsertAll<T>(IEnumerable<T>? entities)
            where T : class;
        IEnumerable<T> Get<T>()
            where T : new();
        IEnumerable<T> GetWhere<T>(Func<T, bool> predicate)
            where T : new();
    }
    internal class SqlService : ISqlService
    {
        private readonly SQLiteConnection _connection;
        private readonly ILogger<SqlService> _logger;

        public SqlService(string connectionString, ILogger<SqlService> logger)
        {
            _connection = new SQLiteConnection(connectionString,
                SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);

            _connection.CreateTable<FleaEventModel>();
            _connection.CreateTable<ItemModel>();
            _connection.CreateTable<TaskModelFlat>();
            _logger = logger;
        }
        public void Insert<T>(T? entity)
            where T : class
        {
            if (entity is null) return;
            try
            {
                _connection.Insert(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        public void InsertAll<T>(IEnumerable<T>? entities)
            where T : class
        {
            if (entities is null) return;
            try
            {
                foreach (var entity in entities)
                {
                    _connection.InsertOrReplace(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        public IEnumerable<T> Get<T>()
            where T : new()
        {
            try
            {
                return _connection.Table<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return [];
        }
        public IEnumerable<T> GetWhere<T>(Func<T, bool> predicate)
            where T : new()
        {
            try
            {
                return _connection.Table<T>().Where(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return [];
        }
    }
}
