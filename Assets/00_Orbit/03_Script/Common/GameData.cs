using System.Collections.Generic;
using UnityEngine;
using System;

namespace STARTING
{
    [Serializable]
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

    [Serializable]
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

        public List<int> availableWeaponIndices;  // 사용할 수 있는 무기 인덱스들
        public List<int> equippedWeaponIndices;   // 현재 장착 중인 무기 인덱스들
    }
}