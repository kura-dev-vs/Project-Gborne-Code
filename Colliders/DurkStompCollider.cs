using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// スタンプ攻撃専用
    /// </summary>
    public class DurkStompCollider : DamageCollider
    {
        [SerializeField] AIDurkCharacterManager durkCharacterManager;
        protected override void Awake()
        {
            base.Awake();

            durkCharacterManager = GetComponentInParent<AIDurkCharacterManager>();
        }
        public void StompAttack()
        {
            GameObject stompVFX = Instantiate(durkCharacterManager.durkCombatManager.durkImpactVFX, transform);
            Collider[] colliders = Physics.OverlapSphere(transform.position, durkCharacterManager.durkCombatManager.stompAttackAOERadius, WorldUtilityManager.instance.GetCharacterLayers());
            List<CharacterManager> charactersDamaged = new List<CharacterManager>();

            foreach (var collider in colliders)
            {
                CharacterManager character = collider.GetComponentInParent<CharacterManager>();

                if (character != null)
                {
                    if (charactersDamaged.Contains(character))
                        continue;
                    if (character == durkCharacterManager)
                        continue;
                    charactersDamaged.Add(character);

                    if (character.IsOwner)
                    {
                        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                        damageEffect.physicalDamage = durkCharacterManager.durkCombatManager.stompDamage;
                        damageEffect.poiseDamage = durkCharacterManager.durkCombatManager.stompDamage;

                        character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                    }
                }
            }
        }
    }
}
