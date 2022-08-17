using Compendium.Models.CoreData;
using Compendium.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.Controllers
{
    [ApiController, Produces("application/json"), Route("api/textgen")]
    public class ActionTextController : Controller
    {
        private readonly DataProvider _dataProvider;

        public static string[] TextifyNumber = { "zero", "one", "two", "three", "four", "five", "six" };
        public static string[] TextifyNumberCe = { "zero", "once", "twice", "three times", "four times", "five times", "six times" };

        public ActionTextController(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        [HttpPost]
        [Route("multiattack")]
        public Multiattack GenerateMultiattackText([FromBody] Monster monster)
        {
            var actions = monster.MultiattackAction.Actions.Select(a => new MultiActionHelper(a, monster.Actions.FirstOrDefault(ac => ac.Name.Equals(a.Key)))).ToList();
            var nonAttack = actions.Where(a => !a.IsAttack).ToList();
            var attack = actions.Where(a => a.IsAttack).ToList();
            var nonAttackText = ConcatStrings(nonAttack);
            var attackText = ConcatStrings(attack);
            var attackCount = attack.Sum(a => a.Amount);

            var shortName = monster.ShortName ?? monster.Name;

            if (nonAttackText != null && attackText == null)
            {
                monster.MultiattackAction.Text = $"The {shortName} uses {nonAttackText}.";
                return monster.MultiattackAction;
            }
            if (nonAttackText != null && attackText != null)
            {
                monster.MultiattackAction.Text = $"The {shortName} uses {nonAttackText}. It then makes {TextifyNumber[attackCount]} attack{(attackCount == 1 ? "" : "s")}: {attackText}.";
                return monster.MultiattackAction;
            }
            if (nonAttackText == null && attackText != null)
            {
                monster.MultiattackAction.Text = $"The {shortName} makes {TextifyNumber[attackCount]} attack{(attackCount == 1 ? "" : "s")}: {attackText}.";
                return monster.MultiattackAction;
            }
            return monster.MultiattackAction;
        }

        private string? ConcatStrings(List<MultiActionHelper> list)
        {
            if (list == null || !list.Any())
                return null;
            if (list.Count == 1)
                return list.First().ToString();
            var allButLast = list.Take(list.Count - 1);
            return string.Join(", ", allButLast) + " and " + list.Last();
        }
    }

    internal class MultiActionHelper
    {
        public Models.CoreData.Action Action;
        public bool IsAttack => Action.Attack != null;
        public int Amount { get; internal set; }

        public MultiActionHelper(KeyValuePair<string, int> a, Models.CoreData.Action action)
        {
            Amount = a.Value;
            Action = action;
        }

        public override string ToString()
        {
            if (IsAttack)
            {
                return $"{ActionTextController.TextifyNumber[Amount]} with its {Action.Name.ToLower()}";
            }
            return $"{Action.Name} {ActionTextController.TextifyNumberCe[Amount]}";
        }
    }
}
