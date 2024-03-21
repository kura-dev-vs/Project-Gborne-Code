using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using UnityEngine.TextCore.Text;

namespace RK
{
    /// <summary>
    /// characterのネットワーク変数関連
    /// </summary>
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;
        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [Header("Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.05f;
        public float networkRotationSmoothTime = 0.05f;

        [Header("Animator")]
        public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Target")]
        public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isToughBody = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Resources")]
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void CheckHP(int oldValue, int newValue)
        {
            if (currentHealth.Value <= 0)
            {
                if (!character.isDead.Value)
                    StartCoroutine(character.ProcessDeathEvent());
            }

            // 過剰な回復を阻止
            if (character.IsOwner)
            {
                if (currentHealth.Value > maxHealth.Value)
                {
                    currentHealth.Value = maxHealth.Value;
                }
            }
        }
        public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
        {
            if (!IsOwner)
            {
                character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
            }
        }
        public void OnIsLockedOnChanged(bool old, bool isLockedOn)
        {
            if (!isLockedOn)
            {
                character.characterCombatManager.currentTarget = null;
            }
        }
        public void OnIsChargingAttackChanged(bool oldStatus, bool newStatus)
        {
            character.animator.SetBool("isChargingAttack", isChargingAttack.Value);
        }

        public void OnIsMovingChanged(bool oldStatus, bool newStatus)
        {
            character.animator.SetBool("isMoving", isMoving.Value);
        }
        public virtual void OnIsActiveChanged(bool oldStatus, bool newStatus)
        {
            gameObject.SetActive(isActive.Value);
        }

        // ServerRpc: クライアントからサーバーに対して呼び出される関数
        // アクションアニメーション
        [ServerRpc]
        public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // サーバーの場合clientrpcを起動
            if (IsServer)
            {
                PlayActionAnimationForAllClientClientRpc(clientID, animationID, applyRootMotion);
            }

        }

        // ClientRpc: サーバーからすべてのクライアントに送信
        [ClientRpc]
        public void PlayActionAnimationForAllClientClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // clientIDが同一 (サーバーrpcを呼び出したクライアント自身) の場合はスルー (同じアクションを既に行っているため)
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerfomActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerfomActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }

        // 上記と同様
        // アタックアニメーション
        [ServerRpc]
        public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            if (IsServer)
            {
                PlayAttackActionAnimationForAllClientClientRpc(clientID, animationID, applyRootMotion);
            }

        }

        [ClientRpc]
        public void PlayAttackActionAnimationForAllClientClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerfomAttackActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerfomAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }

        // ダメージ
        [ServerRpc(RequireOwnership = false)]
        public void NotifyTheServerOfCharacterDamageServerRpc(
            ulong damageCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            if (IsServer)
            {
                NotifyTheServerOfCharacterDamageClientRpc(damageCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
            }

        }
        [ClientRpc]
        public void NotifyTheServerOfCharacterDamageClientRpc(
                    ulong damageCharacterID,
                    ulong characterCausingDamageID,
                    float physicalDamage,
                    float magicDamage,
                    float fireDamage,
                    float holyDamage,
                    float poiseDamage,
                    float angleHitFrom,
                    float contactPointX,
                    float contactPointY,
                    float contactPointZ)
        {
            ProcessCharacterDamageFromServer(damageCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }
        public void ProcessCharacterDamageFromServer(
                    ulong damageCharacterID,
                    ulong characterCausingDamageID,
                    float physicalDamage,
                    float magicDamage,
                    float fireDamage,
                    float holyDamage,
                    float poiseDamage,
                    float angleHitFrom,
                    float contactPointX,
                    float contactPointY,
                    float contactPointZ)
        {
            CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damageCharacterID].gameObject.GetComponent<CharacterManager>();
            CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.angleHitFrom = angleHitFrom;
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
            damageEffect.characterCausingDamage = characterCausingDamage;

            damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }

        // ヒール
        [ServerRpc(RequireOwnership = false)]
        public void NotifyTheServerOfCharacterHealServerRpc(
            ulong healCharacterID,
            ulong characterCausingHealID,
            float baseHeal)
        {
            if (IsServer)
            {
                NotifyTheServerOfCharacterHealClientRpc(healCharacterID, characterCausingHealID, baseHeal);
            }
        }
        [ClientRpc]
        public void NotifyTheServerOfCharacterHealClientRpc(
                    ulong healCharacterID,
                    ulong characterCausingHealID,
                    float baseHeal)
        {
            ProcessCharacterHealFromServer(healCharacterID, characterCausingHealID, baseHeal);
        }

        public void ProcessCharacterHealFromServer(
                    ulong healCharacterID,
                    ulong characterCausingHealID,
                    float baseHeal)
        {
            CharacterManager healedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[healCharacterID].gameObject.GetComponent<CharacterManager>();
            CharacterManager characterCausingHeal = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingHealID].gameObject.GetComponent<CharacterManager>();
            TakeHealEffect healEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealEffect);

            healEffect.baseHeal = baseHeal;
            healEffect.characterCausingHeal = characterCausingHeal;

            healedCharacter.characterEffectsManager.ProcessInstantEffect(healEffect);
        }
    }
}