using System.ComponentModel;

namespace Compendium.Models.CoreData
{
    public class Action
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string LimitedUsageText => LimitedUsage.HasValue ? $" ({LimitedUsage.Value.ToDescriptionString()})" : "";
        public LimitedUsage? LimitedUsage { get; set; }
        public Attack? Attack { get; set; }
        public List<HitEffect>? HitEffects { get; set; } = new List<HitEffect>();
    }

    public enum LimitedUsage
    {
        [Description("Recharge 6")]
        Recharge6,
        [Description("Recharge 5-6")]
        Recharge5,
        [Description("Recharge after a Short or Long Rest")]
        RechargeShort,
        [Description("Recharge after a Long Rest")]
        RechargeLong,
        [Description("1/Day")]
        OnePerDay,
        [Description("2/Day")]
        TwoPerDay,
        [Description("3/Day")]
        ThreePerDay,
    }
    public static class MyEnumExtensions
    {
        public static string ToDescriptionString(this LimitedUsage val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    } 
}