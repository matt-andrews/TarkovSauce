
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
        public SqlService(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString,
                SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);

            _connection.CreateTable<FleaEventModel>();
            _connection.CreateTable<ItemModel>();
            _connection.CreateTable<TaskModel>();
        }
        public void Insert<T>(T? entity)
            where T : class
        {
            if (entity is null) return;
            try
            {
                _connection.Insert(entity);
            }
            catch { }//TODO log me
        }
        public void InsertAll<T>(IEnumerable<T>? entities)
            where T : class
        {
            if (entities is null) return;
            try
            {
                _connection.InsertAll(entities);
            }
            catch { }//TODO log me
        }
        public IEnumerable<T> Get<T>()
            where T : new()
        {
            try
            {
                return _connection.Table<T>();
            }
            catch { }//TODO log me
            return [];
        }
        public IEnumerable<T> GetWhere<T>(Func<T, bool> predicate)
            where T : new()
        {
            try
            {
                return _connection.Table<T>().Where(predicate);
            }
            catch { }//TODO log me
            return [];
        }
    }
}
