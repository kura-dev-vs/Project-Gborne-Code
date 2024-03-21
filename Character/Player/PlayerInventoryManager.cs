using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 現在装備中の武器関連の管理を行う
    /// </summary>
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public WeaponItem currentRightHandWeapon;
        public WeaponItem currentLeftHandWeapon;
        [Header("Quick Slots")]
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
        public int rightHandWeaponIndex = 0;
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
        public int leftHandWeaponIndex = 0;
    }
}
