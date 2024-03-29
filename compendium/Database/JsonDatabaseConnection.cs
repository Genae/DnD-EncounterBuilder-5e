﻿using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Compendium.Database
{
    public class JsonDatabaseConnection : IDatabaseConnection
    {
        public static readonly string Root = @"C:\Database";
        private string Subfolder = "";
        private string DatabaseRoot => Path.Combine(Root, Subfolder);
        private Dictionary<string, List<KeyedDocument>> _database;

        private JsonDatabaseConnection(string subfolder)
        {
            Subfolder = subfolder;
        }

        public JsonDatabaseConnection() { }

        private Dictionary<string, List<KeyedDocument>> Database => _database ?? (_database = LoadDatabase());

        private Dictionary<string, List<KeyedDocument>> LoadDatabase()
        {

            var myDb = new Dictionary<string, List<KeyedDocument>>();
            var dbs = Directory.GetDirectories(DatabaseRoot);
            foreach (var db in dbs.Select(d => new DirectoryInfo(d)))
            {
                myDb[db.Name] = new List<KeyedDocument>();
                foreach (var entry in Directory.GetFiles(db.FullName))
                {
                    var json = File.ReadAllText(entry);
                    var obj = JsonConvert.DeserializeObject(json,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            Converters = new List<JsonConverter>
                            {
                                new ObjectIdConverter(),
                                new StringEnumConverter()
                            }
                        });
                    myDb[db.Name].Add((KeyedDocument)obj);
                }
            }
            return myDb;
        }

        public void Add<T>(T item) where T : KeyedDocument
        {
            if (!Database.ContainsKey(typeof(T).Name))
            {
                Database[typeof(T).Name] = new List<KeyedDocument>();
                Directory.CreateDirectory(Path.Combine(DatabaseRoot, typeof(T).Name));
            }
            if (item.Id == null)
            {
                item.Id = ObjectId.NewObjectId();
            }
            var old = Database[typeof(T).Name].FirstOrDefault(i => (i as T)?.Id == item.Id);
            if (old != null)
            {
                Database[typeof(T).Name].Remove(old);
            }
            Database[typeof(T).Name].Add(item);
            Database[typeof(T).Name] = Database[typeof(T).Name].OrderBy(i => i.Id).ToList();

            File.WriteAllText(Path.Combine(DatabaseRoot, typeof(T).Name, item.Id + ".json"), JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>
                            {
                                new ObjectIdConverter(),
                                new StringEnumConverter()
                            }
            }));
        }

        internal IEnumerable<string> GetAllProjectNames()
        {
            var dir = Path.Combine(Root, "ProjectFolders");
            return Directory.EnumerateDirectories(dir).Select(d => new DirectoryInfo(d)).Where(d => !d.Name.StartsWith(".")).Select(d => d.Name);
        }

        public void Remove<T>(T item) where T : KeyedDocument
        {
            if (Database.ContainsKey(typeof(T).Name))
            {
                Database[typeof(T).Name].Remove(item);
                File.Delete(Path.Combine(DatabaseRoot, typeof(T).Name, item.Id + ".json"));
            }
        }

        public IQueryable<T> GetQueryable<T>()
        {
            if (!Database.ContainsKey(typeof(T).Name))
                return new List<T>().AsQueryable();
            return Database[typeof(T).Name].Cast<T>().AsQueryable();
        }

        public void Delete()
        {
            if (Directory.Exists(DatabaseRoot))
                Directory.Delete(DatabaseRoot, true);
        }

        public static JsonDatabaseConnection GetProjectDb(string projectName)
        {
            var db = new JsonDatabaseConnection(Path.Combine("ProjectFolders", projectName));
            if (!Directory.Exists(db.DatabaseRoot))
                Directory.CreateDirectory(db.DatabaseRoot);
            return db;
        }

        public void Rename(string name)
        {
            var newSub = Path.Combine("ProjectFolders", name);
            Directory.Move(DatabaseRoot, Path.Combine(Root, newSub));
            Subfolder = newSub;
        }

        public void Update<T>(T item) where T : KeyedDocument
        {
            Add(item);
        }

        public T Store<T>(T item) where T : KeyedDocument
        {
            if (item.Id == null || item.Id == ObjectId.Empty)
                Add(item);
            else
                Update(item);
            return item;
        }
    }
}