
using SQLite;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Services
{
    public interface ISqlService
    {
        void Insert<T>(T entity) where T
            : class;
        IEnumerable<T> Get<T>() where T
            : new();
    }
    internal class SqlService : ISqlService
    {
        private readonly SQLiteConnection _connection;
        public SqlService(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            _connection.CreateTable<FleaEventModel>();
        }
        public void Insert<T>(T entity) where T 
            : class
        {
            _connection.Insert(entity);
        }
        public IEnumerable<T> Get<T>() where T 
            : new()
        {
            return _connection.Table<T>();
        }
    }
}
