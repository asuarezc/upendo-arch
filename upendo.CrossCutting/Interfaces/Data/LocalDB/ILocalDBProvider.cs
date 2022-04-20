using LiteDB;

namespace upendo.CrossCutting.Interfaces.Data.LocalDB
{
    public interface ILocalDBProvider
    {
        ILiteDatabase Database { get; }

        void InitDatabase(string connectionString);

        void DisposeDatabase();
    }
}
