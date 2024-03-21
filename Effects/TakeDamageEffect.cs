using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ダメージを与えるエフェクト
    /// </summary>
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // ダメージの発生元 ("受けた"ではなく"与えた"キャラクター)
        [Header("Damage")]
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;
        [Header("Final Damage")]
        private int finalDamageDealt = 0;   // 最終的に与えるダメージ
        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;    // SAを崩すための蓄積ダメージ

        // 追加効果はここで受け取るための変数を用意

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = true;
        public string damageAnimation;
        [Header("Sound FX")]
        public bool willDamageSFX = true;
        public AudioClip elementalDamageSoundSFX;   // 特別な属性の攻撃 (fireとか) が当たったときのsfxを弄りたい場合は指定
        [Header("Direction Damage Taken From")]
        public float angleHitFrom;  // どの角度から攻撃が当たったか
        public Vector3 contactPoint;    // 攻撃が当たった場所
        Transform _markerPanel;
        [SerializeField] private HealthChangeTextController healthChangeText;
        private void Awake()
        {
            _markerPanel = PlayerUIManager.instance.playerUIHudManager.damageTextParent;
        }
        public override void ProcessEffect(CharacterManager character)
        {
            if (character.characterNetworkManager.isInvulnerable.Value)
                return;

            base.ProcessEffect(character);

            if (character.isDead.Value)
                return;

            CalculateDamage(character);
            PlayDirectionalBasedDamageAnimation(character);

            // 追加効果がある場合はここで計算

            PlayDamageSFX(character);
            PlayDamageVFX(character);
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (characterCausingDamage != null)
            {
                // ダメージ修正をチェック
            }
            // 最終的なダメージを計算する前にダメージの計算方式を考える
            // 防御力, アーマー, バフ, デバフなど
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }
            if (character.IsOwner)
            {

                character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

                // poise蓄積を計算
            }

            // 最終的なダメージをUIに表示
            var marker = Instantiate(healthChangeText, _markerPanel);
            marker.Initialize(contactPoint, finalDamageDealt.ToString());

            if (character.characterGroup != PlayerCamera.instance.player.characterGroup)
            {
                // ダメージを負ったのが視点側のプレイヤーでない場合のみダメージ表示したいとき
            }
        }
        /// <summary>
        /// ダメージが発生したとき用のvfx
        /// </summary>
        /// <param name="character"></param>
        private void PlayDamageVFX(CharacterManager character)
        {
            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);

        }
        /// <summary>
        /// ダメージが発生したとき用のsfx
        /// </summary>
        /// <param name="character"></param>
        private void PlayDamageSFX(CharacterManager character)
        {
            AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
            character.characterSoundFXManager.PlayDamageGruntSoundFX();

        }
        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner)
                return;
            if (character.isDead.Value)
                return;
            if (character.characterNetworkManager.isToughBody.Value)
                return;

            // SAが崩れたことを示す
            poiseIsBroken = true;

            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }

            // SAが崩れた場合ダメージアニメーションを再生
            if (poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }

    }
}
