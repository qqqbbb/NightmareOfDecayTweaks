using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace Tweaks
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "qqqbbb.NightmareOfDecay.tweaks";
        public const string PLUGIN_NAME = "Tweaks";
        public const string PLUGIN_VERSION = "1.0.0";

        public static ConfigFile config;
        public static ConfigEntry<string> abilityTypes;
        public static ManualLogSource logger;

        private void Awake()
        {
            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
            config = this.Config;
            Tweaks.Config.Bind();
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void Start()
        {
            //Logger.LogInfo("tweaks Start ");
        }


        public void Update()
        {

        }


    }
}
