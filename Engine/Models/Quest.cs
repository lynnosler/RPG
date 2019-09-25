using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
     public class Quest
     {
          public int ID { get; }
          public string Name { get; }
          public string Description { get; }

          public List<ItemQuantity> ItemsToComplete { get; }
          public int RewardXP { get; }
          public int RewardGold { get; }

          public List<ItemQuantity> RewardItems { get; }

          public Quest(int id, string name, string desc, List<ItemQuantity> itemsToComplete,
               int rewardXp, int rewardGold, List<ItemQuantity> rewardItems)
          {
               ID = id;
               Name = name;
               ItemsToComplete = itemsToComplete;
               RewardXP = rewardXp;
               RewardGold = rewardGold;
               RewardItems = rewardItems;
          }
     }
}
