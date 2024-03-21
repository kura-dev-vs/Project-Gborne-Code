using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 天音かなたの固有アクション
    /// </summary>
    public class KanataSkillAction : UniqueSkillActionManager
    {
        public PlayerManager player;
        PCSkill rightSlill;
        public PCRangedSkill leftSkill;
        SkillRangedCollider skillCollider;
        private void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            rightSlill = GetComponent<RightSkillManager>().skill;
            leftSkill = (PCRangedSkill)GetComponent<LeftSkillManager>().skill;
            SetSkillModifier(player, leftSkill);
        }
        private void SetSkillModifier(PlayerManager player, PCRangedSkill rangedSkill)
        {
            var skillModel = rangedSkill.skillModel;
            skillCollider = skillModel.GetComponent<SkillRangedCollider>();
            skillCollider.characterCausingDamage = player.entry;
            //skillCollider.baseDamage = rangedSkill.base_Damage[0];
            skillCollider.skillModifier = rangedSkill.base_Damage[0];
            SetDamages(skillCollider, rangedSkill);
        }

        public void RangedSkillInstantiation()
        {
            Transform leftWeaponTransform = player.playerEquipmentManager.leftHandWeaponModel.transform;
            Vector3 bulletDirection;
            GameObject bullet = Instantiate(leftSkill.skillModel, leftWeaponTransform.position, player.transform.rotation);
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                Vector3 targetPosition = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position;
                bulletDirection = targetPosition - leftWeaponTransform.position;
                bullet.GetComponent<HomingController>().SetBulletDirection(bulletDirection);
                bullet.GetComponent<HomingController>().SetTarget(player.playerCombatManager.currentTarget);
            }
            else
            {
                bulletDirection = player.transform.forward;
                bullet.GetComponent<HomingController>().SetBulletDirection(bulletDirection);
            }


            SkillRangedCollider bulletCollider = bullet.GetComponent<SkillRangedCollider>();
            SetDamages(bulletCollider, leftSkill);
        }
    }
}
