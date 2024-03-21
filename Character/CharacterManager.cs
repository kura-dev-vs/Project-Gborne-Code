using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// キャラクターオブジェクトに必須
    /// 
    /// </summary>
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        /*[HideInInspector]*/
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [Header("Character Group")]
        public CharacterGroup characterGroup;
        public LockOnTransform myLockOnTransform;
        public float HitStopTime = 0.05f;

        [Header("Flags")]
        // キャラクターが新しいアクションを試みるのを止めるために使用する
        // 例えば、ダメージを受け、ダメージアニメーションを開始した場合
        // 気絶した場合、このフラグがtrueになる
        // 新しいアクションを試みる前に、このフラグをチェックすることができる
        public bool isPerformingAction = false;
        public bool canDodge = true;
        public bool canAttack = true;
        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            myLockOnTransform = GetComponentInChildren<LockOnTransform>();
        }
        protected virtual void Start()
        {
            IgnoreMyOwnColliders();
        }
        protected virtual void Update()
        {
            if (animator == null)
                return;
            animator.SetBool("isGrounded", characterNetworkManager.isGrounded.Value);
            // ローカルプレイヤーオブジェクトの場合こちらのposition,rotationをネットワーク変数に更新
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            // それ以外のクライアントからは更新されたネットワーク変数から位置を調整
            else
            {
                // position
                transform.position = Vector3.SmoothDamp
                (transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime);

                // rotation
                transform.rotation = Quaternion.Slerp
                (transform.rotation,
                characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime);
            }
            canAttack = canDodge;
        }
        protected virtual void FixedUpdate()
        {

        }
        protected virtual void LateUpdate()
        {

        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (animator != null)
            {
                animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);
            }
            characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDamageAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // 死亡時になんらかのフラグをリセットしたいときはここに書く

                if (!manuallySelectDamageAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }
            // sfxやvfxなどここで書く
            yield return new WaitForSeconds(5);
        }
        public virtual void ReviveCharacter()
        {

        }

        /// <summary>
        /// キャラクターの持つ複数のコライダー同士の衝突判定を無効化する
        /// </summary>
        protected virtual void IgnoreMyOwnColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
            List<Collider> ignoreColliders = new List<Collider>();

            // モデルの関節ジョイントの位置依存のコライダーをlistでまとめる
            foreach (var collider in damageableCharacterColliders)
            {
                ignoreColliders.Add(collider);
            }

            // character colliderのコライダーもlistに追加
            ignoreColliders.Add(characterControllerCollider);

            // list内のすべてのcollider同士の衝突を無効化
            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }
        /// <summary>
        /// 攻撃ヒット時のヒットストップ
        /// Dotweenを使用しているが将来的には別の方法で実装したい
        /// </summary>
        public void OnAttackHitStop()
        {
            // モーションを止める
            animator.speed = 0f;

            var seq = DOTween.Sequence();
            seq.SetDelay(HitStopTime);
            // モーションを再開
            seq.AppendCallback(() => animator.speed = 1f);

        }
    }
}
