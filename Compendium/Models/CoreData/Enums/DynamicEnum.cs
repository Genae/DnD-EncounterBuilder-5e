﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compendium.Database;

namespace Compendium.Models.CoreData.Enums
{
    public class DynamicEnum : KeyedDocument
    {
        public string Name { get; set; }
        public List<string> Data { get; set; }
        protected string[] DefaultValues;

        public string GetFromInt(int index)
        {
            return Data[index];
        }

        public string Parse(string str)
        {
            return Data.FirstOrDefault(n => n.Equals(str, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}