﻿using System;
using System.Collections.Generic;
using System.Linq;
using compendium.Models.CoreData;
using compendium.Models.CoreData.Enums;
using LiteDB;

namespace compendium.Database
{
    public interface IDatabaseConnection
    {
        IQueryable<T> GetQueryable<T>();
        void Add<T>(T item) where T : KeyedDocument;
    }

    public class LiteDbConnection : IDatabaseConnection
    {
        private readonly string DatabaseFile = @"MyData.db";
        private LiteDatabase _database;
        private LiteDatabase Database => _database ?? (_database = OpenDatabase());

        private LiteDatabase OpenDatabase()
        {

            BsonMapper.Global.RegisterType<Dictionary<Ability, int>>
            (
                serialize: (dic) => new BsonDocument(dic.ToDictionary(kv => kv.Key.ToString(), kv => new BsonValue(kv.Value))),
                deserialize: (bson) => ((BsonDocument)bson).RawValue.ToDictionary(kv => Enum.Parse<Ability>(kv.Key), kv => kv.Value.AsInt32)
            );
            BsonMapper.Global.RegisterType<Dictionary<Ability, AbilityScore>>
            (
                serialize: (dic) => new BsonDocument(dic.ToDictionary(kv => kv.Key.ToString(), kv => BsonMapper.Global.ToDocument(kv.Value) as BsonValue)),
                deserialize: (bson) => ((BsonDocument)bson).RawValue.ToDictionary(kv => Enum.Parse<Ability>(kv.Key), kv => BsonMapper.Global.ToObject<AbilityScore>(kv.Value as BsonDocument))
            );
            BsonMapper.Global.RegisterType<Dictionary<Skill, int>>
            (
                serialize: (dic) => new BsonDocument(dic.ToDictionary(kv => kv.Key.ToString(), kv => new BsonValue(kv.Value))),
                deserialize: (bson) => ((BsonDocument)bson).RawValue.ToDictionary(kv => Enum.Parse<Skill>(kv.Key), kv => kv.Value.AsInt32)
            );
            BsonMapper.Global.RegisterType<Dictionary<MovementType, int>>
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
