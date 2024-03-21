using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// PlayableCharacterに共通で必要な情報を記述する
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacter")]
    public class PlayableCharacter : ScriptableObject
    {
        [Header("Playable Character Information")]
        public string characterName;
        public Sprite faceIcon, fullBodyIcon;
        public int playableCharacterID;

        [Header("Character Model")]
        public GameObject characterModel;
        [Header("Character Skill")]
        public PCSkill leftSkill;
        public PCSkill rightSkill;
        [Header("Character Burst")]
        public PCBurst burst;
        [Header("Character Stats")]
        public int characterLv = 1;
        public int basicOffense, basicDefense;
    }
}