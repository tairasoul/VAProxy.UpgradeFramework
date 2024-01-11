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
            CheckCategory(upgrade);
            RegisteredUpgrades = RegisteredUpgrades.Append(upgrade).ToArray();
        }

        public static void AddUpgrade(string Name, string Identifier, string Category, string Description, Action<int> Upgraded, int DefaultLevel, int MaxLevel, int Price)
        {
            Upgrade upgrade = new Upgrade(Name, Identifier, Category, Description, Upgraded, DefaultLevel, MaxLevel, Price);
            CheckCategory(upgrade);
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
            Plugin.Log.LogInfo(toCheck);
            foreach (Category category in Categories)
            {
                if (category.Name == toCheck.Category)
                {
                    UpgradeAdded(toCheck);
                    return;
                }
            }
            GameObject CategoryButton = ComponentUtils.CreateButton(toCheck.Category, $"tairasoul.upgradeframework.button.{toCheck.Category}");
            Text ButtonText = CategoryButton.Find("ItemName").GetComponent<Text>();
            ButtonText.font = ComponentUtils.GetFont("Orbitron-Regular");
            GameObject Category = UpgradeUtils.CreateCategory(toCheck.Category);
            LayoutElement elem = CategoryButton.GetComponent<LayoutElement>() ?? CategoryButton.AddComponent<LayoutElement>();
            elem.minHeight = 50;
            elem.minWidth = 300;
            CategoryButton.SetParent(Plugin.ObjectStorage, true);
            Category.SetParent(Plugin.ObjectStorage, true);
            Category toAdd = new Category(toCheck.Category, CategoryButton.GetComponent<Button>(), Category);
            Categories = Categories.Append(toAdd).ToArray();
            UpgradeAdded(toCheck);
            CategoryAdded(toAdd);
        }

        internal static void UpgradeAdded(Upgrade upgrade)
        {
            if (Plugin.RegisteredWindow != null)
            {
                Category category = Categories.Find((Category cat) =>
                {
                    return cat.Name == upgrade.Category;
                });
                try
                {
                    GameObject CategoryObj = category.CategoryObject;
                    Plugin.Log.LogInfo(CategoryObj);
                    GameObject UpgradeObject = UpgradeUtils.CreateUpgrade(upgrade);
                    Plugin.Log.LogInfo(UpgradeObject);
                    UpgradeObject.SetParent(CategoryObj.Find("Viewport/Content"), false);
                    Plugin.Log.LogInfo("Creation for RegisteredWindow done.");
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }
            if (Plugin.IngameWindow != null)
            {
                Category category = Categories.Find((Category cat) =>
                {
                    return cat.Name == upgrade.Category;
                });
                try
                {
                    Plugin.Log.LogInfo($"Categories/tairasoul.upgradeframework.category.{category.Name}");
                    GameObject CategoryObj = Plugin.IngameWindow.Find($"Categories/tairasoul.upgradeframework.category.{category.Name}");
                    Plugin.Log.LogInfo(CategoryObj);
                    GameObject UpgradeObject = UpgradeUtils.CreateUpgrade(upgrade);
                    Plugin.Log.LogInfo(UpgradeObject);
                    UpgradeObject.SetParent(CategoryObj.Find("Viewport/Content"), false);
                    Plugin.Log.LogInfo("Creation for IngameWindow done.");
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }
        }

        internal static void CategoryAdded(Category category)
        {
            Plugin.RunOnBoth((GameObject window) =>
            {
                if (window != null)
                {
                    try
                    {
                        GameObject button = category.ButtonObject.Instantiate();
                        GameObject categoryObj = category.CategoryObject.Instantiate();
                        categoryObj.SetActive(false);
                        button.name = button.name.Replace("(Clone)", "");
                        categoryObj.name = categoryObj.name.Replace("(Clone)", "");
                        button.SetParent(window.Find("CategoryButtons/Viewport/Content"), false);
                        categoryObj.SetParent(window.Find("Categories"), true);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.LogError(ex);
                    }
                }
            });
        }
    }

    public class Upgrade
    {
        public string Name;
        public string Identifier;
        public string Category;
        public string Description;
        private readonly Action<int> Upgraded;
        public int CurrentLevel;
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

        public override string ToString()
        {
            return $"{Name} {Identifier} {Category} (UpgradeFramework.Upgrade)";
        }
    }

    internal class Category
    {
        public string Name;
        internal Button button;
        internal GameObject CategoryObject;
        internal GameObject ButtonObject;

        public Category(string Name, Button button, GameObject CategoryObject)
        {
            this.Name = Name;
            this.button = button;
            ButtonObject = button.gameObject;
            this.CategoryObject = CategoryObject;
        }
    }
}
