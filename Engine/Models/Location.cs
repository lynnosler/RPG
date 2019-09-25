using Engine.Factories;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Models
{
     public class Location
     {
          public int XCoordinate { get; }
          public int YCoordinate { get; }
          public string Name { get;}
          public string Description { get; }
          public string ImageName { get; }

          public List<Quest> QuestsAvailableHere { get; } = new List<Quest>();
          public List<MonsterEncounter> MonstersHere { get; } = new List<MonsterEncounter>();

          public Trader TraderHere { get; set; }

          public Location(int xCoord, int yCoord, string name, string desc, string imageName)
          {
               XCoordinate = xCoord;
               YCoordinate = yCoord;
               Name = name;
               Description = desc;
               ImageName = imageName;
          }

          public void AddMonster(int monsterid, int chanceOfEncountering)
          {
               if(MonstersHere.Exists(m=>m.MonsterID == monsterid))
               {
                    //This monster has already been added to this location
                    //So, overwrite the changeOfEncountering with the new number
                    MonstersHere.First(m => m.MonsterID == monsterid).ChanceOfEncounter = chanceOfEncountering;
               }
               else
               {
                    //this monster is not already here, so add it
                    MonstersHere.Add(new MonsterEncounter(monsterid, chanceOfEncountering));
               }
          }

          public Monster GetMonster()
          {
               if(!MonstersHere.Any())
               {
                    return null;
               }

               //total the percentages of all monster at this location
               int totalChances = MonstersHere.Sum(m => m.ChanceOfEncounter);

               //Select a random number between 1 and the total (in case the total chance is not 100)
               int randomNumber = RandomNumberGenerator.NumberBetween(1, totalChances);

               //loop through the monster list
               //add the monster's percentag chance of appearing to the runningTotal variable
               //when the random number is lower than the runningTotal
               //that is the monter to return.
               int runningTotal = 0;

               foreach(MonsterEncounter me in MonstersHere)
               {
                    runningTotal += me.ChanceOfEncounter;

                    if(randomNumber <= runningTotal)
                    {
                         return MonsterFactory.GetMonster(me.MonsterID);
                    }
               }
               //If there was a problem, return the last monster in the list
               return MonsterFactory.GetMonster(MonstersHere.Last().MonsterID);
          }
     }
}
