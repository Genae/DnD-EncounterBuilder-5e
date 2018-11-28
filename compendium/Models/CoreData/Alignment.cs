using compendium.Models.CoreData.Enums;

namespace compendium.Models.CoreData
{
    public class Alignment
    {
        public Morality Morality { get; set; } 
        public Order Order { get; set; } 

        public Alignment() { }

        public Alignment(Morality morality, Order order)
        {
            Morality = morality;
            Order = order;
        }
    }
}