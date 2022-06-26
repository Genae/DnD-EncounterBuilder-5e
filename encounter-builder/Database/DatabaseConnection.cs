using System;
using System.Collections.Generic;
using System.Linq;
using encounter_builder.Models.CoreData;
using encounter_builder.Models.CoreData.Enums;
using LiteDB;

namespace encounter_builder.Database
{
    public interface IDatabaseConnection
    {
        IQueryable<T> GetQueryable<T>();
        void Add<T>(T item) where T : KeyedDocument;
    }

    public class LiteDbConnection : IDatabaseConnection
    {
        private readonly string DatabaseFile = @"D:\Database\MyData.db";
        private LiteDatabase _database;
        private LiteDatabase Database => _database ?? (_database = OpenDatabase());

        private LiteDatabase OpenDatabase()
        {
            BsonMapper.Global.RegisterType
            (
                serialize: (dic) => new BsonDocument(dic.ToDictionary(kv => kv.Key.ToString(), kv => new BsonValue(kv.Value))),
                deserialize: (bson) => ((BsonDocument)bson).RawValue.ToDictionary(kv => Enum.Parse<Skill>(kv.Key), kv => kv.Value.AsInt32)
            );
            BsonMapper.Global.RegisterType
            (
                serialize: (dic) => new BsonDocument(dic.ToDictionary(kv => kv.Key.ToString(), kv => new BsonValue(kv.Value))),
                deserialize: (bson) => ((BsonDocument)bson).RawValue.ToDictionary(kv => Enum.Parse<MovementType>(kv.Key), kv => kv.Value.AsInt32)
            );
            return new LiteDatabase(DatabaseFile);
        }

        public IQueryable<T> GetQueryable<T>()
        {
            return Database.GetCollection<T>(typeof(T).Name).FindAll().AsQueryable();
        }

        public void Add<T>(T item) where T : KeyedDocument
        {
            Database.GetCollection<T>(typeof(T).Name).Insert(item);
        }
    } 
}
