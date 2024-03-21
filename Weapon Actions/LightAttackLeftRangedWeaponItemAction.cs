using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 遠距離武器用弱攻撃
    /// 遠距離武器に強攻撃を作成するかは未定
    /// 遠距離武器でコンボを行うかも未定なので
    /// </summary>
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Left Ranged Weapon Action")]
    public class LightAttackLeftRangedWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string light_Attack_01 = "Left_Ranged_Light_Attack_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if (!playerPerformingAction.IsOwner)
                return;
            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;
            if (!playerPerformingAction.playerNetworkManager.isGrounded.Value)
                return;

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);
        }
        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // モーション中且つコンボが可能な状態
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                // 最後の攻撃モーションに応じたモーションを行う
                // 遠距離武器でコンボを行うかは未定なのでどちらにせよ同じモーションを行う
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true, true, false, false);

            }
            // 非モーション中
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true, true, false, false);
            }
            else
            {
                // playerPerformingAction.isPerformingAction == true && playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon == false

            }
            // ロックオン中はターゲットに向き直る
            if (playerPerformingAction.playerNetworkManager.isLockedOn.Value)
            {
                playerPerformingAction.playerLocomotionManager.RotationToTarget();
            }
        }
    }
}
