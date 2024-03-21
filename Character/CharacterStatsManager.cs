using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ステータス関連
    /// </summary>
    public class CharacterStatsManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;
        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        protected virtual void Start()
        {
        }
        /// <summary>
        /// 体力の計算
        /// </summary>
        /// <param name="vitality"></param>
        /// <returns></returns>
        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            health = vitality * 10;

            return Mathf.RoundToInt(health);
        }
        /// <summary>
        /// スタミナの計算
        /// </summary>
        /// <param name="endurance"></param>
        /// <returns></returns>
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }
        /// <summary>
        /// スタミナの回復
        /// </summary> 
        public virtual void RegenerateStamina()
        {
            if (character == null)
                return;
            if (!character.IsOwner)
                return;
            if (character.characterNetworkManager.isSprinting.Value)
                return;
            if (character.isPerformingAction)
                return;
            staminaRegenerationTimer += Time.deltaTime;
            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer = staminaTickTimer + Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        /// <summary>
        /// スタミナ消費時、回復を行うまでのタイマーをリセットする用
        /// </summary>
        /// <param name="previousStaminaAmount"></param>
        /// <param name="currentStaminaAmount"></param>
        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = staminaRegenerationDelay;
            }
        }
    }
}
