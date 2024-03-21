using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// プレイヤーの武器アクション関連
    /// </summary>
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;
        public WeaponItem currentWeaponBeingUsed;
        [Header("Flags")]
        public bool canComboWithMainHandWeapon = false;
        // public bool canComboWithOffHandWeapon=false;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }
        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            /*
            if (player.isPerformingAction)
                return;
                */
            if (weaponAction == null)
            {
                Debug.Log("DON'T SET WEAPON ACTION");
                return;
            }
            // アクションを実行
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

            // サーバーにアクションを実行したことを通知
            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
        public virtual void DrainStaminaBasedOnAttack()
        {
            if (!player.IsOwner)
                return;
            if (currentWeaponBeingUsed == null)
                return;

            float staminaDeducted = 0;
            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack03:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack04:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargedAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargedAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                case AttackType.RunningAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttackStaminaCostMultiplier;
                    break;
                case AttackType.RollingAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingAttackStaminaCostMultiplier;
                    break;
                case AttackType.BackstepAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backstepAttackStaminaCostMultiplier;
                    break;
                default:
                    break;
            }
            Debug.Log("Stamina deducted" + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

    }
}
