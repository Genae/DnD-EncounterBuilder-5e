using Compendium.Models.ImportData;
using Compendium.Parser;
using NUnit.Framework;
using Action = Compendium.Models.CoreData.Action;

namespace Compendium.Tests
{
    [TestFixture]
    public class TestMultiattackParser
    {
        [Test]
        public void TestMultiattack()
        {
            TestInternal("The aboleth makes three tentacle attacks.",
                new string[] { "Tentacle", "Tail", "Enslave (3/day)" },
                new Dictionary<string, int>() { { "Tentacle", 3 } }, "aboleth");

            TestInternal("The yeti can use its Chilling Gaze and makes two claw attacks.",
                new string[] { "Cold Breath (Recharge 6)", "Chilling Gaze", "Claw" },
                new Dictionary<string, int>() { { "Chilling Gaze", 1 }, { "Claw", 2 } }, "yeti");

            TestInternal("The dragon can use its Frightful Presence. It then makes three attacks: one with its bite and two with its claws.",
                new string[] { "Acid Breath (Recharge 5-6)", "Frightful Presence", "Tail", "Claw", "Bite" },
                new Dictionary<string, int>() { { "Frightful Presence", 1 }, { "Bite", 1 }, { "Claw", 2 } }, "dragon");

            TestInternal("The archer makes two attacks with its longbow.",
                new string[] { "Longbow", "Shortsword" },
                new Dictionary<string, int>() { { "Longbow", 2 } }, "archer");

            TestInternal("The babau makes two melee attacks. It can also use Weakening Gaze before or after making these attacks.",
                new string[] { "Claw", "Spear", "Weakening Gaze" },
                new Dictionary<string, int>() { { "Weakening Gaze", 1 }, { "Claw", 1 } }, "babau");

            //or
            /*TestInternal("The captain makes three melee attacks: two with its scimitar and one with its dagger. Or the captain makes two ranged attacks with its daggers.",
                new string[] { "Scimitar", "Dagger" },
                new Dictionary<string, int>() { { "Scimitar", 2 }, { "Dagger", 1 } }, "captain");
            
            TestInternal("The devil makes three melee attacks: one with its tail and two with its claws. Alternatively, it can use Hurl Flame twice.",
                new string[] { "Scimitar", "Tail", "Hurl Flame" },
                new Dictionary<string, int>() { { "Claw", 2 }, { "Tail", 1 } }, "devil");

            TestInternal("The blackguard makes three attacks with its glaive or its shortbow.",
                new string[] { "Glaive", "Shortbow", "Dreadful Aspect (Recharges after a Short or Long Rest)" },
                new Dictionary<string, int>() { { "Glaive", 3 } }, "blackguard");    
            
            TestInternal("Othelstan attacks twice with his flail or spear, or makes two ranged attacks with his spears.",
                new string[] { "Spear", "Flail" },
                new Dictionary<string, int>() { { "Flail", 2 } }, "Othelstan");              
             */

        }

        private void TestInternal(string text, string[] actions, Dictionary<string, int> result, string shortName)
        {
            var ap = new ActionParser();
            var raw = new ActionRaw() { Name = "Multiattack", Text = text };
            var errors = new List<string>();
            var res = ap.ParseMultiattack(raw, errors, actions.Select(a => a == "Claw" ? new Action() { Name = a, Attack = new Models.CoreData.Attack() { Type = Models.CoreData.Enums.AttackType.Melee_Weapon_Attack } } : new Action() { Name = a }).ToList(), out var sn);

            Assert.True(res.Actions.All(ra => result[ra.Key] == ra.Value));
            Assert.True(result.All(ra => res.Actions[ra.Key] == ra.Value));
            Assert.AreEqual(shortName, sn);
        }
    }
}
