using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Data.Common;

namespace RK
{
    /// <summary>
    /// タイトルスクリーン上での挙動, ボタンを押したときのイベント管理
    /// </summary>
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;
        [Header("Menus")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button loadMenuReturnButton;
        [SerializeField] Button mainMenuLoadGameButton;
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button deleteCharacterPopUpConfirmButton;
        [Header("Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPop;
        [SerializeField] Button noCharacterSlotsOkeyButton;
        [SerializeField] GameObject deleteCharacterSlotPopUp;
        [Header("Character Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;
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
        }
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }
        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false);

            titleScreenLoadMenu.SetActive(true);
            // 最初にリセッﾄボタンを選択
            loadMenuReturnButton.Select();
        }
        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false);

            titleScreenMainMenu.SetActive(true);
            // 最初にロードゲームボタンを選択
            mainMenuLoadGameButton.Select();
        }
        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPop.SetActive(true);
            noCharacterSlotsOkeyButton.Select();
        }
        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPop.SetActive(false);
            mainMenuNewGameButton.Select();
        }
        // character slots
        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }
        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }
        public void AttempToDeleteCharacterSlot()
        {
            if (currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                deleteCharacterSlotPopUp.SetActive(true);
                deleteCharacterPopUpConfirmButton.Select();
            }
        }
        public void DeleteCharacterSlot()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
            // ロードメニューを無効にしてから有効にし、スロットをリフレッシュする（削除されたスロットは無効になる)
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            loadMenuReturnButton.Select();
        }
        public void CloseDeleteCharacterButton()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            loadMenuReturnButton.Select();
        }

        public void StartExtraGame()
        {
            WorldSaveGameManager.instance.CreateExtraScene();
        }
    }
}
