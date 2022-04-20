using System;
using LiteDB;
using upendo.CrossCutting.Interfaces.Data.LocalDB;

namespace upendo.Services.Data.LocalDB
{
    public sealed class LocalDBProvider : ILocalDBProvider
    {
        private ILiteDatabase database = null;

        public ILiteDatabase Database => database;

        public string ConnectionString { get; private set; }

        public void InitDatabase(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (database != null)
                return;

            ConnectionString = connectionString;

            database = new LiteDatabase(ConnectionString);
        }

        public void DisposeDatabase()
        {
            if (database != null)
            {
                database.Dispose();
                database = null;
            }
        }
    }
}
