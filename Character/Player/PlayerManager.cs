using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEditor.ShaderGraph;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace RK
{
    /// <summary>
    /// プレイヤー操作キャラクターの大元マネージャー
    /// 基本ここから各種マネージャーアクセスする
    /// NetworkBehaviour継承
    /// </summary>
    public class PlayerManager : CharacterManager
    {
        [Header("DEBUG MENU")]
        [HideInInspector] public EntryManager entry;
        public bool respawnCharacter = false;
        [SerializeField] bool switchRightWeapon = false;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public RightSkillManager rightSkillManager;
        [HideInInspector] public LeftSkillManager leftSkillManager;
        [HideInInspector] public PlayerBurstManager playerBurstManager;
        public PlayerCombatManager playerCombatManager;
        protected override void Awake()
        {
            base.Awake();
            entry = GetComponent<EntryManager>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            rightSkillManager = GetComponent<RightSkillManager>();
            leftSkillManager = GetComponent<LeftSkillManager>();
            playerBurstManager = GetComponent<PlayerBurstManager>();
        }
        protected override void Update()
        {
            base.Update();
            if (!IsOwner)
                return;

            // isowner (ローカルプレイヤーのオブジェクト) でのみ実行

            // handle movement
            playerLocomotionManager.HandleAllMovement();

            // スタミナの回復
            playerStatsManager.RegenerateStamina();

            // スキル、爆発関連のUIHud表示を実行している (UI管理だけなのでリキャストは別のマネージャーで行っている)
            SetRecastSkillBurst();
            SetEnergyValue();

            // 機能の試運転デバッグ用
            DebugMenu();
        }
        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;

            base.LateUpdate();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // "クライアントが接続すると呼び出されるコールバック。このコールバックはサーバと接続したローカルクライアントでのみ実行される。"
            // クライアントがサーバーと接続した瞬間に発生するコールバックに対し、"OnClientConnectedCallback"を実行するようスポーン時に設定しておく
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            // isowner (ローカルプレイヤーのオブジェクト) でのみ実行
            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                // ネットワーク変数の対応
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                // スタミナや体力の変化時にローカルプレイヤーuiも変化させる
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                //playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
            // Lock On
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;
            // Equipment
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            // Flags
            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;

            // 非サーバーのクライアントとして接続時にキャラクターデータをロードする
            // サーバーのホストはセーブデータ選択時にロード済であるため必要無い
            if (IsOwner && !IsServer)
            {
                LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }

            playerNetworkManager.currentScore.OnValueChanged += playerNetworkManager.OnCurrentScoreChange;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            // isowner (ローカルプレイヤーのオブジェクト) でのみ実行
            if (IsOwner)
            {
                playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                //playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
            }

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
            // Lock On
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;
            // Equipment
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            // Flags
            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;

            playerNetworkManager.currentScore.OnValueChanged -= playerNetworkManager.OnCurrentScoreChange;
        }

        /// <summary>
        /// クライアントがサーバーにコネクトした際に呼び出される
        /// ゲームセッションにプレイヤーを追加し、他のプレイヤーの情報を読み込む
        /// </summary>
        /// <param name="clientID"></param>
        private void OnClientConnectedCallback(ulong clientID)
        {
            WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

            // 他のキャラクターの情報をロードする
            // 非サーバーかつローカルプレイヤーの場合のみ実行
            // 同期のためにゲームセッションへの参加時に必要
            if (!IsServer && IsOwner)
            {
                foreach (var player in WorldGameSessionManager.instance.players)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDamageAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }
            return base.ProcessDeathEvent(manuallySelectDamageAnimation);

            // 回生を設定するならこのあたりに実行するか？
        }

        /// <summary>
        /// 復活時のメソッド。現在はなんの演出もなく復活するだけ
        /// </summary>
        public override void ReviveCharacter()
        {
            base.ReviveCharacter();
            if (IsOwner)
            {
                isDead.Value = false;
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

                playerAnimatorManager.PlayTargetActionAnimation("Empty", false, true, true, true);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;

            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }


        public void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            // 武器の同期
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

            if (playerNetworkManager.isLockedOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
            }
        }
        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }
        }
        /// <summary>
        /// アクティブキャラクター変更時に親オブジェクトのcharactercontrollerのコライダー (厳密にはcolliderではなくcharactercontroller固有の当たり判定) をキャラクター固有のものに変更
        /// </summary>
        /// <param name="playableCharacter"></param>
        public void SetController(GameObject playableCharacter)
        {
            characterController.height = playableCharacter.GetComponent<CharacterController>().height;
        }

        /// <summary>
        /// 爆発とスキルのUI関連。後々別のコンポーネントに移したほうがいいかも
        /// </summary>
        public void SetRecastSkillBurst()
        {
            if (playerBurstManager.nowRecastingBurst)
            {
                PlayerUIManager.instance.playerUIHudManager.SetEnabledBurstRecast(playerBurstManager.currentRecastBurst);
            }
            else
            {
                PlayerUIManager.instance.playerUIHudManager.SetDisableBurstRecast();
            }
            if (rightSkillManager.nowRecasting)
            {
                PlayerUIManager.instance.playerUIHudManager.SetEnabledRightRecast(rightSkillManager.currentRecast);
            }
            else
            {
                PlayerUIManager.instance.playerUIHudManager.SetDisableRightRecast();
            }

            if (leftSkillManager.nowRecasting)
            {
                PlayerUIManager.instance.playerUIHudManager.SetEnabledLeftRecast(leftSkillManager.currentRecast);
            }
            else
            {
                PlayerUIManager.instance.playerUIHudManager.SetDisableLeftRecast();
            }
        }
        private void SetEnergyValue()
        {
            float value = playerBurstManager.currentEnergy / playerBurstManager.rechargeEnergy;
            PlayerUIManager.instance.playerUIHudManager.SetEnergyValue(value);
        }
    }
}
