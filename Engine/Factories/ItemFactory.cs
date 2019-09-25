using Engine.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Engine.Shared;

namespace Engine.Models
{
     static class ItemFactory
     {
          private const string GAME_DATA_FILENAME = ".\\GameData\\GameItems.xml";

          private static readonly List<GameItem> _standardGameItems = new List<GameItem>();
          
          static ItemFactory()
          {
               #region Before Xml
               //BuildWeapon(1001, "Pointy Stick", 1, 1, 2);
               //BuildWeapon(1002, "Rusty Sword", 5, 1, 3);

               //BuildWeapon(1501, "Snake Fangs", 0, 0, 2);
               //BuildWeapon(1502, "Rat Claws", 0, 0, 2);
               //BuildWeapon(1503, "Spider Fangs", 0, 0, 4);

               //BuildHealingItem(2001, "Granola Bar", 5, 2);

               //BuildMiscellaneousItem(3001, "Oats", 1);
               //BuildMiscellaneousItem(3002, "Honey", 2);
               //BuildMiscellaneousItem(3003, "Raisins", 2);

               //BuildMiscellaneousItem(9001, "Snake Fang", 1);
               //BuildMiscellaneousItem(9002, "Snakeskin", 2);
               //BuildMiscellaneousItem(9003, "Rat Tail", 1);
               //BuildMiscellaneousItem(9004, "Rat fur", 2);
               //BuildMiscellaneousItem(9005, "Spider fang", 1);
               //BuildMiscellaneousItem(9006, "Spider silk", 2);
               #endregion

               #region After Xml
               if (File.Exists(GAME_DATA_FILENAME))
               {
                    XmlDocument data = new XmlDocument();
                    data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                    LoadItemsFromNodes(data.SelectNodes("/GameItems/Weapons/Weapon"));
                    LoadItemsFromNodes(data.SelectNodes("/GameItems/HealingItems/HealingItem"));
                    LoadItemsFromNodes(data.SelectNodes("/GameItems/MiscellaneousItems/MiscellaneousItem"));
               }
               else
               {
                    throw new FileNotFoundException($"Missing data file {GAME_DATA_FILENAME}");
               }
               #endregion
          }

          public static GameItem CreateGameItem(int itemTypeID)
          {
               return _standardGameItems.FirstOrDefault(item => item.ItemTypeID == itemTypeID)?.Clone();
          }
          public static string ItemName(int itemTypeID)
          {
               return _standardGameItems.FirstOrDefault(i => i.ItemTypeID == itemTypeID)?.Name ?? "";
          }

          #region Before XML
          //private static void BuildMiscellaneousItem(int id, string name, int price)
          //{
          //     _standardGameItems.Add(new GameItem(GameItem.ItemCategory.Miscellaneous, id, name, price));
          //}

          //private static void BuildWeapon(int id, string name, int price, int minDam, int maxDam)
          //{
          //     GameItem weapon = new GameItem(GameItem.ItemCategory.Weapon, id, name, price, true);

          //     weapon.Action = new AttackWithWeapon(weapon, minDam, maxDam);

          //     _standardGameItems.Add(weapon);

          //}

          //private static void BuildHealingItem(int id, string name, int price, int hpToHeal)
          //{
          //     GameItem item = new GameItem(GameItem.ItemCategory.Consumable, id, name, price);
          //     item.Action = new Heal(item, hpToHeal);
          //     _standardGameItems.Add(item);
          //}

          #endregion

          #region After Xml

          private static void LoadItemsFromNodes(XmlNodeList nodes)
          {
               if (nodes == null)
               {
                    return;
               }

               foreach (XmlNode node in nodes)
               {
                    GameItem.ItemCategory itemCategory = DetermineItemCategory(node.Name);

                    GameItem gameItem =
                        new GameItem(itemCategory,
                                     node.AttributeAsInt("ID"),
                                     node.AttributeAsString("Name"),
                                     node.AttributeAsInt("Price"),
                                     itemCategory == GameItem.ItemCategory.Weapon);

                    if (itemCategory == GameItem.ItemCategory.Weapon)
                    {
                         gameItem.Action =
                             new AttackWithWeapon(gameItem,
                                                  node.AttributeAsInt("MinimumDamage"),
                                                  node.AttributeAsInt("MaximumDamage"));
                    }
                    else if (itemCategory == GameItem.ItemCategory.Consumable)
                    {
                         gameItem.Action =
                             new Heal(gameItem,
                                      node.AttributeAsInt("hpToHeal"));
                    }

                    _standardGameItems.Add(gameItem);
               }
          }

          private static GameItem.ItemCategory DetermineItemCategory(string itemType)
          {
               switch (itemType)
               {
                    case "Weapon":
                         return GameItem.ItemCategory.Weapon;
                    case "HealingItem":
                         return GameItem.ItemCategory.Consumable;
                    default:
                         return GameItem.ItemCategory.Miscellaneous;
               }
          }
          private static int GetXmlAttributeAsInt(XmlNode node, string attributeName)
          {
               return Convert.ToInt32(GetXmlAttribute(node, attributeName));
          }

          private static string GetXmlAttributeAsString(XmlNode node, string attributeName)
          {
               return GetXmlAttribute(node, attributeName);
          }

          private static string GetXmlAttribute(XmlNode node, string attributeName)
          {
               XmlAttribute attribute = node.Attributes?[attributeName];

               if (attribute == null)
               {
                    throw new ArgumentException($"The attribute '{attributeName}' does not exist");
               }

               return attribute.Value;
          }
          #endregion
     }
}
     