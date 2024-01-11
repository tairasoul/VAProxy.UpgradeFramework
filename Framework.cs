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
        internal static Text HeaderText = null;
        
        public static void AddUpgrade(Upgrade upgrade)
        {
            CheckCategory(upgrade);
            RegisteredUpgrades = RegisteredUpgrades.Append(upgrade).ToArray();
        }

        /// <summary>
        /// Create a new Upgrade.
        /// </summary>
        /// <param name="Name">The name of the upgrade.</param>
        /// <param name="Identifier">Identifier used for saving upgrade data in PlayerPrefs.</param>
        /// <param name="Category">Category to put the upgrade under.</param>
        /// <param name="Description">Description of the upgrade.</param>
        /// <param name="Upgraded">
        /// The function to call whenever Upgrade is pressed and the player has enough scrap.
        /// Parameter 1 is the current level, and parameter 2 is the current price.
        /// Returns the new price.</param>
        /// <param name="DefaultLevel">Starting level for the upgrade.</param>
        /// <param name="MinLevel">Minimum level.</param>
        /// <param name="MaxLevel">Max level the upgrade can reach.</param>
        /// <param name="Price">The price for the initial upgrade.</param>
        /// <param name="Regain">How much scrap is returned for a downgrade.</param>

        public static void AddUpgrade(string Name, string Identifier, string Category, string Description, Func<int, int, int[]> Upgraded, Func<int, int, int[]> Downgraded, int MinLevel, int DefaultLevel, int MaxLevel, int Price, int Regain)
        {
            Upgrade upgrade = new Upgrade(Name, Identifier, Category, Description, Upgraded, Downgraded, DefaultLevel, MinLevel, MaxLevel, Price, Regain);
            CheckCategory(upgrade);
            RegisteredUpgrades = RegisteredUpgrades.Append(upgrade).ToArray();
        }

        /*public static void RemoveUpgrade(Upgrade upgrade)
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
        }*/

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
            try
            {
                GameObject CategoryButton = ComponentUtils.CreateButton(toCheck.Category, $"tairasoul.upgradeframework.button.{toCheck.Category}");
                Text ButtonText = CategoryButton.Find("ItemName").GetComponent<Text>();
                ButtonText.font = ComponentUtils.GetFont("Orbitron-Regular");
                RectTransform ButtonRect = CategoryButton.Find("ItemName").GetComponent<RectTransform>();
                ButtonRect.anchoredPosition = new Vector2(-11, 0);
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
            catch (Exception ex)
            {
                Plugin.Log.LogError(ex);
            }
        }

        internal static void UpgradeAdded(Upgrade upgrade)
        {
            Plugin.UpgradeCallback += (GameObject window) =>
            {
                Category category = Categories.Find((Category cat) =>
                {
                    return cat.Name == upgrade.Category;
                });
                try
                {
                    Plugin.Log.LogInfo($"Categories/tairasoul.upgradeframework.category.{category.Name}");
                    GameObject CategoryObj = window.Find($"Categories/tairasoul.upgradeframework.category.{category.Name}");
                    Plugin.Log.LogInfo(CategoryObj);
                    GameObject UpgradeObject = UpgradeUtils.CreateUpgrade(upgrade);
                    Plugin.Log.LogInfo(UpgradeObject);
                    UpgradeObject.SetParent(CategoryObj.Find("Viewport/Content"), false);
                    upgrade.UpgradeObject = UpgradeObject;
                    Button Button = UpgradeObject.Find("Upgrade").GetComponent<Button>();
                    Button.onClick.AddListener(upgrade.InvokeUpgrade);
                    Button down = UpgradeObject.Find("Downgrade").GetComponent<Button>();
                    down.onClick.AddListener(upgrade.InvokeDowngrade);
                    upgrade.InitialCreation();
                    Plugin.Log.LogInfo("Creation for IngameWindow done.");
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            };
        }

        internal static void CategoryAdded(Category category)
        {
            Plugin.CategoryCallback += (GameObject window) =>
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
                        categoryObj.SetParent(window.Find("Categories"), false);
                        categoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        categoryObj.transform.localScale = new Vector3(1, 1, 1);
                        button.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            try
                            {
                                foreach (GameObject cate in window.Find("Categories").GetChildren())
                                {
                                    cate.SetActive(false);
                                }
                                HeaderText.text = category.Name;
                                categoryObj.SetActive(true);
                            }
                            catch (Exception ex)
                            {
                                Plugin.Log.LogError(ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.LogError(ex);
                    }
                }
            };
        }
    }

    public class Upgrade
    {
        /// <summary>
        /// The name of the upgrade.
        /// </summary>
        public string Name;
        /// <summary>
        /// Identifier used for saving upgrade data in PlayerPrefs.
        /// </summary>
        public string Identifier;
        /// <summary>
        /// Category to put the upgrade under.
        /// </summary>
        public string Category;
        /// <summary>
        /// Description of the upgrade.
        /// </summary>
        public string Description;
        /// <summary>
        /// The function to call whenever Upgrade is pressed and the player has enough scrap.
        /// Parameter 1 is the current level, and parameter 2 is the current price.
        /// Returns an array, with the first element being the new price and second being the new downgrade return value.
        /// </summary>
        private readonly Func<int, int, int[]> Upgraded;
        /// <summary>
        /// The function to call whenever Downgrade is pressed
        /// Parameter 1 is the new level, and parameter 2 is the current price.
        /// Returns an array, with the first element being the new price and second element being the new downgrade return value.
        /// </summary>
        private readonly Func<int, int, int[]> Downgraded;
        /// <summary>
        /// Current or starting level.
        /// </summary>
        public int CurrentLevel;
        /// <summary>
        /// Max level the upgrade can reach.
        /// </summary>
        public int MaxLevel;
        /// <summary>
        /// The price for the initial upgrade.
        /// </summary>
        public int Price;
        /// <summary>
        /// Scrap regained for downgrading.
        /// </summary>
        public int DowngradeRegain;
        /// <summary>
        /// The minimum level this upgrade can have.
        /// </summary>
        public int MinLevel;
        internal GameObject UpgradeObject = null;

        /// <summary>
        /// Create a new Upgrade.
        /// </summary>
        /// <param name="Name">The name of the upgrade.</param>
        /// <param name="Identifier">Identifier used for saving upgrade data in PlayerPrefs.</param>
        /// <param name="Category">Category to put the upgrade under.</param>
        /// <param name="Description">Description of the upgrade.</param>
        /// <param name="Upgraded">
        /// The function to call whenever Upgrade is pressed and the player has enough scrap.
        /// Parameter 1 is the current level, and parameter 2 is the current price.
        /// Returns the new price.</param>
        /// <param name="DefaultLevel">Starting level for the upgrade.</param>
        /// <param name="MinLevel">Minimum level.</param>
        /// <param name="MaxLevel">Max level the upgrade can reach.</param>
        /// <param name="Price">The price for the initial upgrade.</param>
        /// <param name="Regain">How much scrap is returned for a downgrade.</param>
        public Upgrade(string Name, string Identifier, string Category, string Description, Func<int, int, int[]> Upgraded, Func<int, int, int[]> Downgraded, int DefaultLevel, int MinLevel, int MaxLevel, int Price, int Regain)
        {
            this.Name = Name;
            this.Identifier = Identifier;
            this.Category = Category;
            this.Description = Description;
            this.Upgraded = Upgraded;
            this.Downgraded = Downgraded;
            this.MinLevel = MinLevel;
            CurrentLevel = DefaultLevel;
            this.MaxLevel = MaxLevel;
            this.Price = Price;
            DowngradeRegain = Regain;
            if (PlayerPrefs.GetInt($"{Identifier}.Price", -1000) != -1000)
            {
                this.Price = PlayerPrefs.GetInt($"{Identifier}.Price", -1000);
            }
            if (PlayerPrefs.GetInt($"{Identifier}.Level", -1000) != -1000)
            {
                CurrentLevel = PlayerPrefs.GetInt($"{Identifier}.Level", -1000);
                this.Upgraded.Invoke(CurrentLevel, this.Price);
            }
            if (PlayerPrefs.GetInt($"{Identifier}.DowngradeRegain", -1000) != -1000)
            {
                DowngradeRegain = PlayerPrefs.GetInt($"{Identifier}.DowngradeRegain", -1000);
            }
        }

        public void InitialCreation()
        {
            try
            {
                if (CurrentLevel >= MaxLevel)
                {
                    Text priceText = UpgradeObject.Find("Info/UpgradePrice").GetComponent<Text>();
                    Plugin.Log.LogInfo("Max level reached. Informing user.");
                    priceText.text = "";
                    Text upgradeText = UpgradeObject.Find("Upgrade/UpgradeButton").GetComponent<Text>();
                    upgradeText.text = "Max level";
                }
                if (CurrentLevel <= MinLevel)
                {
                    Text priceText = UpgradeObject.Find("Info/DowngradeRegain").GetComponent<Text>();
                    Plugin.Log.LogInfo("Minimum level reached. Informing user.");
                    priceText.text = "";
                    Text upgradeText = UpgradeObject.Find("Downgrade/DowngradeButton").GetComponent<Text>();
                    upgradeText.text = "Min level";
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError("Error in InitialCreation!");
                Plugin.Log.LogError(ex);
            }
        }

        public void InvokeUpgrade()
        {
            if (CurrentLevel < MaxLevel)
            {
                Text priceText = UpgradeObject.Find("Info/UpgradePrice").GetComponent<Text>();
                Text downgradeText = UpgradeObject.Find("Info/DowngradeRegain").GetComponent<Text>();
                int[] prices = Upgraded.Invoke(CurrentLevel++, Price);
                UpgradeObject.Find("Downgrade/DowngradeButton").GetComponent<Text>().text = "Downgrade";
                try
                {
                    PlayerPrefs.SetInt($"{Identifier}.Level", CurrentLevel);
                    PlayerPrefs.SetInt($"{Identifier}.Price", Price);
                    PlayerPrefs.SetInt($"{Identifier}.DowngradeRegain", DowngradeRegain);
                    int scrapValue = UpgradeUtils.GetScrapValue();
                    scrapValue -= Price;
                    UpgradeUtils.SetScrapValue(scrapValue);
                    PlayerPrefs.Save();
                    Price = prices[0];
                    DowngradeRegain = prices[1];
                    priceText.text = $"-{Price} Scrap";
                    downgradeText.text = $"+{DowngradeRegain} Scrap";
                    Text LevelText = UpgradeObject.Find("Info/Level").GetComponent<Text>();
                    LevelText.text = $"Level {CurrentLevel}";
                    if (CurrentLevel >= MaxLevel)
                    {
                        Plugin.Log.LogInfo("Max level reached. Informing user.");
                        priceText.text = "";
                        Text upgradeText = UpgradeObject.Find("Upgrade/UpgradeButton").GetComponent<Text>();
                        upgradeText.text = "Max level";
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }
        }

        public void InvokeDowngrade()
        {
            if (CurrentLevel > MinLevel)
            {
                Text priceText = UpgradeObject.Find("Info/UpgradePrice").GetComponent<Text>();
                Text downgradeText = UpgradeObject.Find("Info/DowngradeRegain").GetComponent<Text>();
                int[] prices = Downgraded.Invoke(CurrentLevel--, Price);
                UpgradeObject.Find("Upgrade/UpgradeButton").GetComponent<Text>().text = "Upgrade";
                try
                {
                    PlayerPrefs.SetInt($"{Identifier}.Level", CurrentLevel);
                    PlayerPrefs.SetInt($"{Identifier}.Price", Price);
                    PlayerPrefs.SetInt($"{Identifier}.DowngradeRegain", DowngradeRegain);
                    int scrapValue = UpgradeUtils.GetScrapValue();
                    scrapValue += DowngradeRegain;
                    Price = prices[0];
                    DowngradeRegain = prices[1];
                    downgradeText.text = $"+{DowngradeRegain} Scrap";
                    priceText.text = $"-{Price} Scrap";
                    UpgradeUtils.SetScrapValue(scrapValue);
                    PlayerPrefs.Save();
                    Text LevelText = UpgradeObject.Find("Info/Level").GetComponent<Text>();
                    LevelText.text = $"Level {CurrentLevel}";
                    if (CurrentLevel <= MinLevel)
                    {
                        Plugin.Log.LogInfo("Min level reached. Informing user.");
                        downgradeText.text = "";
                        Text upgradeText = UpgradeObject.Find("Downgrade/DowngradeButton").GetComponent<Text>();
                        upgradeText.text = "Min level";
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }
            /*if (CurrentLevel < MaxLevel)
            {
                Text priceText = UpgradeObject.Find("Info/UpgradePrice").GetComponent<Text>();
                Price = Upgraded.Invoke(CurrentLevel++, Price);
                priceText.text = $"{Price} Scrap";
                try
                {
                    PlayerPrefs.SetInt($"{Identifier}.Level", CurrentLevel);
                    PlayerPrefs.SetInt($"{Identifier}.Price", Price);
                    int scrapValue = UpgradeUtils.GetScrapValue();
                    scrapValue -= Price;
                    UpgradeUtils.SetScrapValue(scrapValue);
                    Text LevelText = UpgradeObject.Find("Info/Level").GetComponent<Text>();
                    LevelText.text = $"Level {CurrentLevel}";
                    if (CurrentLevel >= MaxLevel)
                    {
                        Plugin.Log.LogInfo("Max level reached. Informing user.");
                        Button Button = UpgradeObject.Find("Upgrade").GetComponent<Button>();
                        priceText.text = "";
                        Text upgradeText = UpgradeObject.Find("Upgrade/UpgradeButton").GetComponent<Text>();
                        upgradeText.text = "Max level";
                        Button.onClick.RemoveAllListeners();
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError(ex);
                }
            }*/
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
