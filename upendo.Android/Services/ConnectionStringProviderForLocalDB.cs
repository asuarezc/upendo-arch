using System;
using System.IO;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.Helpers;

namespace upendo.Droid.Services
{
    public class ConnectionStringProviderForLocalDB : IConnectionStringProviderForLocalDB
    {
        private static readonly string databaseName = "upendoLocalDatabase.db";

        public string GetConnectionStringForLocalDB()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName
            );

            //if (!File.Exists(path))
            //    File.Create(path).Dispose();

            string password = PasswordForLocalDBFactory.GetLocalDBPassword();

            return $"Filename={path};Connection=direct;Password={password}";
        }
    }
}
