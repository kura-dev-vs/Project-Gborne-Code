using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RK
{
    /// <summary>
    /// playerそれぞれのUIを管理する大元のコンポーネント
    /// ここでプレイヤー自身のinstanceを指定し、各UIのManagerへアクセス
    /// </summary>
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;
        [HideInInspector] public PlayerUIHudManager playerUIHudManager;
        [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
        [HideInInspector] public PlayerUICurrentPTManager playerUICurrentPTManager;
        [HideInInspector] public PlayerUISelectablePCManager playerUISelectableCharacterManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
            playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
            playerUICurrentPTManager = GetComponentInChildren<PlayerUICurrentPTManager>();
            playerUISelectableCharacterManager = GetComponentInChildren<PlayerUISelectablePCManager>();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;
                // we must first shut down, because we have started as a host during the title screen
                NetworkManager.Singleton.Shutdown();
                // we then restart, as a client
                NetworkManager.Singleton.StartClient();
            }
        }
        public void OpenMenuUI(EntryManager entry)
        {
            playerUICurrentPTManager.UIActivity(entry);
        }
    }
}
