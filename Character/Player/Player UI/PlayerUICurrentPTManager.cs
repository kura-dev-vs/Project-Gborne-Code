using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// CurrentPTUIを管理するコンポーネント
    /// </summary>
    /// @Var characterSprites: 現在のPTを表示する。初期値をInspecterで設定し、動的に変更される。
    /// @Var characterNames: PTのキャラクターの名前。初期値をInspecterで設定し、動的に変更される。
    /// @Var currentPTID: 現在のPTID
    /// @Var ptSetUI: Inspecterで子のUI部分を設定する。
    public class PlayerUICurrentPTManager : MonoBehaviour
    {
        public Image[] characterSprites = new Image[4];
        public TextMeshProUGUI[] characterNames = new TextMeshProUGUI[4];
        int[] currentPTID = new int[4];
        public GameObject ptSetUI;
        public bool uiActivity = false;
        EntryManager entry;
        [SerializeField] GameObject first_Slot;

        public void OpenUI()
        {
            Time.timeScale = 0f;
            for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTID.Count; i++)
                currentPTID[i] = entry.playableCharacterEntryNetworkManager.currentPTID[i];
            RefreshCurrentPTUI(currentPTID);
            uiActivity = true;
            ptSetUI.SetActive(true);
            PlayerUIManager.instance.playerUIHudManager.gameObject.SetActive(false);
            EventSystemManager.instance.eventSystem.SetSelectedGameObject(first_Slot);
        }
        public void CloseUI()
        {
            Time.timeScale = 1f;
            PlayerUIManager.instance.playerUISelectableCharacterManager.CloseUI();
            uiActivity = false;
            ptSetUI.SetActive(false);
            PlayerUIManager.instance.playerUIHudManager.gameObject.SetActive(true);
        }
        public void UIActivity(EntryManager entryManager)
        {
            if (entry == null)
                entry = entryManager;

            if (uiActivity)
            {
                CloseUI();
            }
            else
            {
                OpenUI();
            }
        }
        /// <summary>
        /// UI更新時に表示するキャラクターの情報を変更する。
        /// </summary>
        /// <param name="index">PT内のindex</param>
        /// <param name="characterID">キャラクターのID</param>
        public void SetCharacterInfo(int index, int characterID)
        {
            PlayableCharacter pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(characterID);
            characterSprites[index].sprite = pc.fullBodyIcon;
            currentPTID[index] = characterID;
        }
        /// <summary>
        /// UIの更新時の挙動
        /// </summary>
        /// <param name="refreshPTID">更新後のPTID</param>
        public void RefreshCurrentPTUI(int[] refreshPTID)
        {
            int teamLength = refreshPTID.Length;
            for (int i = 0; i < teamLength; i++)
            {
                int pcID = refreshPTID[i];
                PlayerUIManager.instance.playerUICurrentPTManager.SetCharacterInfo(i, pcID);
            }
        }
        /// <summary>
        /// PTのボタン (画像) を押した時の挙動
        /// </summary>
        public void ClickPTSlots()
        {
            PlayerUIManager.instance.playerUISelectableCharacterManager.UIActivity(entry);
        }
        public void Deployed()
        {
            //entry.RefreshDeployedPT();
        }
        public void Back()
        {
            UIActivity(entry);
        }
        public void Restart()
        {
            WorldAIManager.instance.DespawnALLCharacters();
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                entry.player.respawnCharacter = true;
                WorldSaveGameManager.instance.CreateExtraScene();
            }
            else
            {
                WorldSaveGameManager.instance.LoadGame();
                entry.player.respawnCharacter = true;
            }
            entry.player.playerNetworkManager.currentScore.Value = 0;
        }
        public void ButtonsInteractable(bool isInteractable)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.interactable = isInteractable;
            }
            if (isInteractable)
            {
                EventSystemManager.instance.eventSystem.SetSelectedGameObject(first_Slot);
            }
        }
    }
}
