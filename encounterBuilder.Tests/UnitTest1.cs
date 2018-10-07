using System.Collections.Generic;
using encounter_builder.Models.CoreData;
using encounter_builder.Models.CoreData.Enums;
using encounter_builder.Models.ImportData;
using encounter_builder.Parser;
using Xunit;

namespace encounterBuilder.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var ap = new ActionParser();
            List<string> errors = new List<string>();
            var action = ap.ParseAction(new ActionRaw()
            {
                Name = "Bite",
                Text = @"Melee Weapon Attack: +4 to hit, reach 5 ft., one target.
                Hit: 7 (1d10 + 2) piercing damage plus 5 (1d10) poison
                damage, and the target is grappled (escape DC 13). Until this
                grapple ends, the target is restrained, and the toad can't bite
                another target. "
            }, errors);
            Assert.Equal(new Attack
            {
                AttackBonus = 4,
                LongRange = 0,
                ShortRange = 0,
                Reach = 5,
                Target = "one target",
                Type = AttackType.Melee_Weapon_Attack
            }, action.Attack);
            Assert.Contains(new HitEffect
            {
                Condition = null,
                DamageType = DamageType.Piercing,
                DC = null,
                DamageDie = new DieRoll(10, 1, 2)
            }, action.HitEffects);
            Assert.Contains(new HitEffect
            {
                Condition = null,
                DamageType = DamageType.Poison,
                DC = null,
                DamageDie = new DieRoll(10, 1, 0)
            }, action.HitEffects);
            Assert.Contains(new HitEffect
            {
                Condition = new List<Condition> {Condition.Grappled, Condition.Restrained},
                DamageType = null,
                DC = new SkillCheck {Skill = Skill.Acrobatics | Skill.Athletics, Value = 13},
                DamageDie = null
            }, action.HitEffects);
        }
    }
}
