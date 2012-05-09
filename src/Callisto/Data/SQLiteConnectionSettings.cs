using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Callisto.Data.SQLite;

namespace Callisto.Data
{
    public class SQLiteConnectionSettings
    {
        public string DatabasePath { get; private set; }
        public SQLiteOpenFlags OpenFlags { get; private set; }
        public bool OverridePath { get; private set; }
        public bool HasOpenFlags { get; private set; }

        internal string Key { get; private set; }

        public SQLiteConnectionSettings(string databasePath, bool overridePath = false)
        {
            this.DatabasePath = databasePath;
            this.OverridePath = overridePath;
            this.HasOpenFlags = false;

            BuildKey();
        }

        public SQLiteConnectionSettings(string databasePath, SQLiteOpenFlags openFlags, bool overridePath = false)
        {
            this.DatabasePath = databasePath;
            this.OpenFlags = openFlags;
            this.OverridePath = overridePath;
            this.HasOpenFlags = true;

            BuildKey();
        }

        private void BuildKey()
        {
            this.Key = string.Format("{0}|{1}|{2}|{3}", this.DatabasePath, this.HasOpenFlags, this.OpenFlags, this.OverridePath);
        }
    }
}
