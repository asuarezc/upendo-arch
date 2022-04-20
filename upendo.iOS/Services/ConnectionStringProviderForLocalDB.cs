using System;
using System.IO;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.Helpers;

namespace upendo.iOS.Services
{
    public class ConnectionStringProviderForLocalDB : IConnectionStringProviderForLocalDB
    {
        private static readonly string databaseName = "upendoLocalDatabase.db";

        public string GetConnectionStringForLocalDB()
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            string path = Path.Combine(libFolder, databaseName);
            string password = PasswordForLocalDBFactory.GetLocalDBPassword();

            return $"Filename={path};Connection=direct;Password={password}";
        }
    }
}
