using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace RK
{
    /// <summary>
    /// スキルやバーストのアクションを受け取ったら可能か判定し、可能であればアクションを始めサーバーへアクションid等を通知させる
    /// </summary>
    public class PlayableCharacterActionManager : NetworkBehaviour
    {
        [SerializeField] EntryManager entry;
        private void Awake()
        {
            entry = GetComponent<EntryManager>();
        }
        public void PerformPCBasedSkillAction(PCSkillAction skillAction, PlayerSkillManager pcSkill, string direction)
        {
            if (skillAction == null)
            {
                Debug.Log("DON'T SET SKILL ACTION");
                return;
            }
            // owner側がアクションを起こす
            skillAction.AttemptToPerformSkill(entry.player, pcSkill);

            // サーバーにアクションを実行したことを通知
            entry.playableCharacterEntryNetworkManager.NotifyTheServerOfSkillActionServerRpc(NetworkManager.Singleton.LocalClientId, skillAction.skillActionID, pcSkill.skill.skillID, direction);
        }
    }
}
