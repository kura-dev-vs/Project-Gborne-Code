using System.Collections;
using System.Collections.Generic;
using Mebiustos.MMD4MecanimFaciem;
using UnityEngine;

namespace RK
{
    public class WeaponHiddenFlag : StateMachineBehaviour
    {
        PlayerManager player;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (player == null)
            {
                // PlayerManagerが親、Animatorが子オブジェクトにあるためInParent
                player = animator.GetComponentInParent<PlayerManager>();
            }
            PlayerEquipmentManager equip = player.playerEquipmentManager;
            equip.rightHandWeaponModel.SetActive(false);
            equip.leftHandWeaponModel.SetActive(false);

            FaciemController faciem = animator.GetComponent<FaciemController>();
            faciem.SetFace("ウインク右");

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (player == null)
            {
                // PlayerManagerが親、Animatorが子オブジェクトにあるためInParent
                player = animator.GetComponentInParent<PlayerManager>();
            }
            PlayerEquipmentManager equip = player.playerEquipmentManager;
            equip.rightHandWeaponModel.SetActive(true);
            equip.leftHandWeaponModel.SetActive(true);
            FaciemController faciem = animator.GetComponent<FaciemController>();
            faciem.SetFace("Default");
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
