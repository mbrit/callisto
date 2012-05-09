using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Callisto.Data.SQLite
{
    /// <summary>
    /// Defines a class that provides asynchronous methods around a SQLiteConnection.
    /// </summary>
    /// <remarks>
    /// This class will hold a reference to a SQLite database, but not an actual connection. Connections are
    /// created on demand when an operation needs to run. This class also holds a shared lock across all
    /// threads to prevent contention on the database.
    /// </remarks>
    public class SQLiteAsyncConnection
    {
        // holds the actual connection...
        private SQLitePooledConnection PooledConnection { get; set; }

        // provides locking across threads...
        //public object _lock = new object();

        public SQLiteAsyncConnection(string filePath, bool overridePath = false)
            : this(new SQLiteConnectionSettings(filePath, overridePath))
        {
        }

        public SQLiteAsyncConnection(string filePath, SQLiteOpenFlags openFlags, bool overridePath = false)
            : this(new SQLiteConnectionSettings(filePath, openFlags, overridePath))
        {
        }

        public SQLiteAsyncConnection(SQLiteConnectionSettings settings)
        {
            this.PooledConnection = SQLiteConnectionPool.GetConnection(settings);
            if (PooledConnection == null)
                throw new InvalidOperationException("'PooledConnection' is null.");
        }

        private SQLiteConnection InnerConnection
        {
            get
            {
                return this.PooledConnection.InnerConnection;
            }
        }

        public Task<int> CreateTableAsync<T>()
            where T : new()
        {
            return Task<int>.Factory.StartNew(() =>
            {
                //lock(_lock)
                    return this.InnerConnection.CreateTable<T>();
            });
        }

        public Task<TableQuery<T>> TableAsync<T>()
            where T : new()
        {
            return Task<TableQuery<T>>.Factory.StartNew(() =>
            {
                //lock(_lock)
                    return this.InnerConnection.Table<T>();
            });
        }

        public Task<int> InsertAsync(object obj)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                //lock(_lock)
                    return this.InnerConnection.Insert(obj);
            });
        }

        public Task<int> InsertAsync(object obj, Type type)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                //lock(_lock)
                    return this.InnerConnection.Insert(obj, type);
            });
        }

        public Task<int> InsertAsync(object obj, string extra)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                //lock(_lock)
                    return this.InnerConnection.Insert(obj, extra);
            });
        }

        public Task<int> InsertAsync(object obj, string extra, Type type)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                //lock(_lock) 
                    return this.InnerConnection.Insert(obj, extra, type);
            });
        }
    }
}
