using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayableCharacterInventoryManager : MonoBehaviour
    {
        [HideInInspector] public const int MaxPTCount = 4;
        EntryManager entry;
        public PlayableCharacter currentCharacter;
        [Header("Quick Slots")]
        public PlayableCharacter[] currentPCPT = new PlayableCharacter[4];
        public int currentCharacterIndex = 0;
        protected void Awake()
        {
            entry = GetComponent<EntryManager>();
        }
        public void SetActivePlayableCharacter(bool result, GameObject model)
        {
            if (model != null)
            {
                model.GetComponent<Animator>().enabled = result;
                model.GetComponent<PlayerAnimatorManager>().enabled = result;
                foreach (Transform item in model.transform)
                {
                    item.gameObject.SetActive(result);
                }
            }
        }

        /// <summary>
        /// 引数のmodelをアクティブキャラにする際に呼び出される
        /// instantiation
        /// </summary>
        /// <param name="characterModel"></param>
        public void PositioningCharacterModel(GameObject characterModel)
        {
            characterModel.transform.parent = transform;
            characterModel.transform.localPosition = Vector3.zero;
            characterModel.transform.localRotation = Quaternion.identity;
        }


        public void ChangeCharacter(int slotsIndex)
        {
            int newCharacterID = entry.playableCharacterEntryNetworkManager.currentPTID[slotsIndex];
            if (newCharacterID == WorldPlayableCharacterDatabase.instance.NoCharacter.playableCharacterID)
                return;
            entry.playableCharacterEntryManager.FindCharacterByIDFromChildren(newCharacterID);
        }
        private void DestroyChildAll(Transform root)
        {
            foreach (Transform child in root)
            {
                Destroy(child.gameObject);
            }
        }
        public void RefreshDeployedPT()
        {
            DestroyChildAll(transform);
            for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTID.Count; i++)
            {
                entry.playableCharacterInventoryManager.currentPCPT[i] = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(entry.playableCharacterEntryNetworkManager.currentPTID[i]);
            }

            if (entry.player.IsOwner)
            {
                entry.playableCharacterEntryNetworkManager.resetPTFire.Value = true;
            }


            entry.playableCharacterEntryManager.SetFirstCharacter();

            entry.player.playerCombatManager.lockOnTransform = entry.player.myLockOnTransform.transform;
        }
    }

}