using Engine.Models;
using System;

namespace Engine.Actions
{
     public class Heal : BaseAction, IAction
     {
          
          private readonly int _hpToHeal;

          public Heal(GameItem itemInUse, int hpToHeal) :
               base(itemInUse)
          {
               if (itemInUse.Category != GameItem.ItemCategory.Consumable)
               {
                    throw new ArgumentException($"{itemInUse.Name} is not consumable");
               }

               _hpToHeal = hpToHeal;
          }


          public void Execute(LivingEntity actor, LivingEntity target)
          {
               string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}";
               string targetName = (target is Player) ? "yourself" : $"the {target.Name.ToLower()}";

               ReportResult($"{actorName} heal {targetName} for {_hpToHeal} point{(_hpToHeal > 1 ? "s" : "")}.");
               target.Heal(_hpToHeal);
          }
     }
}
