using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// boss Character用のsleep ステート
    /// </summary>
    [CreateAssetMenu(menuName = "A.I/States/Boss Sleep")]
    public class BossSleepState : AIState
    {
        public override AIState Tick(AICharacterManager aICharacter)
        {
            return base.Tick(aICharacter);
        }

    }
}
