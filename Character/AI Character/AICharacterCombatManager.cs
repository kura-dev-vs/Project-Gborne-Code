using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// aicharacter用のcombatmanager
    /// ターゲットの索敵範囲や攻撃間隔等を用意
    /// </summary>
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;
        [Header("Action Recovery")]
        public float actionRecoveryTimer = 0;
        [Header("Pivot")]
        public bool enablePivot = true;
        [Header("Target Imformation")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;
        public float minimumFOV = -35;
        public float maximumFOV = 35;
        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25;
        protected override void Awake()
        {
            base.Awake();
            aiCharacter = GetComponent<AICharacterManager>();
        }
        /// <summary>
        /// targetの索敵. 先にspherecollliderで候補を見つけてから正面の敵をターゲットに入れる
        /// </summary>
        /// <param name="aiCharacter"></param>
        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;
            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();
                if (targetCharacter == null)
                    continue;
                if (targetCharacter == aiCharacter)
                    continue;
                if (targetCharacter.isDead.Value)
                    continue;
                // ターゲットとcharacterGroupを比較して別グループだった場合候補に入れる
                if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    //　候補の中から正面の敵をターゲットにする
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);
                    if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                    {
                        // linecastが障害物に邪魔されていないか
                        if (aiCharacter.characterCombatManager.lockOnTransform.position == null)
                        {
                            Debug.Log("CAN'T FIND LOCKON TRANSFORM");
                            return;
                        }
                        if (Physics.Linecast(
                            aiCharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position,
                            WorldUtilityManager.instance.GetEnviroLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                            Debug.Log("Blocked");
                        }
                        else
                        {
                            this.targetsDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, this.targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            if (enablePivot)
                                PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// ターゲットの方へ回頭するアニメーション
        /// </summary>
        /// <param name="aiCharacter"></param>
        public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return;
            if (viewableAngle >= 90 && viewableAngle <= 180)
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("zombie_turn_R", true);
            else if (viewableAngle <= -90 && viewableAngle >= -180)
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("zombie_turn_L", true);
        }
        /// <summary>
        /// 上記ほどじゃない場合にrotationで正面に捉え続ける
        /// </summary>
        /// <param name="aiCharacter"></param>
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
        /// <summary>
        /// アタックアニメーション中にターゲットへ回転させる目的. animatorのイベントでcanrotateをオンにすると有効になる
        /// </summary>
        /// <param name="aiCharacter"></param>
        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
                return;
            if (!aiCharacter.aiCharacterLocomotionManager.canRotate)
                return;
            if (!aiCharacter.isPerformingAction)
                return;
            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        }
        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}
