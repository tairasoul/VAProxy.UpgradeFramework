using BepInEx;
using System;
using System.Collections;
using UnityEngine;
using Pages = UIWindowPageFramework.Framework;

namespace UpgradeFramework
{
    internal class PluginInfo
    {
        public const string GUID = "tairasoul.upgradeframework";
        public const string Name = "UpgradeFramework.Indev";
        public const string Version = "1.0.0";
    }
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    internal class Plugin : BaseUnityPlugin
    {
        internal static GameObject RegisteredWindow = null;
        internal static GameObject IngameWindow = null;
        void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.GUID} ({PluginInfo.Name}) version {PluginInfo.Version} loaded.");
        }

        void Start() => StartCoroutine(Init());
        void OnDestroy() => Logger.LogError("UpgradeFramework.Plugin should not be getting destroyed! Is hideManagerObject disabled?");

        IEnumerator Init()
        {
            while (!Pages.Ready)
            {
                yield return null;
            }
            RegisteredWindow = Pages.CreateWindow("Upgrades");
            Pages.RegisterWindow(RegisteredWindow, (GameObject window) =>
            {
                IngameWindow = window;
            });
        }

        internal static void RunOnBoth(Action<GameObject> action)
        {
            action.Invoke(RegisteredWindow);
            action.Invoke(IngameWindow);
        }
    }
}
