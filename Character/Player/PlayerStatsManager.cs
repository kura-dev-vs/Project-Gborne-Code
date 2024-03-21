using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// プレイヤーキャラ専用のステータス関連を追加したいときに使う予定
    /// </summary>
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponentInParent<PlayerManager>();
        }
        protected override void Start()
        {
            base.Start();
            if (player == null)
                return;
            // なぜここで計算するのか？
            // キャラクター作成メニューを作成し、クラスによってステータスを設定すると、そこで計算される
            // セーブファイルが存在する場合、シーンにロードするときに上書きされる
            //CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            //CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }

        public void SetManager()
        {
            character = GetComponentInParent<CharacterManager>();
            player = GetComponentInParent<PlayerManager>();
        }

    }
}
