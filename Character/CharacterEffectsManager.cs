using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace RK
{
    /// <summary>
    /// キャラクターに影響するエフェクトの処理を行うコンポーネント
    /// ・ダメージやヒールなど実数値に即座に影響が出るもの
    /// ・時限式の効果 (毒とか)
    /// ・バフ・デバフ関連
    /// </summary>
    public class CharacterEffectsManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;
        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;
        [SerializeField] GameObject healVFX;
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
        /// <summary>
        /// 被ダメージのVFX
        /// 専用のものを追加する場合はbloodSplatterVFXに設定しておき、なかったら共通のものが実行される
        /// </summary>
        /// <param name="contactPoint"></param>
        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

        /// <summary>
        /// 被ヒールのVFX
        /// 専用のものを追加する場合はhealVFXに設定しておき、なかったら共通のものが実行される
        /// </summary>
        /// <param name="contactPoint"></param>
        public void PlayHealVFX()
        {
            if (bloodSplatterVFX != null)
            {
                GameObject heal = Instantiate(healVFX);
            }
            else
            {
                GameObject heal = Instantiate(WorldCharacterEffectsManager.instance.healVFX);
            }
        }

    }
}
