using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// バースト (必殺技) の管理に使用する
    /// 現在開発中
    /// </summary> <summary>
    /// 
    /// </summary>
    public class PlayerBurstManager : MonoBehaviour
    {
        PlayerManager player;
        EntryManager entry;
        PlayableCharacterManager playableCharacterManager;
        public PlayableCharacter playableCharacter;
        PCBurst burst;
        float recastTimeBurst;
        public float currentRecastBurst;
        public bool nowRecastingBurst = false;
        public float rechargeEnergy;
        public float currentEnergy;
        bool chargedBurst = false;
        private void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            entry = GetComponentInParent<EntryManager>();
            playableCharacterManager = GetComponent<PlayableCharacterManager>();
            playableCharacter = playableCharacterManager.playableCharacter;
            burst = playableCharacter.burst;
            recastTimeBurst = burst.recastTime;
            currentRecastBurst = recastTimeBurst;
            rechargeEnergy = burst.rechargeNeedEnergy;
            currentEnergy = rechargeEnergy / 2;
        }
        private void Update()
        {
            if (currentEnergy >= rechargeEnergy)
            {
                chargedBurst = true;
            }
            else
            {
                chargedBurst = false;
            }
        }
        private void FixedUpdate()
        {
            if (nowRecastingBurst)
            {
                currentRecastBurst -= 1f * Time.deltaTime;
            }
            if (currentRecastBurst <= 0)
            {
                nowRecastingBurst = false;
                currentRecastBurst = recastTimeBurst;
            }
        }
    }
}
