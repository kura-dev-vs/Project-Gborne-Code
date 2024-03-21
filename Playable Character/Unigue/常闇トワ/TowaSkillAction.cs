using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 常闇トワの固有アクション
    /// </summary>
    public class TowaSkillAction : MonoBehaviour
    {
        public PlayerManager player;
        PCSkill rightSlill;
        public PCSupportSkill leftSkill;
        SkillHealCollider skillCollider;
        private void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            rightSlill = GetComponent<RightSkillManager>().skill;
            leftSkill = (PCSupportSkill)GetComponent<LeftSkillManager>().skill;
            SetSkillModifier(player, leftSkill);
        }
        private void SetSkillModifier(CharacterManager character, PCSupportSkill supportSkill)
        {
            var skillModel = supportSkill.skillModel;
            skillCollider = skillModel.GetComponent<SkillHealCollider>();
            skillCollider.characterCausingHeal = character;
            skillCollider.baseHeal = supportSkill.support_Modifier[0];
            skillCollider.durationTime = supportSkill.durationTime[0];
        }

        public void SupportSkillInstantiation()
        {
            Instantiate(leftSkill.skillModel, transform.parent);
        }
    }
}
