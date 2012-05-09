using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Callisto.Data.SQLite;

namespace Callisto.TestApp
{
    // ignore this - just testing something...
    public abstract class Entity
    {
        public Task<int> InsertAsync(SQLiteAsyncConnection conn)
        {
            return conn.InsertAsync(this);
        }
    }
}
