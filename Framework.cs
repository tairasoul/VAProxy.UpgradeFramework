using FluffyUnderware.DevTools.Extensions;
using System;
using System.Linq;
using UIWindowPageFramework;
using UnityEngine;
using UnityEngine.UI;
using UpgradeFramework.Extensions;

namespace UpgradeFramework
{
    public class Framework
    {
        private static Upgrade[] RegisteredUpgrades = Array.Empty<Upgrade>();
        private static Category[] Categories = Array.Empty<Category>();
        
        public static void AddUpgrade(Upgrade upgrade)
        {
            RegisteredUpgrades = RegisteredUpgrades.Append(upgrade).ToArray();
        }

        public static void AddUpgrade(string Name, string Identifier, string Category, string Description, Action<int> Upgraded, int DefaultLevel, int MaxLevel, int Price)
        {
            Upgrade upgrade = new Upgrade(Name, Identifier, Category, Description, Upgraded, DefaultLevel, MaxLevel, Price);
            RegisteredUpgrades = RegisteredUpgrades.Append(upgrade).ToArray();
        }

        public static void RemoveUpgrade(Upgrade upgrade)
        {
            RegisteredUpgrades = RegisteredUpgrades.Remove(upgrade);
        }

        public static void RemoveUpgrade(string Identifier, string Category)
        {
            Upgrade upg = RegisteredUpgrades.Find((Upgrade val) =>
            {
                if (val.Identifier == Identifier && val.Category == Category) return true;
                return false;
            });
            RegisteredUpgrades = RegisteredUpgrades.Remove(upg);
        }

        internal static void CheckCategory(Upgrade toCheck)
        {
            foreach (Category category in Categories)
            {
                if (category.Name == toCheck.Category)
                    return;
            }
            GameObject CategoryButton = ComponentUtils.CreateButton(toCheck.Category, $"tairasoul.upgradeframework.category.{toCheck.Category}.button");
            Category toAdd = new Category(toCheck.Category, CategoryButton.GetComponent<Button>());
            Categories = Categories.Append(toAdd).ToArray();
        }
    }

    public class Upgrade
    {
        public string Name;
        public string Identifier;
        public string Category;
        public string Description;
        private readonly Action<int> Upgraded;
        private int CurrentLevel;
        public int MaxLevel;
        public int Price;

        public Upgrade(string Name, string Identifier, string Category, string Description, Action<int> Upgraded, int DefaultLevel, int MaxLevel, int Price)
        {
            this.Name = Name;
            this.Identifier = Identifier;
            this.Category = Category;
            this.Description = Description;
            this.Upgraded = Upgraded;
            CurrentLevel = DefaultLevel;
            this.MaxLevel = MaxLevel;
            this.Price = Price;
        }

        public void InvokeUpgrade()
        {
            if (CurrentLevel < MaxLevel)
            {
                Upgraded.Invoke(CurrentLevel++);
            }
        }
    }

    internal class Category
    {
        public string Name;
        internal Button button;
        internal GameObject SubObjects;

        public Category(string Name, Button button, GameObject SubObjects)
        {
            this.Name = Name;
            this.button = button;
            this.SubObjects = SubObjects;
        }

        public void AddObject(GameObject obj)
        {
            obj.transform.SetParent(SubObjects.transform);
        }

        public void RemoveObject(GameObject obj)
        {
            obj.transform.SetParent(null);
        }
    }
}
