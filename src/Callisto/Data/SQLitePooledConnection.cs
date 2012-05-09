using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Callisto.Data.SQLite;

namespace Callisto.Data
{
    /// <summary>
    /// Defines a class that holds a reference to a pooled SQLite connection.
    /// </summary>
    internal class SQLitePooledConnection : IDisposable
    {
        private SQLiteConnectionSettings Settings { get; set; }
        private SQLiteConnection _innerConnection;

        internal SQLitePooledConnection(SQLiteConnectionSettings settings)
        {
            this.Settings = settings;
        }

        ~SQLitePooledConnection()
        {
            this.Dispose();
        }

        internal void Suspend()
        {
            if (_innerConnection != null)
            {
                try
                {
                    _innerConnection.Dispose();
                }
                finally
                {
                    _innerConnection = null;
                }
            }
        }

        internal SQLiteConnection InnerConnection
        {
            get
            {
                if (_innerConnection == null)
                {
                    if (this.Settings.HasOpenFlags)
                        _innerConnection = new SQLiteConnection(this.Settings.DatabasePath, this.Settings.OpenFlags, this.Settings.OverridePath);
                    else
                        _innerConnection = new SQLiteConnection(this.Settings.DatabasePath, this.Settings.OverridePath);
                }

                return _innerConnection;
            }
        }

        public void Dispose()
        {
            // suspend and dispose are essentially the same - dispose is really here just
            // to cue people into thinking about lifetime of this object...
            this.Suspend();

            // stop...
            GC.SuppressFinalize(this);
        }

        internal bool IsOpen
        {
            get
            {
                return _innerConnection != null;
            }
        }
    }
}
