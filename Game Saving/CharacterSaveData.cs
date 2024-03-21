using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{

    [System.Serializable]
    /// <summary>
    /// 保存ファイルごとにこのデータを参照したいので、このスクリプトはMonoBehaviorではなく, serializable
    /// 
    /// </summary>
    public class CharacterSaveData
    {
        [Header("SCENE INDEX")]
        public int sceneIndex = 1;
        [Header("Character Name")]
        public string characterName = "Character";
        [Header("Time Played")]
        public float secondsPlayed;

        // monobehaiviorではないのでvector3が使えない
        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")]
        public int currentHealth;
        public float currentStamina;
        [Header("Stats")]
        public int vitality, endurance;
        [Header("Bosses")]
        public SerializableDictionary<int, bool> bossesAwakened;    // 対応するidを持つボスがawakenedされているか
        public SerializableDictionary<int, bool> bossesDefeated;    // 対応するidを持つボスがdefeatedされているか
        public CharacterSaveData()
        {
            bossesAwakened = new SerializableDictionary<int, bool>();
            bossesDefeated = new SerializableDictionary<int, bool>();
        }
    }
}
