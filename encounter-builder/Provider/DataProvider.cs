using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using encounter_builder.Database;
using encounter_builder.Models.ImportData;

namespace encounter_builder.Provider
{
    public class DataProvider
    {
        public CompendiumRaw Compendium;

        public DataProvider()
        {
            Compendium = new Importer().ImportCompendium(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data", "SRD.xml"));
        }

        public MonsterRaw[] GetAllMonsters()
        {
            return Compendium.Monsters.ToArray();
        }
    }
}
