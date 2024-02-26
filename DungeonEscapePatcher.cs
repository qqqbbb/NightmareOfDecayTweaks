using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Tweaks
{
    internal class DungeonEscapePatcher
    {
        static string dungeonEscapeSaveFilePath = GameSaveLoad.DirectoryPath + Path.DirectorySeparatorChar + "DungeonEscape.sav";
        static string extrasSaveFilePath = GameSaveLoad.DirectoryPath + Path.DirectorySeparatorChar + "extra.unlock";
        //public static int dungeonEscapeFloorOverride = 0;
        //public static bool dungeonEscapeFloorOverriden;

        public static void SaveDungeonEscape()
        {
            Main.logger.LogInfo(" SaveDungeonEscape Floor " + DungeonEscape.currentFloor);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            if (!Directory.Exists(GameSaveLoad.DirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(GameSaveLoad.DirectoryPath);
                }
                catch (Exception ex)
                {
                    Main.logger.LogInfo($"SaveDungeonEscape CreateDirectory: {ex.Message}");
                }
            }
            FileStream fileStream = null;
            try
            {
                fileStream = File.Create(dungeonEscapeSaveFilePath);
            }
            catch (Exception ex)
            {
                Main.logger.LogInfo($"SaveDungeonEscape File.Create: {ex.Message}");
            }
            string str = null;
            try
            {
                str = JsonConvert.SerializeObject(new DungeonEscapeSaveData(DungeonEscape.currentFloor, ExtraGameMode.credits, Player.instance.playerStats.health, DungeonEscape.kills, GameManager.currentGameData.playTime, PlayerEquipment.equippedItem.ID, ControlManager.hotkeyItems, PlayerInventorySaveData.Save()));
            }
            catch (Exception ex)
            {
                Main.logger.LogInfo($"SaveDungeonEscape JsonConvert.SerializeObject: {ex.Message}");
                fileStream.Close();
            }
            try
            {
                binaryFormatter.Serialize(fileStream, str);
            }
            catch (Exception ex)
            {
                Main.logger.LogInfo($"SaveDungeonEscape binaryFormatter.Serialize: {ex.Message}");
            }
            finally
            {
                fileStream.Close();
            }
        }

        public static void LoadDungeonEscape()
        {
            Main.logger.LogInfo(" LoadDungeonEscape Floor " );
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            if (!File.Exists(dungeonEscapeSaveFilePath))
            {
                Main.logger.LogInfo("No saved DungeonEscape ");
                return;
            }
            FileStream fileStream;
            try
            {
                fileStream = File.Open(dungeonEscapeSaveFilePath, FileMode.Open);
            }
            catch (Exception ex)
            {
                Main.logger.LogInfo($"LoadDungeonEscape File.Open: {ex.Message}");
                return;
            }
            string str;
            try
            {
                str = binaryFormatter.Deserialize(fileStream) as string;
            }
            catch (Exception ex)
            {
                Main.logger.LogInfo($"LoadDungeonEscape binaryFormatter.Deserialize: {ex.Message}");
                DeleteDungeonEscapeSaveFile();
                return;
            }
            finally
            {
                fileStream.Close();
            }
            DungeonEscapeSaveData saveData;
            try
            {
                saveData = JsonConvert.DeserializeObject<DungeonEscapeSaveData>(str);
            }
            catch (Exception ex)
            {
                Main.logger.LogInfo($"LoadDungeonEscape JsonConvert.DeserializeObject: {ex.Message}");
                return;
            }
            DungeonEscape.currentFloor = saveData.floor;
            ExtraGameMode.credits = saveData.money;
            Player.instance.playerStats.health = saveData.health;
            DungeonEscape.kills = saveData.kills;
            GameManager.currentGameData.playTime = saveData.playTime;
            saveData.playerInventorySaveData.Load();
            string equippedItemID = saveData.equippedItemID;
            ControlManager.LoadHotkeyItems(saveData.hotkeys);
            if (equippedItemID != null)
            {
                Item equippedItem = PlayerInventory.GetItem(equippedItemID);
                if (equippedItem)
                    PlayerEquipment.EquipItem(equippedItem);
            }
            Main.logger.LogInfo("LoadDungeonEscape floor " + saveData.floor);
        }

        public static void DeleteDungeonEscapeSaveFile()
        {
            if (File.Exists(dungeonEscapeSaveFilePath))
            {
                try
                {
                    File.Delete(dungeonEscapeSaveFilePath);
                }
                catch (Exception ex)
                {
                    Main.logger.LogInfo($"DeleteDungeonEscapeSaveFile: {ex.Message}");
                }
            }
        }

        public static void DeleteExtrasSaveFile()
        {
            if (File.Exists(extrasSaveFilePath))
            {
                try
                {
                    File.Delete(extrasSaveFilePath);
                    ExtrasManager.instance.InitExtraData();
                }
                catch (Exception ex)
                {
                    Main.logger.LogInfo($"DeleteDungeonExtrasSaveFile: {ex.Message}");
                }
            }
        }

        private static void UpdateFloorName(DungeonEscapeMinimap dungeonEscapeMinimap)
        {
            if (dungeonEscapeMinimap.floorText)
            {
                dungeonEscapeMinimap.floorText.text = LanguageManager.GetLocalization("DUNGEONESCAPE_MINIMAP_FLOOR").Replace("[FLOOR]", DungeonEscape.currentFloor.ToString());
                if (DungeonEscape.currentFloor <= 0)
                    dungeonEscapeMinimap.floorText.text = LanguageManager.GetLocalization("DUNGEONESCAPE_MINIMAP_FLOOR0");

                //updateFloorName = false;
            }
        }

        [HarmonyPatch(typeof(DungeonEscape))]
        class DungeonEscape_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("DungeonEscapeOnRoomTransition")]
            public static void DungeonEscapeOnRoomTransitionPrefix()
            {
                //DungeonEscapeUI.instance.ShowPopupMessage(" DungeonEscapeOnRoomTransition");
                DungeonEscapeFloorData floorData = DungeonEscape.GetFloorData(DungeonEscape.currentFloor);
                if (floorData)
                {
                    if (floorData.roomID == "dungeonescape_lastbossentrance")
                    {
                        DeleteDungeonEscapeSaveFile();
                        return;
                    }
                    //    string customExitDoor = " no customExitDoor";
                    //    if (floorData.genSettings.customExitDoor)
                    //        customExitDoor = " customExitDoor " + floorData.genSettings.customExitDoor.name;

                    //    Main.logger.LogInfo(" DungeonEscapeOnRoomTransition currentFloor " + DungeonEscape.currentFloor + " roomID " + floorData.roomID + customExitDoor + " maxRooms " + floorData.genSettings.maxRooms);
                    //    if (DungeonEscape.curRoom)
                    //    {
                    //        Main.logger.LogInfo(" DungeonEscapeOnRoomTransition currentFloor " + DungeonEscape.currentFloor + " curRoom.noMapGen " + DungeonEscape.curRoom.noMapGen);
                    //    }
                    //}
                    //if (dungeonEscapeFloorOverride > 0 && !dungeonEscapeFloorOverriden)
                    //{
                    //    DungeonEscape.currentFloor = dungeonEscapeFloorOverride;
                    //    dungeonEscapeFloorOverriden = true;
                }
                //if (dungeonEscapeFloorOverriden)
                //    return;

                if (DungeonEscape.currentFloor == 1)
                {
                    LoadDungeonEscape();
                }
                else if (DungeonEscape.currentFloor > 1)
                {
                    SaveDungeonEscape();
                }

            }

            //[HarmonyPostfix]
            //[HarmonyPatch("DungeonEscapeOnRoomTransition")]
            public static void DungeonEscapeOnRoomTransitionPostfix()
            {
                //UpdateFloorName();
                //updateFloorName = true;
            }
          
            [HarmonyPrefix]
            [HarmonyPatch("OnCharacterDie")]
            public static bool OnCharacterDiePrefix(DungeonEscape __instance, Character character)
            {
                if (Config.clearFloorCredits.Value == 400)
                    return true;

                if (!character || character.isPlayer)
                    return false;

                if (character.creditsWorth > 0)
                {
                    ExtraGameMode.credits += character.creditsWorth;
                    if (__instance.dungeonEscapeUI)
                        __instance.dungeonEscapeUI.SpawnCreditsIndicator(character.creditsWorth);
                }
                ++DungeonEscape.kills;
                if (DungeonEscape.floorCleared || DungeonEscape.currentFloor <= 0 || DungeonEscape.currentFloor >= 10)
                    return false;

                List<Character> characterList = ListTools.CleanedListNew<Character>(EntityManager.allCharacters);
                characterList.RemoveAll(obj => obj == null || !obj.status.isAlive || obj.isPlayer || obj.characterID == "MIMICCHEST");
                if (characterList.Count > 0)
                    return false;

                ExtraGameMode.credits += Config.clearFloorCredits.Value;
                if (__instance.dungeonEscapeUI)
                {
                    __instance.dungeonEscapeUI.SpawnCreditsIndicator(Config.clearFloorCredits.Value);
                    __instance.dungeonEscapeUI.ShowPopupMessage("DUNGEONESCAPE_FLOORCLEARED", fadeInSpeed: 0.25f);
                    __instance.floorClearedSound.Play();
                }
                DungeonEscape.floorCleared = true;
                return false;
            }

            //[HarmonyPostfix]
            //[HarmonyPatch("Reset")]
            public static void ResetPostfix(DungeonEscape __instance )
            {
                //dungeonEscapeFloorOverriden = false;
            }

        }

        //[HarmonyPatch(typeof(DungeonEscapeMinimap))]
        class DungeonEscapeMinimap_Patch
        {
            //[HarmonyPrefix]
            //[HarmonyPatch("Awake")]
            public static bool AwakePrefix(DungeonEscapeMinimap __instance)
            {
                //bool enabled = Config.dungeonEscapeMinimap.Value;
                //if (!enabled)
                //{
                //    updateFloorName = true;
                //    Transform mm = __instance.transform.Find("Map");
                //    if (mm)
                //        mm.gameObject.SetActive(false);

                //    mm = __instance.transform.Find("LargeModeInstructions");
                //    if (mm)
                //        mm.gameObject.SetActive(false);
                //}

                //return enabled;
                return false;
            }

            //[HarmonyPrefix]
            //[HarmonyPatch("Start")]
            public static bool StartPrefix(DungeonEscapeMinimap __instance)
            {
                //if (Config.dungeonEscapeFloorSizeMult.Value > 1 && Config.dungeonEscapeMinimap.Value)
                //{
                //    Transform t = __instance.transform.Find("Map/Background");
                //    if (t)
                //        t.gameObject.SetActive(false);

                //    t = __instance.transform.Find("Map/Border");
                //    if (t)
                //        t.gameObject.SetActive(false);

                //    t = __instance.transform.Find("LargeModeInstructions");
                //    if (t)
                //        t.gameObject.SetActive(false);
                //}
                //return Config.dungeonEscapeMinimap.Value;
                return false;
            }

            //[HarmonyPrefix]
            //[HarmonyPatch("Update")]
            public static bool UpdatePrefix(DungeonEscapeMinimap __instance)
            {
                //if (Config.dungeonEscapeMinimap.Value)
                //    return true;
                //else
                //{
                //    if (updateFloorName)
                //        UpdateFloorName(__instance);

                    return false;
                //}
            }

            //[HarmonyPrefix]
            //[HarmonyPatch("ApplyLargeMode")]
            public static bool ApplyLargeModePrefix(DungeonEscapeMinimap __instance)
            {
                //__instance.mapRect.gameObject.SetActive(false);
                return false;
            }
        }
    

        //[HarmonyPatch(typeof(DungeonEscapeMapNav))]
        class DungeonEscapeMapNav_Patch
        {
            //[HarmonyPrefix]
            //[HarmonyPatch("CreateTiles")]
            public static void AwakePrefix(DungeonEscapeMapNav __instance, ref int sizeX, ref int sizeY)
            {
                //Main.logger.LogInfo("DungeonEscapeMapNav CreateTiles sizeX " + sizeX + " sizeY " + sizeY);
                sizeX = (int)(sizeX * Config.dungeonEscapeFloorSizeMult.Value);
                sizeY = (int)(sizeY * Config.dungeonEscapeFloorSizeMult.Value);
                //Main.logger.LogInfo("DungeonEscapeMapNav CreateTiles new sizeX " + sizeX + " sizeY " + sizeY);
            }
        }

        [HarmonyPatch(typeof(Player))]
        class Player_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Die")]
            public static void DiePrefix(Player __instance)
            {
                if (ExtrasManager.activeGameMode == null || __instance.status.hasDied)
                    return;

                if (ExtrasManager.activeGameMode.ID == "DUNGEONESCAPE")
                {
                    DeleteDungeonEscapeSaveFile();
                    //dungeonEscapeFloorOverriden = false;
                    //Main.log.LogInfo(" Player Die DeleteDungeonEscapeSaveFile");
                }
            }
        }

        //[HarmonyPatch(typeof(DungeonEscape), "GenerateMap")]
        class RoomManager_GoToRoom_Patch
        {
            public static void Postfix(DungeonEscape __instance, DungeonEscapeMapGeneratorSettings settings)
            {
                Main.logger.LogInfo("GenerateMap currentFloor " + DungeonEscape.currentFloor);

            }
        }



    }
}
