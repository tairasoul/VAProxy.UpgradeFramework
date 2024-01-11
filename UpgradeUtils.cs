using Devdog.General.UI;
using UIWindowPageFramework;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeFramework
{
    internal class UpgradeUtils
    {
        internal static GameObject CreateCategory(string name)
        {
            GameObject Category = new GameObject($"tairasoul.upgradeframework.category.{name}");
            ScrollRect rect = Category.AddComponent<ScrollRect>();
            GameObject ScrollbarVertical = Category.AddObject("Scrollbar Vertical");
            Scrollbar vertical = ScrollbarVertical.AddComponent<Scrollbar>();
            GameObject SlidingArea = ScrollbarVertical.AddObject("Sliding Area");
            GameObject Handle = SlidingArea.AddObject("Handle");
            Image handle = Handle.AddComponent<Image>();
            GameObject Viewport = Category.AddObject("Viewport");
            Viewport.AddComponent<Animator>();
            Viewport.AddComponent<CanvasGroup>();
            Viewport.AddComponent<Mask>();
            Viewport.AddComponent<AnimatorHelper>();
            Viewport.AddComponent<Image>().color = new Color(1, 1, 1, 0.0025f);
            GameObject Content = Viewport.AddObject("Content");
            handle.color = Color.clear;
            VerticalLayoutGroup group = Content.AddComponent<VerticalLayoutGroup>();
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;
            Category.AddComponent<RectTransform>();
            ScrollbarVertical.AddComponent<RectTransform>();
            SlidingArea.AddComponent<RectTransform>();
            rect.viewport = Viewport.GetComponent<RectTransform>() ?? Viewport.AddComponent<RectTransform>();
            rect.viewport.sizeDelta = new Vector2(1000, 700);
            rect.viewport.anchoredPosition = new Vector2(159.0417f, -82.6578f);
            rect.content = Content.AddComponent<RectTransform>();
            vertical.image = handle;
            vertical.direction = Scrollbar.Direction.TopToBottom;
            vertical.handleRect = Handle.AddComponent<RectTransform>();
            rect.scrollSensitivity = 25;
            return Category;
        }

        internal static GameObject CreateUpgrade(Upgrade upg)
        {
            GameObject upgrade = new GameObject($"tairasoul.upgradeframework.upgrade.{upg.Name}");
            Font OrbitronRegular = ComponentUtils.GetFont("Orbitron-Regular");
            RectTransform Upg = upgrade.AddComponent<RectTransform>();
            LayoutElement elem = upgrade.GetComponent<LayoutElement>() ?? upgrade.AddComponent<LayoutElement>();
            elem.minHeight = 220;
            elem.minWidth = 1100;
            Image UpgImg = upgrade.AddComponent<Image>();
            UpgImg.color = new Color(1, 1, 1, 0.0588f);
            Upg.sizeDelta = new Vector2(1100, 220);
            GameObject Upgrade = upgrade.AddObject("Upgrade");
            Image ButtonImg = Upgrade.AddComponent<Image>();
            ButtonImg.color = new Color(1, 1, 1, 0.0588f);
            RectTransform UpgRect = Upgrade.GetComponent<RectTransform>() ?? Upgrade.AddComponent<RectTransform>();
            UpgRect.anchoredPosition = new Vector2(948.7892f, -111.8394f);
            UpgRect.sizeDelta = new Vector2(200, 100);
            GameObject UpgradeButton = Upgrade.AddObject("UpgradeButton");
            RectTransform ButtonRect = UpgradeButton.AddComponent<RectTransform>();
            ButtonRect.sizeDelta = new Vector2(200, 100);
            ButtonRect.anchoredPosition = new Vector2(138.7582f, -90.9186f);
            Text UpgradeText = UpgradeButton.AddComponent<Text>();
            UpgradeText.font = OrbitronRegular;
            UpgradeText.fontSize = 27;
            UpgradeText.text = "Upgrade";
            GameObject Info = upgrade.AddObject("Info");
            RectTransform InfoRect = Info.AddComponent<RectTransform>();
            InfoRect.sizeDelta = new Vector2(200, 100);
            InfoRect.anchoredPosition = new Vector2(138.7582f, -90.9186f);
            GameObject Name = Info.AddObject("Name");
            RectTransform NameRect = Name.AddComponent<RectTransform>();
            NameRect.sizeDelta = new Vector2(200, 100);
            NameRect.anchoredPosition = new Vector2(103.8034f, -37.1999f);
            Text NameText = Name.AddComponent<Text>();
            NameText.font = OrbitronRegular;
            NameText.fontSize = 26;
            NameText.horizontalOverflow = HorizontalWrapMode.Overflow;
            NameText.text = upg.Name;
            GameObject Description = Info.AddObject("Description");
            RectTransform DescriptionRect = Description.AddComponent<RectTransform>();
            DescriptionRect.sizeDelta = new Vector2(700, 100);
            DescriptionRect.anchoredPosition = new Vector2(103.8034f, -67.9876f);
            Text DescriptionText = Description.AddComponent<Text>();
            DescriptionText.font = OrbitronRegular;
            DescriptionText.fontSize = 18;
            DescriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
            DescriptionText.verticalOverflow = VerticalWrapMode.Truncate;
            DescriptionText.text = upg.Description;
            DescriptionText.color = new Color(1, 1, 1, 0.7f);
            GameObject Level = Info.AddObject("Level");
            RectTransform LevelRect = Level.AddComponent<RectTransform>();
            LevelRect.sizeDelta = new Vector2(200, 100);
            LevelRect.anchoredPosition = new Vector2(107.2271f, -180.3161f);
            Text LevelText = Level.AddComponent<Text>();
            LevelText.font = OrbitronRegular;
            LevelText.fontSize = 20;
            LevelText.horizontalOverflow = HorizontalWrapMode.Overflow;
            LevelText.text = $"Level {upg.CurrentLevel}";
            LevelText.color = new Color(1, 1, 1, 0.6f);
            GameObject LevelCap = Info.AddObject("LevelCap");
            RectTransform LevelCapRect = LevelCap.AddComponent<RectTransform>();
            LevelCapRect.sizeDelta = new Vector2(200, 100);
            LevelCapRect.anchoredPosition = new Vector2(226.1422f, -180.3161f);
            Text LevelCapText = LevelCap.AddComponent<Text>();
            LevelCapText.font = OrbitronRegular;
            LevelCapText.fontSize = 20;
            LevelCapText.horizontalOverflow = HorizontalWrapMode.Overflow;
            LevelCapText.text = $"Level Cap: {upg.MaxLevel}";
            LevelCapText.color = new Color(1, 1, 1, 0.6f);
            GameObject Price = Info.AddObject("UpgradePrice");
            RectTransform PriceRect = Price.AddComponent<RectTransform>();
            PriceRect.sizeDelta = new Vector2(200, 100);
            PriceRect.anchoredPosition = new Vector2(914.1624f, -147.0102f);
            Text PriceText = Price.AddComponent<Text>();
            PriceText.font = OrbitronRegular;
            PriceText.fontSize = 20;
            PriceText.horizontalOverflow = HorizontalWrapMode.Overflow;
            PriceText.text = $"{upg.Price} Scrap";
            PriceText.color = new Color(1, 1, 1, 0.6f);
            return upgrade;
        }
    }
}
