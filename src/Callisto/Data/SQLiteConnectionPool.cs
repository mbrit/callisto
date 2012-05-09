using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callisto.Data
{
    /// <summary>
    /// Defines a class that holds a connection pool.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public static class SQLiteConnectionPool
    {
        // thread-safe lock...
        private static object _lock = new object();

        // pool of connections...
        private static Dictionary<string, SQLitePooledConnection> Connections { get; set; }

        static SQLiteConnectionPool()
        {
            Connections = new Dictionary<string, SQLitePooledConnection>();
        }

        public static void AppSuspending()
        {
            // close out all the connections...
            foreach (SQLitePooledConnection conn in GetOpenConnections())
                conn.Suspend();
        }

        private static IEnumerable<SQLitePooledConnection> GetOpenConnections()
        {
            lock (_lock)
            {
                return Connections.Values.Where(v => v.IsOpen == true);
            }
        }

        internal static SQLitePooledConnection GetConnection(SQLiteConnectionSettings settings)
        {
            lock (_lock)
            {
                // find it in the dictionary...
                string key = settings.Key;
                if (!(Connections.ContainsKey(key)))
                {
                    SQLitePooledConnection conn = new SQLitePooledConnection(settings);
                    Connections[key] = conn;
                }

                // return...
                return Connections[key];
            }
        }
    }
}
