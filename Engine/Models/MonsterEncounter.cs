namespace Engine.Models
{
     public class MonsterEncounter
     {
          public int MonsterID { get;}
          public int ChanceOfEncounter { get; set; }

          public MonsterEncounter(int monsterid, int chanceOfEncounter)
          {
               MonsterID = monsterid;
               ChanceOfEncounter = chanceOfEncounter;
          }
     }
}
