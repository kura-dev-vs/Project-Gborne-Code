using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Unity.Netcode;

namespace RK
{
    /// <summary>
    /// 戦闘に関する共通コンポーネント
    /// 基本は変数に書いてあるとおり
    /// </summary>
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;
        [Header("Last Attack Animation Performed")]
        public string lastAttackAnimationPerformed;
        [Header("Attack Target")]
        public CharacterManager currentTarget;
        [Header("Attack Type")]
        public AttackType currentAttackType;
        [Header("Lock On Transform")]
        public Transform lockOnTransform;
        [Header("Attack Flags")]
        public bool canPerformRollingAttack = false;
        public bool canPerformBackstepAttack = false;
        public bool isChase = false;
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        protected virtual void Start()
        {
            if (lockOnTransform == null)
                lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }
        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null;
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = 0;
                }
            }
        }

    }
}
