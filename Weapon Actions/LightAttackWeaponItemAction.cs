using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 近距離武器用弱攻撃
    /// </summary>
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [Header("Light Attacks")]
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";
        [SerializeField] string light_Attack_03 = "Main_Light_Attack_03";
        [SerializeField] string light_Attack_04 = "Main_Light_Attack_04";
        [Header("Running Attacks")]
        [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";
        [Header("Rolling Attacks")]
        [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";
        [Header("Backstep Attacks")]
        [SerializeField] string backstep_Attack_01 = "Main_Backstep_Attack_01";
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if (!playerPerformingAction.IsOwner)
                return;
            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;
            if (!playerPerformingAction.playerNetworkManager.isGrounded.Value)
                return;

            // sprint時の特殊モーション
            if (playerPerformingAction.characterNetworkManager.isSprinting.Value)
            {
                PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            // rolling時の特殊モーション
            if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
            {
                PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            // backstep時の特殊モーション
            if (playerPerformingAction.characterCombatManager.canPerformBackstepAttack)
            {
                PerformBackstepAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);
        }
        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // モーション中且つコンボが可能な状態
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                // 最後の攻撃モーションに応じたモーションを行う
                if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
                }
                else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_02)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack03, light_Attack_03, true);
                }
                else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_03)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack04, light_Attack_04, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
                }
            }
            // 非モーション中
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
            else
            {
                // playerPerformingAction.isPerformingAction == true && playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon == false
            }

        }
        private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RunningAttack01, run_Attack_01, true);
        }
        private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RollingAttack01, roll_Attack_01, true);
        }
        private void PerformBackstepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canPerformBackstepAttack = false;
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.BackstepAttack01, backstep_Attack_01, true);
        }
    }
}
