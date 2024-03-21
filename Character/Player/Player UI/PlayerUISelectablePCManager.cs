using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;


namespace RK
{
    /*
    SelectablePCUIに関するコンポ―ネント
    @param changedPTID: 変更後のPT (仮) に加入するPCIDの配列
    @param currentPTID: 現在のPTに加入するPCIDの配列
    @param characterIcon: PC一覧表示に生成させるアイコン
    @param pCLists: characterIcon生成時の親
    @param savePT: 変更後のPTを確定させる場合ボタン
    @param tmpIcon
    @param selectablePC_UI: SelectablePC_UI
    @param uiActivity: SelectablePC_UIのアクティビティ

    */
    public class PlayerUISelectablePCManager : MonoBehaviour
    {
        [HideInInspector] public int[] changedPTID = new int[4];
        [HideInInspector] public int[] currentPTID = new int[4];
        [SerializeField] GameObject characterIcon;
        [SerializeField] GameObject pCLists;
        public Button savePT;
        GameObject tmpIcon;
        [SerializeField] GameObject selectablePC_UI;
        bool uiActivity = false;
        EntryManager entry;
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
        public void OpenUI()
        {
            SetPCIcons();
            uiActivity = true;
            selectablePC_UI.SetActive(true);
            PlayerUIManager.instance.playerUICurrentPTManager.ButtonsInteractable(false);
        }
        public void CloseUI()
        {
            DestroyChildAll(pCLists.transform);
            uiActivity = false;
            selectablePC_UI.SetActive(false);
            PlayerUIManager.instance.playerUICurrentPTManager.ButtonsInteractable(true);
        }

        /// <summary>
        /// PCIconの一覧を表示させる
        /// 現PTに加入時はシンボルを付与する
        /// </summary>
        private void SetPCIcons()
        {
            if (pCLists.transform == null)
                return;
            if (characterIcon == null)
                return;

            for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTID.Count; i++)
                currentPTID[i] = entry.playableCharacterEntryNetworkManager.currentPTID[i];
            Array.Copy(currentPTID, changedPTID, 4);

            ComparisonPT();
            for (int i = 1; i < WorldPlayableCharacterDatabase.instance.GetPlayableCharacterCount(); i++)
            {
                tmpIcon = Instantiate(characterIcon, pCLists.transform, false);
                TransformOfFaceIcon icon = tmpIcon.GetComponentInChildren<TransformOfFaceIcon>();
                icon.pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i);
                icon.faceIcon.sprite = icon.pc.faceIcon;

                int ptIndex = CheckPTJoined(icon.pc.playableCharacterID);
                icon.numberInPT = ptIndex;
                if (ptIndex != -1)
                {
                    icon.alreadyJoinedSymbol.SetActive(true);
                    icon.SetNumberInPT(icon.numberInPT);
                }
                if (i == 1)
                {
                    EventSystemManager.instance.eventSystem.SetSelectedGameObject(tmpIcon);
                }
            }
        }
        private void DestroyChildAll(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
        public int CheckPTJoined(int pcID)
        {
            int result = Array.IndexOf(changedPTID, pcID);
            return result;
        }
        public void ChangePTCharacter(int index, int pcID)
        {
            changedPTID[index] = pcID;
        }
        public void Back()
        {
            UIActivity(entry);
        }

        /// <summary>
        /// SavePTボタンを押した場合の挙動
        /// </summary>
        public void ClickSavePT()
        {
            if (currentPTID.SequenceEqual(changedPTID))
                return;
            if (entry.player.isDead.Value)
                return;
            bool result = changedPTID.All(x => x == WorldPlayableCharacterDatabase.instance.NoCharacter.playableCharacterID);
            if (result)
            {
                Debug.Log("キャラクターを最低一人以上は選択してください。");
                return;
            }
            Array.Copy(changedPTID, currentPTID, 4);
            for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTID.Count; i++)
            {
                entry.playableCharacterEntryNetworkManager.currentPTID[i] = currentPTID[i];
                entry.playableCharacterInventoryManager.currentPCPT[i] = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(entry.playableCharacterEntryNetworkManager.currentPTID[i]);
            }

            DestroyChildAll(PlayerUIManager.instance.playerUIHudManager.characterSlotParent);
            PlayerUIManager.instance.playerUICurrentPTManager.RefreshCurrentPTUI(currentPTID);
            ComparisonPT();
            entry.playableCharacterInventoryManager.RefreshDeployedPT();
        }

        /// <summary>
        /// currentPTとchangedPTを比較し、同一であればsavePTボタンを無効にする
        /// 現在は停止中(savePTを押した瞬間savePTボタンがinteractableになるとPADでのUIの操作が出来なくなるため)
        /// </summary>
        public void ComparisonPT()
        {
            //savePT.interactable = !currentPTID.SequenceEqual(changedPTID);
        }
    }
}
