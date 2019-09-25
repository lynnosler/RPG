using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
     public class Player : LivingEntity
     {
          #region Properties
          private string _characterClass { get; set; }
          private int _xp { get; set; }
             
          public string CharacterClass
          {
               get { return _characterClass; }
               set
               {
                    _characterClass = value;
                    OnPropertyChanged();
               }
          }
          
          public int XP
          {
               get { return _xp; }
               private set
               {
                    _xp = value;
                    OnPropertyChanged();
               }
          }

          public ObservableCollection<QuestStatus> Quests { get; }
          public ObservableCollection<Recipe> Recipes { get; }
          #endregion

          public event EventHandler OnLeveledUp;

          #region Constructor
          public Player(string name, string cc, int xp, int maxhp, int chp, int gold):
               base(name, maxhp, chp, gold)
          {
               CharacterClass = cc;
               XP = xp;

               Quests = new ObservableCollection<QuestStatus>();
               Recipes = new ObservableCollection<Recipe>();
          }
          #endregion

           public  void AddExperience(int xp)
          {
               XP += xp;
          }

          public void LearnRecipe(Recipe recipe)
          {
               if(!Recipes.Any(r=>r.ID == recipe.ID))
               {
                    Recipes.Add(recipe);
               }
          }

          public void SetLevelAndMaxHP()
          {
               int originalLevel = Level;

               Level = (XP / 100) - 1;

               if(Level != originalLevel)
               {
                    MaxHP = Level * 10;
                    OnLeveledUp?.Invoke(this, System.EventArgs.Empty);
               }
          }

     }
}
