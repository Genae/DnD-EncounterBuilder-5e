using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;

namespace encounter_builder.Database
{
    public class JsonDatabaseConnection : IDatabaseConnection
    {
        private readonly string DatabaseRoot = @"D:\Database";
        private Dictionary<string, List<object>> _database;

        private Dictionary<string, List<object>> Database => _database ?? (_database = LoadDatabase());

        private Dictionary<string, List<object>> LoadDatabase()
        {
            var myDb = new Dictionary<string, List<object>>();
            var dbs = Directory.GetDirectories(DatabaseRoot);
            foreach (var db in dbs.Select(d => new DirectoryInfo(d)))
            {
                myDb[db.Name] = new List<object>();
                foreach (var entry in Directory.GetFiles(db.FullName))
                {
                    var json = File.ReadAllText(entry);
                    myDb[db.Name].Add(JsonConvert.DeserializeObject(json, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All}));
                }
            }
            return myDb;
        }

        public void Add<T>(T item) where T: KeyedDocument
        {
            if (!Database.ContainsKey(typeof(T).Name))
            {
                Database[typeof(T).Name] = new List<object>();
                Directory.CreateDirectory(Path.Combine(DatabaseRoot, typeof(T).Name));
            }
            if (item.Id == null)
            {
                item.Id = ObjectId.NewObjectId();
            }
            Database[typeof(T).Name].Add(item);
            File.WriteAllText(Path.Combine(DatabaseRoot, typeof(T).Name, item.Id + ".json"), JsonConvert.SerializeObject(item, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented }));
        }

        public IQueryable<T> GetQueryable<T>()
        {
            if(!Database.ContainsKey(typeof(T).Name))
                return new List<T>().AsQueryable();
            return Database[typeof(T).Name].Cast<T>().AsQueryable();
        }
    }
}