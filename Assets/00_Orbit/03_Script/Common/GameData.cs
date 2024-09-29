using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    [System.Serializable]
    public class ZoneData
    {
        public string zoneName;
        public bool isLiberated;

        public ZoneData()
        {
        }

        public ZoneData(string zoneName, bool isLiberated)
        {
            this.zoneName = zoneName;
            this.isLiberated = isLiberated;
        }
    }

    [System.Serializable]
    public class GameData
    {
        public float gameTime;

        public int maxHealth;
        public int maxMana;
        public int maxExperience;

        public int currentHealth;
        public int currentMana;
        public int currentExperience;

        public int level;
        public Vector3 playerPosition;
        public List<ZoneData> zones;
        public int chip;
    }
}