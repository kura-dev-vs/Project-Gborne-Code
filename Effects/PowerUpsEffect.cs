using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Power Ups Effect")]
    public class PowerUpsEffect : StaticCharacterEffect
    {
        [SerializeField] int strengthGainedFromWEapon;

        public override void ProcessStaticEffect(CharacterManager character)
        {
            base.ProcessStaticEffect(character);
            if (character.IsOwner)
            {
                strengthGainedFromWEapon = Mathf.RoundToInt(character.characterNetworkManager.strength.Value / 2);
                Debug.Log("strength gained " + strengthGainedFromWEapon);
                character.characterNetworkManager.strengthModifier.Value += strengthGainedFromWEapon;
            }
        }
        public override void RemoveStaticEffect(CharacterManager character)
        {
            base.RemoveStaticEffect(character);
            if (character.IsOwner)
            {

                character.characterNetworkManager.strengthModifier.Value -= strengthGainedFromWEapon;
            }
        }
    }
}
