using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tweaks
{
    internal class Testing
    {
        //[HarmonyPatch(typeof(Player), "Update")]
        class Player_Patch
        {
            public static void Postfix(Player __instance)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (ExtrasManager.activeGameMode == null)
                    {
                        //Logger.LogInfo("ExtrasManager.activeGameMode == null");
                        return;
                    }
                    string s = "activeGameMode " + ExtrasManager.activeGameMode.ID;
                    Main.logger.LogInfo(s);
                    DungeonEscapeUI.instance.ShowPopupMessage(s, duration: 1, fadeInSpeed: 11f, fadeSpeed: 11f);
                    //s = "DungeonEscape.currentFloor " + DungeonEscape.currentFloor;
                    //DungeonEscapeUI.instance.ShowPopupMessage(s, duration: 1, fadeInSpeed: 11f, fadeSpeed: 11f);
                    //ExtrasManager.activeGameMode = (ExtraGameMode)this.dungeonEscape;
                    //DungeonEscape.currentFloor = 9;
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    string s = "DungeonEscape.currentFloor " + DungeonEscape.currentFloor;
                    DungeonEscapeUI.instance.ShowPopupMessage(s, duration: 1, fadeInSpeed: 11f, fadeSpeed: 11f);
                }
            }
        }

        static void Test()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (DungeonEscape.activeDungeonEscape)
                {
                    int numFloors = DungeonEscape.activeDungeonEscape.floors.Count;
                    string s = "DungeonEscape floors " + numFloors;
                    DungeonEscapeUI.instance.ShowPopupMessage(s, duration: 1, fadeInSpeed: 11f, fadeSpeed: 11f);
                    DungeonEscapeFloorData floor9 = DungeonEscape.activeDungeonEscape.floors[numFloors - 2];
                    DungeonEscape.activeDungeonEscape.floors[numFloors - 1] = floor9;
                    foreach (var defd in DungeonEscape.activeDungeonEscape.floors)
                    {
                        Main.logger.LogInfo("DungeonEscapeFloorData " + defd.roomID);
                    }
                }
            }
        }

        //[HarmonyPatch(typeof(CutsceneManager))]
        class CutsceneManagerv_Patch
        {
            //[HarmonyPrefix]
            //[HarmonyPatch("SpawnAndPlayCutscene")]
            public static bool AwakePrefix(CutsceneManager __instance, Cutscene cutscene)
            {
                Main.logger.LogInfo("CutsceneManager SpawnAndPlayCutscene " + cutscene.ID);
                if (ExtrasManager.activeGameMode == null)
                    Main.logger.LogInfo("CutsceneManager SpawnAndPlayCutscene activeGameMode == null");
                //return true;
                if (ExtrasManager.activeGameMode.ID == "DUNGEONESCAPE" && cutscene.ID == "start")
                {
                    //if (File.Exists(dungeonEscapeSaveFilePath))
                    //    return false;
                }
                return true;
            }
        }

    }
}
