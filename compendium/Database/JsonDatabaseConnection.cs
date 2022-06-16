using LiteDB;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace compendium.Database
{
    public class JsonDatabaseConnection : IDatabaseConnection
    {
        public static readonly string Root = @"D:\Database";
        private string Subfolder = "";
        private string DatabaseRoot => Path.Combine(Root, Subfolder);
        private Dictionary<string, List<object>> _database;

        private JsonDatabaseConnection(string subfolder)
        {
            Subfolder = subfolder;
        }

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
                    myDb[db.Name].Add(JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
                }
            }
            return myDb;
        }

        public static JsonDatabaseConnection GetProjectDb(string projectName)
        {
            var db = new JsonDatabaseConnection(Path.Combine("ProjectFolders", projectName));
            if (!Directory.Exists(db.DatabaseRoot))
                Directory.CreateDirectory(db.DatabaseRoot);
            return db;
        }

        public void Add<T>(T item) where T : KeyedDocument
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
            if (!Database.ContainsKey(typeof(T).Name))
                return new List<T>().AsQueryable();
            return Database[typeof(T).Name].Cast<T>().AsQueryable();
        }
    }
}