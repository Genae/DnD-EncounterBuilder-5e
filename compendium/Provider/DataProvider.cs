using Compendium.Database;
using Compendium.Models.CoreData;
using Compendium.Models.CoreData.Enums;
using Compendium.Models.ImportData;
using Compendium.Models.ProjectData;
using Compendium.Parser;
using LiteDB;
using System.Reflection;

namespace Compendium.Provider
{
    public class DataProvider
    {
        private readonly IDatabaseConnection _db;
        public CompendiumRaw Compendium;

        public DataProvider(IDatabaseConnection db)
        {
            _db = db;
            ImportXML(db);
            LoadAllProjects(db);
        }

        private void ImportXML(IDatabaseConnection db)
        {
            var srd = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data", "SRD.xml");
            if (File.Exists(@"D:\Dateien\OneDrive\Xerios\AllData.xml"))
            {
                Compendium = new Importer().ImportCompendium(@"D:\Dateien\OneDrive\Xerios\AllData.xml");
            }
            else if (File.Exists(srd))
            {
                Compendium = new Importer().ImportCompendium(srd);
            }
            else
            {
                return;
            }

            var monsterParser = new MonsterParser(new SpellcastingParser(), new ActionParser(), new DynamicEnumProvider(db));
            var spellParser = new SpellParser();
            var allMonsters = GetAllMonsters();
            var allSpells = GetAllSpells();
            foreach (var compendiumSpell in Compendium.Spells)
            {
                if (allSpells.Any(m => m.Name.Equals(compendiumSpell.Name)))
                    continue;
                var spell = spellParser.Parse(compendiumSpell);
                db.Add(spell);
            }

            allSpells = GetAllSpells();
            foreach (var compendiumMonster in Compendium.Monsters)
            {
                if (allMonsters.Any(m => m.Name.Equals(compendiumMonster.Name)))
                    continue;
                var monster = monsterParser.Parse(compendiumMonster, allSpells);
                db.Add(monster);
            }
        }

        internal Monster SaveMonster(Monster monster)
        {
            if (monster.Id == null || monster.Id == ObjectId.Empty)
                _db.Add(monster);
            else
                _db.Update(monster);
            return monster;
        }

        private void LoadAllProjects(IDatabaseConnection db)
        {
            var allProjects = GetAllProjects();
            var allMonsters = GetAllMonsters();
            var allSpells = GetAllSpells();
            var jDb = new JsonDatabaseConnection();
            foreach (var project in jDb.GetAllProjectNames())
            {
                var pDb = JsonDatabaseConnection.GetProjectDb(project);
                foreach (var p in pDb.GetQueryable<Project>().Where(p => !allProjects.Any(mp => p.Id == mp.Id)).ToList())
                    db.Add(p);
                foreach (var m in pDb.GetQueryable<Monster>().Where(m => !allMonsters.Any(mm => m.Id == mm.Id)).ToList())
                    db.Add(m);
                foreach (var s in pDb.GetQueryable<Spell>().Where(s => !allSpells.Any(ms => s.Id == ms.Id)).ToList())
                    db.Add(s);
            }
        }

        internal IEnumerable<Monster> GetAllMonstersWithIds(ObjectId[] ids)
        {
            return _db.GetQueryable<Monster>().Where(s => ids.Contains(s.Id)).ToArray();
        }

        internal void DeleteProject(Project project)
        {
            var old = GetAllProjects().FirstOrDefault(p => p.Id.Equals(project.Id));
            if (old == null)
                return;
            JsonDatabaseConnection.GetProjectDb(old.Name).Delete();
            _db.Remove(project);
        }

        internal Project EditProject(Project project)
        {
            var old = GetAllProjects().FirstOrDefault(p => p.Id.Equals(project.Id));
            if (old == null)
                return CreateProject(project);

            var jDb = JsonDatabaseConnection.GetProjectDb(old.Name);
            if (!old.Name.Equals(project.Name))
                jDb.Rename(project.Name);
            _db.Update(project);
            jDb.Update(project);
            foreach (var monsters in GetAllMonsters().Where(m => project.MonsterIds.Contains(m.Id)))
            {
                jDb.Update(monsters);
            }
            foreach (var spell in GetAllSpells().Where(m => project.SpellIds.Contains(m.Id)))
            {
                jDb.Update(spell);
            }
            return project;
        }

        internal Project CreateProject(Project project)
        {
            _db.Add(project);
            JsonDatabaseConnection.GetProjectDb(project.Name).Add(project);
            return project;
        }

        internal IEnumerable<Project> GetAllProjects()
        {
            return _db.GetQueryable<Project>().ToArray();
        }

        public Monster[] GetAllMonsters()
        {
            return _db.GetQueryable<Monster>().ToArray();
        }

        public List<Spell> GetAllSpells()
        {
            return _db.GetQueryable<Spell>().ToList();
        }

        public IEnumerable<Spell> GetAllSpellsWithIds(ObjectId[] ids)
        {
            return _db.GetQueryable<Spell>().Where(s => ids.Contains(s.Id)).ToArray();
        }

