using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweaks
{
    public static class Config
    {
        public static ConfigEntry<bool> unlockExtras;
        public static ConfigEntry<float> playerSpeedMult;
        public static AcceptableValueRange<float> playerSpeedMultRange = new(1f, 3f);
        public static ConfigEntry<int> playerDamageMult;
        public static ConfigEntry<float> playerTakenDamageMult;
        public static ConfigEntry<float> dungeonEscapeFloorSizeMult;
        public static AcceptableValueRange<float> dungeonEscapeFloorSizeMultRange = new(0.5f, 1.5f);
        public static ConfigEntry<int> dungeonEscapeFloors;
        public static AcceptableValueRange<int> playerDamageMultRange = new(1, 10);
        public static AcceptableValueRange<float> playerTakenDamageMultRange = new(0f, 2f);
        public static AcceptableValueRange<int> dungeonEscapeFloorsRange = new(1, 100);
        //public static ConfigEntry<bool> dungeonEscapeMinimap;
        public static ConfigEntry<int> clearFloorCredits;
        public static AcceptableValueRange<int> clearFloorCreditsRange = new(0, 1000);

        public static void Bind()
        {
            playerSpeedMult = Main.config.Bind("", "Player movement speed multiplier", 1f, new ConfigDescription("", playerSpeedMultRange));
            playerDamageMult = Main.config.Bind("", "Player dealt damage multiplier", 1, new ConfigDescription("", playerDamageMultRange));
            playerTakenDamageMult = Main.config.Bind("", "Player taken damage multiplier", 1f, new ConfigDescription("", playerTakenDamageMultRange));
            //dungeonEscapeFloors = Main.config.Bind("", "Number of floors in Dungeon Escape", 10, new ConfigDescription("Change this only from main menu", dungeonEscapeFloorsRange));
            unlockExtras = Main.config.Bind("", "Unlock extra game modes and weapons", false);

            clearFloorCredits = Main.config.Bind("", "Dungeon Escape clear floor credits", 400, new ConfigDescription("", clearFloorCreditsRange));
            //dungeonEscapeFloorSizeMult = Main.config.Bind("Dungeon Escape", "Dungeon escape floor size multiplier", 1f, new ConfigDescription("", dungeonEscapeFloorSizeMultRange));
            unlockExtras.SettingChanged += OnUnlockExtrasChanged;
        }

        public static void OnUnlockExtrasChanged(object sender, EventArgs e)
        {
            if (unlockExtras.Value)
            {
                Unlocker.UnlockExtras();
            }
            else
            {
                //DungeonEscapePatcher.DeleteDungeonExtrasSaveFile();
            }
        }



    }
}
