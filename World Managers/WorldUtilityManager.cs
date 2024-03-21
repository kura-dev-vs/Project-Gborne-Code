using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace RK
{
    /// <summary>
    /// ワールドにおけるキャラクターレイヤーやステージのレイヤー等管理し参照しやすくする
    /// </summary>
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager instance;
        [Header("Layers")]
        [SerializeField] LayerMask characterLayers;
        [SerializeField] LayerMask enviroLayers;
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
        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }
        public LayerMask GetEnviroLayers()
        {
            return enviroLayers;
        }
        public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            if (attackingCharacter == CharacterGroup.Team01)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01:
                        return false;
                    case CharacterGroup.Team02:
                        return true;
                    default:
                        break;
                }
            }
            else if (attackingCharacter == CharacterGroup.Team02)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01:
                        return true;
                    case CharacterGroup.Team02:
                        return false;
                    default:
                        break;
                }
            }

            return false;
        }
        public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
        {
            targetDirection.y = 0;
            float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);
            Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);

            if (cross.y < 0) viewableAngle = -viewableAngle;

            return viewableAngle;
        }
    }
}