        internal IEnumerable<WeaponType> GetAllWeapons()
        {
            return new[]
            {
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Club",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target"},
                    new HitEffect {DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(4, 1, 0)},
                    new []{"Light"}),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Dagger",
                    new Attack() { Type = AttackType.Melee_or_Ranged_Weapon_Attack, Reach = 5, ShortRange = 20, LongRange = 60, Target="one target"},
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(4, 1, 0) },
                    new[] { "Finesse", "Light", "Throw" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Greatclub",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Two-handed" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Handaxe",
                    new Attack() { Type = AttackType.Melee_or_Ranged_Weapon_Attack, Reach = 5, ShortRange = 20, LongRange = 60,Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Light", "Throw" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Javelin",
                    new Attack() { Type = AttackType.Melee_or_Ranged_Weapon_Attack, Reach = 5, ShortRange = 30, LongRange = 120, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Throw" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Light hammer",
                    new Attack() { Type = AttackType.Melee_or_Ranged_Weapon_Attack, Reach = 5, ShortRange = 20, LongRange = 60, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Light", "Throw" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Mace",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(6, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Quarterstaff",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Versatile" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Sickle",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(4, 1, 0) },
                    new[] { "Light" }),
                new WeaponType(WeaponCategory.SimpleMeleeWeapon, "Spear",
                    new Attack() { Type = AttackType.Melee_or_Ranged_Weapon_Attack, Reach = 5, ShortRange = 20, LongRange = 60, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Versatile", "Throw" }),
                new WeaponType(WeaponCategory.SimpleRangedWeapon, "Crossbow, light",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 80, LongRange = 320, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Ammunition", "Loading", "Two-handed" }),
                new WeaponType(WeaponCategory.SimpleRangedWeapon, "Dart",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 20, LongRange = 60, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(4, 1, 0) },
                    new[] { "Finesse", "Throw" }),
                new WeaponType(WeaponCategory.SimpleRangedWeapon, "Shortbow",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 80, LongRange = 320, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Ammunition", "Two-handed" }),
                new WeaponType(WeaponCategory.SimpleRangedWeapon, "Sling",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 30, LongRange = 120, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Ammunition" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Battleaxe",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Versatile" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Flail",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Glaive",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(10, 1, 0) },
                    new[] { "Heavy", "Reach", "Two-handed" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Greataxe",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(12, 1, 0) },
                    new[] { "Heavy", "Two-handed" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Greatsword",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(6, 2, 0) },
                    new[] { "Heavy", "Two-handed" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Halberd",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(10, 1, 0) },
                    new[] { "Heavy", "Reach", "Two-handed" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Lance", //TODO: Special
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(12, 1, 0) },
                    new[] { "Special", "Reach" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Longsword",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Versatile" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Maul",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(6, 2, 0) },
                    new[] { "Heavy", "Two-handed" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Morningstar",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Pike",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(10, 1, 0) },
                    new[] { "Heavy", "Reach", "Two-handed" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Rapier",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Finesse" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Scimitar",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Finesse", "Light" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Shortsword",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Finesse", "Light" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Trident",
                    new Attack() { Type = AttackType.Melee_or_Ranged_Weapon_Attack, Reach = 5, ShortRange = 20, LongRange = 60, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Versatile", "Throw" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "War pick",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Warhammer",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Versatile" }),
                new WeaponType(WeaponCategory.MartialMeleeWeapon, "Whip",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(4, 1, 0) },
                    new[] { "Finesse", "Reach" }),
                new WeaponType(WeaponCategory.MartialRangedWeapon, "Blowgun",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 25, LongRange = 100, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(0, 0, 1) },
                    new[] { "Ammunition", "Loading" }),
                new WeaponType(WeaponCategory.MartialRangedWeapon, "Crossbow, hand",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 30, LongRange = 120, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(6, 1, 0) },
                    new[] { "Ammunition", "Loading", "Light" }),
                new WeaponType(WeaponCategory.MartialRangedWeapon, "Crossbow, heavy",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 100, LongRange = 400, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(10, 1, 0) },
                    new[] { "Ammunition", "Loading", "Two-handed", "Heavy" }),
                new WeaponType(WeaponCategory.MartialRangedWeapon, "Longbow",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 150, LongRange = 600, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(8, 1, 0) },
                    new[] { "Ammunition", "Two-handed", "Heavy" }),
                new WeaponType(WeaponCategory.MartialRangedWeapon, "Net",
                    new Attack() { Type = AttackType.Ranged_Weapon_Attack, ShortRange = 5, LongRange = 15, Target="one target" },
                    new HitEffect { Condition = new List<Condition>() { Condition.Restrained } },
                    new[] { "Special", "Throw" }),
                new WeaponType(WeaponCategory.Body, "Claw",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target"},
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(6, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Bite",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Tail",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 5, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Tentacle",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Fist",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Pincer",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Bludgeoning, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Hoof",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Slashing, DamageDie = new DieRoll(8, 1, 0) },
                    new string[] { }),
                new WeaponType(WeaponCategory.Body, "Gore",
                    new Attack() { Type = AttackType.Melee_Weapon_Attack, Reach = 10, Target="one target" },
                    new HitEffect { DamageType = DamageType.Piercing, DamageDie = new DieRoll(10, 1, 0) },
                    new string[] { }),
            };
        }

    }
}
