using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 使用されるエフェクトの登録を行う
    /// ダメージやヒール等々
    /// </summary>
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager instance;
        [Header("VFX")]
        public GameObject bloodSplatterVFX;
        public GameObject healVFX;
        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;
        [SerializeField] List<InstantCharacterEffect> instantEffects;

        [Header("Heal")]
        public TakeHealEffect takeHealEffect;
        [Header("Energy")]
        public TakeEnergyEffect takeEnergyEffect;

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
            GenerateEffectIDs();
        }
        private void GenerateEffectIDs()
        {
            for (int i = 0; i < instantEffects.Count; i++)
            {
                instantEffects[i].instantEffectID = i;
            }
        }
    }
}
