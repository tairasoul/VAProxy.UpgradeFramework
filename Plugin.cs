using BepInEx;
using BepInEx.Logging;
using Devdog.General.UI;
using System;
using System.Collections;
using System.Linq;
using UIWindowPageFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Pages = UIWindowPageFramework.Framework;

namespace UpgradeFramework
{
    internal class PluginInfo
    {
        public const string GUID = "tairasoul.upgradeframework";
        public const string Name = "UpgradeFramework";
        public const string Version = "1.0.2";
    }
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    internal class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal static GameObject RegisteredWindow = null;
        internal static GameObject IngameWindow = null;
        internal static GameObject ObjectStorage = null;
        internal static ManualLogSource Log;
        internal static event Action<GameObject> CategoryCallback;
        internal static event Action<GameObject> UpgradeCallback;
        void Awake()
        {
            Instance = this;
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.GUID} ({PluginInfo.Name}) version {PluginInfo.Version} loaded.");
        }

        void Start() => StartCoroutine(Init());
        void OnDestroy() 
        {
            Logger.LogError("UpgradeFramework.Plugin should not be getting destroyed! Is hideManagerObject disabled?");
            Logger.LogWarning("UpgradeFramework is initializing with backup method.");
            GameObject obj = new GameObject("TempMonoBehaviour");
            DontDestroyOnLoad(obj);
            obj.AddComponent<TempBehaviour>().Init();
        }

        T Find<T>(Func<T, bool> predicate)
        {
            foreach (T find in GameObject.FindObjectsOfTypeAll(typeof(T)).Cast<T>())
            {
                if (predicate(find)) return find;
            }
            return default;
        }

        void SetupHeader(GameObject RegisteredWindow)
        {
            Logger.LogInfo($"Setting up header for {RegisteredWindow}");
            try
            {
                GameObject Header = RegisteredWindow.Find("Header"); 
                CanvasRenderer rend = Header.GetComponent<CanvasRenderer>() ?? Header.AddComponent<CanvasRenderer>();
                rend.materialCount = 1;
                Material origin = Find((Material m) =>
                {
                    return m.name == "Default UI Material";
                });
                Material newM = new Material(origin)
                {
                    name = "Modified UI Material",
                    renderQueue = origin.renderQueue + 1
                };
                rend.SetMaterial(newM, 0);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        void CreateCategoryButtonsObj(GameObject RegisteredWindow)
        {
            Logger.LogInfo($"Creating category buttons for {RegisteredWindow}");
            GameObject CategoryButtons = RegisteredWindow.AddObject("CategoryButtons");
            RectTransform ButtonsRect = CategoryButtons.AddComponent<RectTransform>();
            ButtonsRect.sizeDelta = new Vector2(100, 100);
            ButtonsRect.anchoredPosition = new Vector2(0, 100);
            GameObject ScrollbarVertical = CategoryButtons.AddObject("Scrollbar Vertical");
            RectTransform VerticalRect = ScrollbarVertical.AddComponent<RectTransform>();
            ScrollRect rect = CategoryButtons.AddComponent<ScrollRect>();
            Scrollbar vertical = ScrollbarVertical.AddComponent<Scrollbar>();
            GameObject SlidingArea = ScrollbarVertical.AddObject("Sliding Area");
            RectTransform SlidingRect = SlidingArea.AddComponent<RectTransform>();
            GameObject Handle = SlidingArea.AddObject("Handle");
            RectTransform HandleRect = Handle.AddComponent<RectTransform>();
            Image HandleImage = Handle.AddComponent<Image>();
            HandleImage.color = Color.clear;
            vertical.direction = Scrollbar.Direction.TopToBottom;
            vertical.handleRect = HandleRect;
            vertical.image = HandleImage;
            GameObject Viewport = CategoryButtons.AddObject("Viewport");
            RectTransform ViewportRect = Viewport.AddComponent<RectTransform>();
            ViewportRect.anchoredPosition = new Vector2(-580.1204f, -183.5188f);
            ViewportRect.sizeDelta = new Vector2(300, 700);
            Viewport.AddComponent<Animator>();
            Viewport.AddComponent<CanvasGroup>();
            Viewport.AddComponent<Mask>();
            Viewport.AddComponent<AnimatorHelper>();
            Viewport.AddComponent<Image>().color = new Color(1, 1, 1, 0.0025f);
            GameObject Content = Viewport.AddObject("Content");
            RectTransform ContentRect = Content.AddComponent<RectTransform>();
            ContentRect.anchoredPosition = new Vector2(-84.7709f, -0.0009f);
            ContentRect.sizeDelta = new Vector2(100, 700);
            VerticalLayoutGroup group = Content.AddComponent<VerticalLayoutGroup>();
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;
            group.childAlignment = TextAnchor.UpperLeft;
            rect.content = ContentRect;
            rect.viewport = ViewportRect;
            rect.horizontal = false;
            rect.verticalScrollbar = vertical;
            rect.scrollSensitivity = 25;
        }

        internal IEnumerator Init()
        {
            ObjectStorage = new GameObject("UpgradeFramework.ObjectStorage");
            DontDestroyOnLoad(ObjectStorage);
            while (!Pages.Ready)
            {
                yield return null;
            }
            while (SceneManager.GetActiveScene().name != "Menu")
            {
                yield return null;
            }
            try
            {
                RegisteredWindow = Pages.CreateWindow("UPGRADES");
                CreateCategoryButtonsObj(RegisteredWindow);
                SetupHeader(RegisteredWindow);
                GameObject CategoryStorage = RegisteredWindow.AddObject("Categories");
                CategoryStorage.AddComponent<RectTransform>();
                Pages.RegisterWindow(RegisteredWindow, (GameObject window) =>
                {
                    SetupHeader(window);
                    IngameWindow = window;
                    GameObject Header = window.Find("Header").AddObject("PageHeader");
                    RectTransform Page = Header.AddComponent<RectTransform>();
                    Text HeaderText = Header.GetComponent<Text>() ?? Header.AddComponent<Text>();
                    HeaderText.alignment = TextAnchor.MiddleCenter;
                    HeaderText.fontSize = 24;
                    HeaderText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    HeaderText.font = ComponentUtils.GetFont("Orbitron-Bold");
                    Framework.HeaderText = HeaderText;
                    Page.sizeDelta = new Vector2(322, 42);
                    Page.anchoredPosition = new Vector2(110, 77);
                    Log.LogInfo("Invoking CategoryCallback");
                    CategoryCallback.Invoke(window);
                    Log.LogInfo("Invoking UpgradeCallback");
                    UpgradeCallback.Invoke(window);
                });
                Framework.Ready = true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
        }
    }

    class TempBehaviour : MonoBehaviour
    {
        public void Init()
        {
            StartCoroutine(Plugin.Instance.Init());
        }
    }
}
