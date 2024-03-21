using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /*
    feceIcon (clone) に必要なコンポーネント
    @param faceIcon: UIに表示する画像
    @param alreadyJoinedSymbol: PTに加入済を示すGameObject
    @param nemberText: PT内の順番 (alreadyJoinedSymbol内のTextで表示) 
    @param numberInPt: PT内の順番 (加入していない場合"-1")
    */
    public class TransformOfFaceIcon : MonoBehaviour
    {
        public Image faceIcon;
        [HideInInspector] public PlayableCharacter pc;
        public GameObject alreadyJoinedSymbol;
        [SerializeField] TextMeshProUGUI numberText;
        [HideInInspector] public int numberInPT;
        public void SetNumberInPT(int number)
        {
            numberText.SetText((number + 1).ToString());
        }

        /*
        FaceIconボタンを押した場合の挙動
        PT内に非加入かつ空きがある場合PTに加入させる
        */
        public void ClickFaceIcon()
        {
            if (numberInPT == -1)
            {
                // PT内に空きがあるか (-1の場合空きがない)
                int result = PlayerUIManager.instance.playerUISelectableCharacterManager.CheckPTJoined(WorldPlayableCharacterDatabase.instance.NoCharacter.playableCharacterID);
                if (result != -1)
                {
                    PlayerUIManager.instance.playerUISelectableCharacterManager.ChangePTCharacter(result, pc.playableCharacterID);
                    SetNumberInPT(result);
                    alreadyJoinedSymbol.SetActive(true);
                    numberInPT = result;
                }
            }
            else
            {
                alreadyJoinedSymbol.SetActive(false);
                PlayerUIManager.instance.playerUISelectableCharacterManager.ChangePTCharacter(numberInPT, WorldPlayableCharacterDatabase.instance.NoCharacter.playableCharacterID);
                numberInPT = -1;
            }
            PlayerUIManager.instance.playerUISelectableCharacterManager.ComparisonPT();
        }
    }
}
