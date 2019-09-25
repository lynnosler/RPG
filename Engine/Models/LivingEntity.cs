using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
     public abstract class LivingEntity : BaseNotificationClass
     {
          private string _name;
          private int _currentHP;
          private int _maxHP;
          private int _gold;
          private int _level;
          private GameItem _currentWeapon;
          private GameItem _currentConsumable;

          #region Properties
          public string Name
          {
               get { return _name; }
               private set
               {
                    _name = value;
                    OnPropertyChanged();
               }
          }
          public int CurrentHP
          {
               get { return _currentHP; }
               private set
               {
                    _currentHP = value;
                    OnPropertyChanged();
               }
          }
          public int MaxHP
          {
               get { return _maxHP; }
               protected set
               {
                    _maxHP = value;
                    OnPropertyChanged();
               }
          }
          public int Gold
          {
               get { return _gold; }
               private set
               {
                    _gold = value;
                    OnPropertyChanged();
               }
          }

          public int Level
          {
               get { return _level; }
               protected set
               {
                    _level = value;
                    OnPropertyChanged();
               }
          }

          public GameItem CurrentWeapon
          {
               get { return _currentWeapon; }
               set
               {
                    if (_currentWeapon != null)
                    {
                         _currentWeapon.Action.OnActionPerformed -= RaiseActionPerformedEvent;
                    }

                    _currentWeapon = value;

                    if (_currentWeapon != null)
                    {
                         _currentWeapon.Action.OnActionPerformed += RaiseActionPerformedEvent;
                    }

                    OnPropertyChanged();
               }
          }
          public GameItem CurrentConsumable
          {
               get {return _currentConsumable; }
               set
               {
                    if(_currentConsumable != null)
                    {
                         _currentConsumable.Action.OnActionPerformed -= RaiseActionPerformedEvent;
                    }

                    _currentConsumable = value;

                    if(_currentConsumable != null)
                    {
                         _currentConsumable.Action.OnActionPerformed += RaiseActionPerformedEvent;
                    }
                    OnPropertyChanged();
               }
          }

          public ObservableCollection<GameItem> Inventory { get; }
          public ObservableCollection<GroupedInventoryItem> GroupedInventory { get; }
          public List<GameItem> Weapons =>
               Inventory.Where(i => i.Category == GameItem.ItemCategory.Weapon).ToList();
          public List<GameItem> Consumables =>
               Inventory.Where(i => i.Category == GameItem.ItemCategory.Consumable).ToList();
          public bool HasConsumable => Consumables.Any();
          public bool IsDead => CurrentHP <= 0;
          #endregion

          #region Events
          public event EventHandler<string> OnActionPerformed;
          public event EventHandler OnKilled;
          #endregion

          #region Constructor
          protected LivingEntity(string name, int maxhp, int currenthp, int gold, int level = 1)
          {
               Name = name;
               MaxHP = maxhp;
               CurrentHP = currenthp;
               Gold = gold;
               Level = level;

               Inventory = new ObservableCollection<GameItem>();
               GroupedInventory = new ObservableCollection<GroupedInventoryItem>();


          }
          #endregion

          #region public functions
          public void UseCurrentWeaponOn(LivingEntity target)
          {
               CurrentWeapon.PerformAction(this, target);
          }
          public void UseCurrentConsumable()
          {
               CurrentConsumable.PerformAction(this, this);
               RemoveItemFromInventory(CurrentConsumable);
          }

          public void TakeDamage(int hitPointsOfDamage)
          {
               CurrentHP -= hitPointsOfDamage;

               if(IsDead)
               {
                    CurrentHP = 0;
                    RaiseOnKilledEvent();
               }
          }

          public void Heal(int hitPointsToHeal)
          {
               CurrentHP += hitPointsToHeal;

               if(CurrentHP > MaxHP)
               {
                    CurrentHP = MaxHP;
               }
          }

          public void CompletelyHeal()
          {
               CurrentHP = MaxHP;
          }

          public void ReceiveGold(int amtOfGold)
          {
               Gold += amtOfGold;
          }

          public void SpendGold(int amtOfGold)
          {
               if(amtOfGold > Gold)
               {
                    throw new ArgumentOutOfRangeException($"{Name} only has {Gold} gold, and cannot spend {amtOfGold} gold.");
               }

               Gold -= amtOfGold;
          }

          public void AddItemToInventory(GameItem item)
          {
               Inventory.Add(item);

               if (item.IsUnique)
               {
                    GroupedInventory.Add(new GroupedInventoryItem(item, 1));
               }
               else
               {
                    if (!GroupedInventory.Any(gi => gi.Item.ItemTypeID == item.ItemTypeID))
                    {
                         GroupedInventory.Add(new GroupedInventoryItem(item, 0));
                    }

                    GroupedInventory.First(gi => gi.Item.ItemTypeID == item.ItemTypeID).Quantity++;
               }

               OnPropertyChanged(nameof(Weapons));
               OnPropertyChanged(nameof(Consumables));
               OnPropertyChanged(nameof(HasConsumable));

          }

          public void RemoveItemFromInventory(GameItem item)
          {
               Inventory.Remove(item);

               GroupedInventoryItem groupedInventoryItemToRemove = (item.IsUnique ?
                   GroupedInventory.FirstOrDefault(gi => gi.Item == item):
               GroupedInventory.FirstOrDefault(gi => gi.Item.ItemTypeID == item.ItemTypeID));

               if (groupedInventoryItemToRemove != null)
               {
                    if (groupedInventoryItemToRemove.Quantity == 1)
                    {
                         GroupedInventory.Remove(groupedInventoryItemToRemove);
                    }
                    else
                    {
                         groupedInventoryItemToRemove.Quantity--;
                    }
               }

               OnPropertyChanged(nameof(Weapons));
               OnPropertyChanged(nameof(Consumables));
               OnPropertyChanged(nameof(HasConsumable));
          }

          public void RemoveItemsFromInventory(List<ItemQuantity> itemQuantities)
          {
               foreach(ItemQuantity itemQuantity in itemQuantities)
               {
                    for(int i = 0; i < itemQuantity.Quantity; i++)
                    {
                         RemoveItemFromInventory(Inventory.First(item => item.ItemTypeID == itemQuantity.ItemID));
                    }
               }
          }

          public bool HasAllTheseItems(List<ItemQuantity> items)
          {
               foreach(ItemQuantity item in items)
               {
                    if(Inventory.Count(i => i.ItemTypeID == item.ItemID) < item.Quantity)
                    {
                         return false;
                    }
               }
               return true;
          }
          #endregion

          #region Private functions
          private void RaiseOnKilledEvent()
          {
               OnKilled?.Invoke(this, new System.EventArgs());
          }

          private void RaiseActionPerformedEvent(object sender, string result)
          {
               OnActionPerformed?.Invoke(this, result);
          }

          #endregion
     }
}
