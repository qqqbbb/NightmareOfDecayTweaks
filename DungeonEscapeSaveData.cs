using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweaks
{
    internal class DungeonEscapeSaveData
    {
        public int floor;
        public int money;
        public int health;
        public int kills;
        public float playTime;
        public string equippedItemID;
        public List<string> hotkeys;
        public PlayerInventorySaveData playerInventorySaveData;

        public DungeonEscapeSaveData(int floor, int money, int health, int kills, float playTime, string equippedItemID, List<string> hotkeys, PlayerInventorySaveData playerInventorySaveData)
        {
            this.floor = floor;
            this.money = money;
            this.health = health;
            this.kills = kills;
            this.playTime = playTime;
            this.equippedItemID = equippedItemID;
            this.hotkeys = hotkeys;
            this.playerInventorySaveData = playerInventorySaveData;
        }

    }
}
