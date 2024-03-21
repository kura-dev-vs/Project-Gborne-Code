using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RK
{
    /// <summary>
    /// プレイアブルキャラクターのためのデータベース
    /// idから該当のキャラクターを取り出す
    /// </summary>
    public class WorldPlayableCharacterDatabase : MonoBehaviour
    {
        public static WorldPlayableCharacterDatabase instance;

        public PlayableCharacter NoCharacter;
        [Header("Weapons")]
        [SerializeField] List<PlayableCharacter> playableCharacters = new List<PlayableCharacter>();
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // すべてのキャラクターにidを付与する
            for (int i = 0; i < playableCharacters.Count; i++)
            {
                playableCharacters[i].playableCharacterID = i;
            }
        }
        public PlayableCharacter GetPlayableCharacterByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return playableCharacters.FirstOrDefault(playableCharacter => playableCharacter.playableCharacterID == ID);
        }
        public int GetPlayableCharacterCount()
        {
            int a = playableCharacters.Count;
            return a;
        }
    }
}