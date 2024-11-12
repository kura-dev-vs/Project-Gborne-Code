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
        public int pcID;
        public GameObject pcUI3D;

        [Header("Character Model")]
        public GameObject characterModel;
        [Header("Animations")]
        public AnimatorOverrideController pcAnimator;
        [Header("Character Skill")]
        public PCSkill leftSkill;
        public PCSkill rightSkill;
        [Header("Character Burst")]
        public PCBurst burst;
        [Header("Character Stats")]
        public int characterLv = 1;
        public int basicOffense = 0;
        public int basicDefense = 0;
        public float basicPhysicalAbs = 0;
        public float basicMagicAbs = 0;
        public float basicFireAbs = 0;
        public float basicLightningAbs = 0;
        public float basicHolyAbs = 0;

        [Header("Equipment")]
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;
        public HatOutfitItem hatOutfit;
        public JacketOutfitItem jacketOutfit;
        public TopsOutfitItem topsOutfit;
        public BottomsOutfitItem bottomsOutfit;
        public ShoesOutfitItem shoesOutfit;

        [HideInInspector] public float afterPhysicalAbs = 0;
        [HideInInspector] public float afterMagicAbs = 0;
        [HideInInspector] public float afterFireAbs = 0;
        [HideInInspector] public float afterLightningAbs = 0;
        [HideInInspector] public float afterHolyAbs = 0;

        [Header("Critical Damage")]
        public int riposte_Attack_01_Modifier = 1;
        public int backstab_Attack_01_Modifier = 1;
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;
        public float poiseDamage = 0;
        public float stanceDamage = 0;

        public void CalculateCharacterStats()
        {
            afterPhysicalAbs = basicPhysicalAbs;
            afterMagicAbs = basicMagicAbs;
            afterFireAbs = basicFireAbs;
            afterLightningAbs = basicLightningAbs;
            afterHolyAbs = basicHolyAbs;

            if (hatOutfit != null)
                CalculateOutfitAbs(hatOutfit);

            if (jacketOutfit != null)
                CalculateOutfitAbs(jacketOutfit);

            if (topsOutfit != null)
                CalculateOutfitAbs(topsOutfit);

            if (bottomsOutfit != null)
                CalculateOutfitAbs(bottomsOutfit);

            if (shoesOutfit != null)
                CalculateOutfitAbs(shoesOutfit);
        }
        private void CalculateOutfitAbs(OutfitItem outfit)
        {
            afterPhysicalAbs += outfit.physicalDamageAbsorption;
            afterMagicAbs += outfit.magicDamageAbsorption;
            afterFireAbs += outfit.fireDamageAbsorption;
            afterLightningAbs += outfit.lightningDamageAbsorption;
            afterHolyAbs += outfit.holyDamageAbsorption;
        }

    }
}