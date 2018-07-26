using System.Xml.Serialization;
using LiteDB;

namespace encounter_builder.Database
{
    public class DatabaseConnection
    {
        private readonly string DatabaseFile = @"D:\Database\MyData.db";
        private LiteDatabase _database;

        private LiteDatabase Database => _database ?? (_database = OpenDatabase());

        private LiteDatabase OpenDatabase()
        {
            return new LiteDatabase(DatabaseFile);
        }

        public LiteCollection<T> GetCollection<T>()
        {
            return Database.GetCollection<T>(typeof(T).Name);
        }
    }

    public class KeyedDocument
    {
        [XmlIgnore]
        public ObjectId Id { get; set; }
    }
}
