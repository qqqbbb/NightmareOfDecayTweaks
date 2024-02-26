using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweaks
{
    internal class Unlocker
    {
        static public void UnlockExtras()
        {
            if (!ExtrasManager.currentExtraData.unlockIDs.Contains("submachinegun"))
                ExtrasManager.currentExtraData.unlockIDs.Add("submachinegun");

            if (!ExtrasManager.currentExtraData.unlockIDs.Contains("randomizer"))
                ExtrasManager.currentExtraData.unlockIDs.Add("randomizer");

            if (!ExtrasManager.currentExtraData.unlockIDs.Contains("machete"))
                ExtrasManager.currentExtraData.unlockIDs.Add("machete");

            if (!ExtrasManager.currentExtraData.unlockIDs.Contains("rocketlauncher"))
                ExtrasManager.currentExtraData.unlockIDs.Add("rocketlauncher");

            ExtrasManager.currentExtraData.gameFinishedNormal = true;
            ExtrasManager.currentExtraData.gameFinishedHard = true;
            ExtrasManager.SaveExtraData();
        }

        public static bool IsEveryExtraUnlocked()
        {
            return ExtrasManager.currentExtraData.unlockIDs.Contains("submachinegun") && ExtrasManager.currentExtraData.unlockIDs.Contains("randomizer") && ExtrasManager.currentExtraData.unlockIDs.Contains("machete") && ExtrasManager.currentExtraData.unlockIDs.Contains("rocketlauncher") && ExtrasManager.currentExtraData.gameFinishedNormal && ExtrasManager.currentExtraData.gameFinishedHard;
        }

        [HarmonyPatch(typeof(ExtrasManager))]
        class ExtrasManager_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("InitExtraData")]
            public static void InitExtraDataPostfix(ExtrasManager __instance)
            {

                Config.unlockExtras.Value = IsEveryExtraUnlocked();
                //Main.log.LogInfo(" Player Die DeleteDungeonEscapeSaveFile");

            }
        }

    }
}
